using YuzuDelivery.Umbraco.TestProject.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using Yuzu.TestProject.SetupSteps;

namespace Yuzu.TestProject;

public class YuzuTestProjectComponent : IComponent
{
    private readonly ILogger<YuzuTestProjectComponent> _logger;
    private readonly IRuntimeState _runtimeState;
    private readonly IYuzuTestProjectState _yuzuState;
    private readonly IEnumerable<IYuzuTestSetupStep> _steps;
    private readonly IHostEnvironment _env;

    public YuzuTestProjectComponent(
        ILogger<YuzuTestProjectComponent> logger,
        IRuntimeState runtimeState,
        IYuzuTestProjectState yuzuState,
        IEnumerable<IYuzuTestSetupStep> steps,
        IHostEnvironment env)
    {
        _logger = logger;
        _runtimeState = runtimeState;
        _yuzuState = yuzuState;
        _steps = steps;
        _env = env;
    }

    public void Initialize()
    {
        if (_runtimeState.Level != RuntimeLevel.Run)
        {
            return;
        }

        var nextStep = _yuzuState.GetNextStep();

        foreach (var step in _steps.Where(x => x.Index >= nextStep).OrderBy(x => x.Index))
        {
            _logger.LogWarning("[Yuzu] Executing setup step: {description}", step.Description);
            step.Execute();

            _yuzuState.MarkStepCompleted(step.Index);

             _logger.LogWarning("[Yuzu] Completed setup step: {description}", step.Description);
            if (!step.RequiresRestart)
            {
                continue;
            }

            _logger.LogWarning("[Yuzu] Restart required for test project setup step: {description}", step.Description);
            var restartTrigger = Path.Combine(_env.ContentRootPath, "restart.txt");
            File.WriteAllText(restartTrigger, $"{step.Index}");
            break;
        }

        if(_yuzuState.GetNextStep() > _steps.Max(x => x.Index))
        {
             _logger.LogWarning("[Yuzu] Setup complete");
        }
    }

    public void Terminate()
    { }
}
