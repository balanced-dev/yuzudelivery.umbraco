namespace Yuzu.TestProject.SetupSteps;

public interface IYuzuTestSetupStep
{
    public int Index { get;  }
    public string Description { get; }
    public bool RequiresRestart { get;  }
    public void Execute();
}