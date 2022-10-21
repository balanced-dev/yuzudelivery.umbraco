using YuzuDelivery.Umbraco.Import;
using Umb = Umbraco.Cms.Core.Services;

namespace Yuzu.TestProject.SetupSteps;

public class CreateGlobals : IYuzuTestSetupStep
{
    private readonly IContentTypeService _contentTypeService;
    private readonly Umbraco.Cms.Core.Services.IContentService _contentService;
    private readonly SchemaChangeController _schemaChangeController;

    public int Index => 2;
    public string Description => "Create Globals";
    public bool RequiresRestart => false;

    public CreateGlobals(
        IContentTypeService contentTypeService,
        Umb.IContentService contentService,
        SchemaChangeController schemaChangeController)
    {
        _contentTypeService = contentTypeService;
        _contentService = contentService;
        _schemaChangeController = schemaChangeController;
    }

    public void Execute()
    {
        var globalArea = _contentTypeService.GetByAlias("globalArea").Umb();
        if(globalArea == null)
        {
            _ = _contentTypeService.Create("Global Area", "globalArea", false).Umb();
        }

        var globalAreaContent = _contentService.Create("Global", -1, "globalArea");
        _ = _contentService.SaveAndPublish(globalAreaContent);

        var globalSettings = new GlobalStoreContentAs()
        {
            ParentContentId = globalAreaContent.Id.ToString(),
            PrimaryPropertyName = "string",
            Type = StoreContentAsType.Global
        };

        _schemaChangeController.ApplySettings(new ApplySettingsReturn()
        {
            ViewmodelName = "vmBlock_Global",
            StoreContentAs = Newtonsoft.Json.JsonConvert.SerializeObject(globalSettings)
        });
    }
}