using Umbraco.Cms.Core.Web;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;
using Umb = Umbraco.Cms.Core.Services;
using Mod = Umbraco.Cms.Core.Models;

namespace Yuzu.TestProject.SetupSteps;

public class CreateContent : IYuzuTestSetupStep
{
    private readonly Umb.IContentService _contentService;
    private readonly IContentTypeService _contentTypeService;
    private readonly YuzuContentImportController _contentImport;
    private readonly Umb.IFileService _fileService;
    private readonly IUmbracoContextFactory _umbracoContextFactory;

    public int Index => 5;
    public string Description => "Create Content";
    public bool RequiresRestart => false;

    public CreateContent(
        Umb.IContentService contentService,
        IContentTypeService contentTypeService,
        YuzuContentImportController contentImport,
        Umb.IFileService fileService,
        IUmbracoContextFactory umbracoContextFactory)
    {
        _contentService = contentService;
        _contentTypeService = contentTypeService;
        _contentImport = contentImport;
        _fileService = fileService;
        _umbracoContextFactory = umbracoContextFactory;
    }

    public void Execute()
    {
        using var umbRef = _umbracoContextFactory.EnsureUmbracoContext();

        var homeContent = _contentService.Create("Home", -1, "home");
        _contentService.SaveAndPublish(homeContent);

        CreateUmbracoContent("gridPage", homeContent);
        CreateUmbracoContent("rowPage", homeContent);
        CreateUmbracoContent("basicPage", homeContent);
        CreateUmbracoContent("formPage", homeContent, false);
        CreateUmbracoContent("transmuted", homeContent);
    }

    private void CreateUmbracoContent(string name, Mod.IContent parent, bool import = true)
    {
        var contenttype = _contentTypeService.GetByAlias(name);
        var content = new Mod.Content(name.FirstCharacterToUpper(), parent, contenttype.Umb());
        content.TemplateId = GetTemplateId(name);
        _contentService.SaveAndPublish(content);

        if (import)
        {
            ImportContent(name, content);
        }
    }

    private int GetTemplateId(string alias)
    {
        var template = _fileService.GetTemplate(alias);
        return template?.Id ?? -1;
    }

    private void ImportContent(string name, Mod.IContent content)
    {
        _contentImport.Import(new ImportContentFromFileVm()
        {
            Content = content.Id,
            DocumentTypeName = name,
            File = new DataFileLocationsVm()
            {
                Location = "Main",
                Filename = $"{name}.json"
            },
            Viewmodel = $"vmPage_{name.FirstCharacterToUpper()}"
        });
    }
}