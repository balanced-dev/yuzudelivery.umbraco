using YuzuDelivery.Umbraco.Core;

namespace Yuzu.TestProject.SetupSteps;

public class CreateViewModels : IYuzuTestSetupStep
{
    private readonly GenerateController _generate;

    public int Index => 0;
    public string Description => "Create ViewModels";
    public bool RequiresRestart => true;

    public CreateViewModels(GenerateController generate) => _generate = generate;

    public void Execute() => _generate.Build();
}