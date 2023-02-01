using Umbraco.Forms.Web.Editors;
using Umbraco.Cms.Core.Services;

namespace Yuzu.TestProject.SetupSteps;

public class CreateAndAssignForm : IYuzuTestSetupStep
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IContentTypeService _contentTypeService;
    private readonly IContentService _contentService;

    public CreateAndAssignForm(IServiceScopeFactory scopeFactory, IContentTypeService contentTypeService, IContentService contentService)
    {
        _scopeFactory = scopeFactory;
        _contentTypeService = contentTypeService;
        _contentService = contentService;
    }

    public int Index => 6;
    public string Description => "Create and assign contact form";
    public bool RequiresRestart => false;
    public void Execute()
    {
        using var scope = _scopeFactory.CreateScope();

        var formController = ActivatorUtilities.CreateInstance<FormController>(scope.ServiceProvider);
        var formDesign = formController.GetScaffoldWithWorkflows("contactForm");
        var form = formController.SaveForm(formDesign);

        var ct = _contentTypeService.Get("formPage")!;
        var content = _contentService.GetPagedOfType(ct.Id, 0, 1, out var total, null).First();

        content.SetValue("form", form.Value!.Id);
        _contentService.SaveAndPublish(content);
    }
}