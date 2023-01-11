using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.Settings.Validators;

namespace YuzuDelivery.Umbraco.Core.Tests.Startup;

public class DefaultUmbracoVmBuilderConfigTests
{
    [Test]
    [TestCase(@"c:\www\", @"c:\www\foo\bar", false, false)]
    [TestCase(@"c:\www\", @"c:\notwww\foo\bar", false, true)]
    [TestCase(@"c:\www\", @"c:\www\..\notwww\foo\bar", false, true)]
    [TestCase(@"c:\www\", @"c:\notwww\foo\bar", true, false)]
    public void Constructor_WithAbsolutePathsForVmGenerationDirectory_RespectsAllowUnsafeConfiguration(
        string contentRoot,
        string viewModelsDirectory,
        bool allowUnsafe,
        bool shouldThrow)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore();
        }

        var hostEnvironment = Substitute.For<IHostEnvironment>();
        hostEnvironment.ContentRootPath.Returns(contentRoot);

        var settings = new ViewModelGenerationSettings
        {
            Directory = viewModelsDirectory,
            AcceptUnsafeDirectory = allowUnsafe
        };

        var validator = new ViewModelGenerationUnsafeDirectoryValidator(hostEnvironment);

        if (!shouldThrow)
        {
            Assert.That(validator.Validate("", settings).Succeeded, Is.True);
        }

        else
        {
            Assert.That(validator.Validate("", settings).Succeeded, Is.False);
        }
    }

    [Test]
    [TestCase(@"c:\www\", @"./bar", false, false)]
    [TestCase(@"c:\www\", @"../bar", false, true)]
    [TestCase(@"c:\www\", @"../bar", true, false)]
    public void Constructor_WithRelativePathsForVmGenerationDirectory_RespectsAllowUnsafeConfiguration(
        string contentRoot,
        string viewModelsDirectory,
        bool allowUnsafe,
        bool shouldThrow)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore();
        }

        var hostEnvironment = Substitute.For<IHostEnvironment>();
        hostEnvironment.ContentRootPath.Returns(contentRoot);

        var settings = new ViewModelGenerationSettings
        {
            Directory = viewModelsDirectory,
            AcceptUnsafeDirectory = allowUnsafe
        };

        var validator = new ViewModelGenerationUnsafeDirectoryValidator(hostEnvironment);

        if (!shouldThrow)
        {
            Assert.That(validator.Validate("", settings).Succeeded, Is.True);
        }

        else
        {
            Assert.That(validator.Validate("", settings).Succeeded, Is.False);
        }
    }
}
