using YuzuDelivery.Umbraco.Import;

namespace Yuzu.TestProject.SetupSteps;

public class CreateGroups : IYuzuTestSetupStep
{
    private readonly SchemaChangeController _schemaChange;

    public int Index => 1;
    public string Description => "Create Groups";
    public bool RequiresRestart => false;

    public CreateGroups(SchemaChangeController schemaChange) => _schemaChange = schemaChange;

    public void Execute()
    {
        _schemaChange.ApplyMultipleGroups(new List<GroupReturn>
        {
            new()
            {
                ChildViewmodel = "vmBlock_Grouped",
                PropertyName = "Grouped",
                Group = "Grouped",
                ParentViewmodel = "vmPage_Transmuted",
                ToAdd = true
            }
        });
    }
}