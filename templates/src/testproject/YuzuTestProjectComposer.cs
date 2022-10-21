using Umbraco.Cms.Core.Composing;
using Yuzu.TestProject.SetupSteps;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.TestProject.Services;

namespace Yuzu.TestProject;

public class YuzuTestProjectComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<GenerateController>();
        builder.Services.AddTransient<YuzuContentImportController>();
        builder.Services.AddTransient<Umbraco.Cms.Web.BackOffice.ModelsBuilder.ModelsBuilderDashboardController>();

        builder.AddComponent<YuzuTestProjectComponent>();

        builder.Services.AddSingleton<IYuzuTestProjectState, YuzuTestProjectState>();

        builder.Services.AddTransient<IYuzuTestSetupStep, CreateViewModels>();
        builder.Services.AddTransient<IYuzuTestSetupStep, CreateGroups>();
        builder.Services.AddTransient<IYuzuTestSetupStep, CreateGlobals>();
        builder.Services.AddTransient<IYuzuTestSetupStep, CreateDocumentTypes>();
        builder.Services.AddTransient<IYuzuTestSetupStep, CreateTemplatesAndPermissions>();
        builder.Services.AddTransient<IYuzuTestSetupStep, CreateContent>();
    }
}
