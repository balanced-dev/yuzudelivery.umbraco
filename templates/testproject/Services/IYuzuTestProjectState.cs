namespace YuzuDelivery.Umbraco.TestProject.Services;

public interface IYuzuTestProjectState
{
    int GetNextStep();
    void MarkStepCompleted(int step);
}