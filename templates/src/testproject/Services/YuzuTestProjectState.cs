using Umbraco.Cms.Core.Services;

namespace YuzuDelivery.Umbraco.TestProject.Services;

public class YuzuTestProjectState : IYuzuTestProjectState
{
    private const string Key = "yuzu:testproject:completed";
    private readonly IKeyValueService _keyValueService;

    public YuzuTestProjectState(IKeyValueService keyValueService)
    {
        _keyValueService = keyValueService;
    }

    public int GetNextStep()
    {
        var value = _keyValueService.GetValue(Key);
        var lastCompleted = string.IsNullOrEmpty(value) ? -1 : Convert.ToInt32(value);
        return lastCompleted + 1;
    }

    public void MarkStepCompleted(int step) => _keyValueService.SetValue(Key, $"{step}");
}