using System.Reflection;
using Umbraco.Cms.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Core.Startup;
using YuzuDelivery.TemplateEngines.Handlebars;

namespace Umbraco.Cms.Web.UI
{
    public class YuzuComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.Services.AddSingleton<IYuzuViewmodelsBuilderConfig, DefaultUmbracoVmBuilderConfig>();
            builder.Services.Configure<YuzuConfiguration>(cfg =>
            {
                cfg.AddToModelRegistry(assembly);
                cfg.AddInstalledManualMaps(assembly);
            });

            builder.Services.AddYuzuHandlebars();
            builder.Services.AddYuzuCore();
    
            builder.Services.RegisterYuzuManualMapping(assembly);

            builder.Services.RegisterFormStrategies(assembly);

            //you can swap this out for grid
            builder.Services.RegisterBlockListStrategies(assembly);
        }
    }
}
