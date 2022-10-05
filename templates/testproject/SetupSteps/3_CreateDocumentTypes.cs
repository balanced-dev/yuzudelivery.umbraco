using Umbraco.Cms.Web.BackOffice.ModelsBuilder;
using YuzuDelivery.Umbraco.Import;

namespace Yuzu.TestProject.SetupSteps;

public class CreateDocumentTypes : IYuzuTestSetupStep
{
    private readonly SchemaChangeController _schemaChangeController;
    private readonly ModelsBuilderDashboardController _modelsBuilderDashboardController;

    public int Index => 3;
    public string Description => "Create Document Types";
    public bool RequiresRestart => true;

    public CreateDocumentTypes(SchemaChangeController schemaChangeController, ModelsBuilderDashboardController modelsBuilderDashboardController)
    {
        _schemaChangeController = schemaChangeController;
        _modelsBuilderDashboardController = modelsBuilderDashboardController;
    }

    public void Execute()
    {
        _schemaChangeController.ChangeDocumentTypes();

        try
        {
            _modelsBuilderDashboardController.BuildModels();
        }
        catch(Exception ex)
        {
            if (ex is not NullReferenceException)
            {
                throw new Exception("Error creating models", ex);
            }
        }
    }
}