using YuzuDelivery.Umbraco.Import;
using Umb = Umbraco.Cms.Core.Services;
using Mod = Umbraco.Cms.Core.Models;

namespace Yuzu.TestProject.SetupSteps;

public class CreateTemplatesAndPermissions : IYuzuTestSetupStep
{
    private readonly Umb.IContentTypeService _umbContentTypeService;
    private readonly IContentTypeService _contentTypeService;
    private readonly Umb.IFileService _fileService;

    public int Index => 4;
    public string Description => "Create Templates and Permissions";
    public bool RequiresRestart => false;

    public CreateTemplatesAndPermissions(Umb.IContentTypeService umbContentTypeService,
        IContentTypeService contentTypeService,
        Umb.IFileService fileService)
    {
        _umbContentTypeService = umbContentTypeService;
        _contentTypeService = contentTypeService;
        _fileService = fileService;
    }

    public void Execute()
    {
        var home = _contentTypeService.Create("Home", "home", false).Umb();
        home.AllowedAsRoot = true;
        AddTemplateToContentType(home);

        var rowPage = GetContentAddTemplate("rowPage");
        var gridPage = GetContentAddTemplate("gridPage");
        var basicPage = GetContentAddTemplate("basicPage");
        var formPage = GetContentAddTemplate("formPage");
        var transmuted = GetContentAddTemplate("transmuted");

        _contentTypeService.AddPermissions(home.Id, new List<IContentType>() { rowPage.Yuzu(), gridPage.Yuzu(), basicPage.Yuzu(), formPage.Yuzu(), transmuted.Yuzu() });
    }

    private void AddTemplateToContentType(Mod.IContentType contentType)
    {
        var result = _fileService.CreateTemplateForContentType(contentType.Alias, contentType.Name);
        contentType.AllowedTemplates = new List<Mod.ITemplate>() { result.Result.Entity };
        _umbContentTypeService.Save(contentType);
        contentType.SetDefaultTemplate(result.Result.Entity);
    }

    private Mod.IContentType GetContentAddTemplate(string alias)
    {
        var contentType = _umbContentTypeService.Get(alias);
        AddTemplateToContentType(contentType);
        return contentType;
    }
}