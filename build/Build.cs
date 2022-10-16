using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LittleForker;
using Microsoft.Extensions.Logging.Abstractions;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static SimpleExec.Command;

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    [NerdbankGitVersioning]
    readonly NerdbankGitVersioning NerdbankVersioning;

    readonly DateTime BuildDate;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath AcceptanceTestsDirectory => RootDirectory / "acceptance";
    AbsolutePath AcceptanceTestResults => AcceptanceTestsDirectory / "test-results";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath PackagesDirectory => OutputDirectory / "packages";
    AbsolutePath TestResultsDirectory => OutputDirectory / "test_results";
    AbsolutePath TestServerDirectory => OutputDirectory / ".test_server";
    AbsolutePath AcceptanceTestsZip => OutputDirectory / "acceptance.zip";


    public Build()
    {
        BuildDate = DateTime.Now;
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(PackagesDirectory);
            EnsureCleanDirectory(TestResultsDirectory);
            EnsureCleanDirectory(TestServerDirectory);
        });


    Target AddAzureArtifactsFeed => _ => _
        .OnlyWhenStatic(() => IsServerBuild)
        .DependentFor(Restore)
        .Executes(() =>
            DotNetNuGetAddSource(s => s
              .SetName("Azure Artifacts")
              .SetSource("https://pkgs.dev.azure.com/hifiagency/Yuzu/_packaging/Yuzu.Delivery/nuget/v3/index.json")
              .SetUsername("devops")
              .SetPassword(AzurePipelines.Instance.AccessToken))
        );

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = Solution.GetProjects("*.Tests");

            try
            {
                DotNetTest(s => s
                    .SetConfiguration(Configuration)
                    .SetResultsDirectory(TestResultsDirectory)
                    .EnableNoBuild()
                    .CombineWith(testProjects, (_, project) => _
                        .SetProjectFile(project)
                        .SetLoggers($"trx;LogFileName={project.Name}.trx")),
                    completeOnFailure: true);
            }
            finally
            {
                ReportTestResults();
            }
        });

    void ReportTestResults()
    {
        TestResultsDirectory.GlobFiles("*.trx").ForEach(x =>
            AzurePipelines.Instance?.PublishTestResults(
                type: AzurePipelinesTestResultsType.VSTest,
                title: $"{Path.GetFileNameWithoutExtension(x)}",
                files: new string[] { x }));
    }

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(s => s
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(PackagesDirectory)
                .SetConfiguration(Configuration));
        });

    Target Report => _ => _
        .Before(Compile)
        .Executes(() =>
        {
            Log.Information("NuGetPackageVersion:          {Value}", NerdbankVersioning.NuGetPackageVersion);
            Log.Information("AssemblyInformationalVersion: {Value}", NerdbankVersioning.AssemblyInformationalVersion);
        });


    Target Acceptance  => _ => _
        .After(Test)
        .DependsOn(Pack)
        .Produces(AcceptanceTestsZip)
        .Executes(async () =>
        {

            try
            {
                var templatesPackage = PackagesDirectory
                                       .GlobFiles("*YuzuDelivery.Umbraco.Templates.*")
                                       .First();

                Run("dotnet", $"nuget add source {PackagesDirectory} --name yuzu_local", handleExitCode: c => true);
                Run("dotnet", "new --uninstall YuzuDelivery.Umbraco.Templates", handleExitCode: c => true);
                Run("dotnet", $"new --install {templatesPackage}");

                Directory.CreateDirectory(TestServerDirectory);

                var newArgs = new StringBuilder();
                newArgs.Append(" --name Yuzu.Acceptance");
                newArgs.Append(" --output .");
                newArgs.Append(" --no-restore");
                newArgs.Append(" --development-database-type SQLite");
                newArgs.Append(" --email e2e@hifi.agency");
                newArgs.Append(" --friendly-name e2e@hifi.agency");
                newArgs.Append(" --password TestThis42!");

                Run("dotnet", $"new yuzu-test {newArgs}", workingDirectory: TestServerDirectory);
                Run("dotnet", $"restore", workingDirectory: TestServerDirectory);

                var supervisor = new ProcessSupervisor(NullLoggerFactory.Instance, ProcessRunType.NonTerminating,
                    TestServerDirectory, "dotnet", "watch run -- --urls http://localhost:8080");

                await supervisor.Start();

                try
                {
                    NpmTasks.NpmCi(s => s
                        .SetProcessWorkingDirectory(AcceptanceTestsDirectory));

                    NpmTasks.NpmRun(s => s
                         .SetProcessWorkingDirectory(AcceptanceTestsDirectory)
                         .SetCommand("browsers"));

                    NpmTasks.NpmRun(s => s
                         .SetProcessWorkingDirectory(AcceptanceTestsDirectory)
                         .SetCommand("wait"));

                    NpmTasks.NpmRun(s => s
                         .SetProcessWorkingDirectory(AcceptanceTestsDirectory)
                         .SetCommand("test"));
                }
                finally
                {
                    await supervisor.Stop(TimeSpan.FromSeconds(3));
                }
            }
            finally
            {
                AcceptanceTestsDirectory.GlobFiles("**/*.junit.xml").ForEach(x =>
                    AzurePipelines.Instance?.PublishTestResults(
                        type: AzurePipelinesTestResultsType.JUnit,
                        title: "Acceptance Tests",
                        files: new string[] { x }));

                ZipFile.CreateFromDirectory(AcceptanceTestResults, AcceptanceTestsZip);

            }
        });

    Target Default => _ => _
        .DependsOn(Clean, Test, Acceptance, Report);
}
