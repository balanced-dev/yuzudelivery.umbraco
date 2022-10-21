using System.Reflection;
using Umbraco.Cms.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Core.Startup;

namespace Umbraco.Cms.Web.UI
{
    public class YuzuComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.Services.AddSingleton<IYuzuViewmodelsBuilderConfig, DefaultUmbracoVmBuilderConfig>();
            builder.Services.AddSingleton<IYuzuConfiguration>(sp =>  ActivatorUtilities.CreateInstance<DefaultUmbracoConfig>(sp, assembly));
            builder.Services.AddSingleton<IYuzuDeliveryImportConfiguration>(sp => ActivatorUtilities.CreateInstance<DefaultUmbracoImportConfig>(sp, assembly));

            builder.Services.RegisterYuzuAutoMapping(assembly);
            builder.Services.RegisterYuzuManualMapping(assembly);

            builder.Services.RegisterFormStrategies(assembly);

            //you can swap this out for grid
            builder.Services.RegisterBlockListStrategies(assembly);
        }
    }
}