using System;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using YuzuDelivery.Umbraco.Grid;
using YuzuDelivery.Umbraco.BlockList;
using YuzuDelivery.Umbraco.Forms;

namespace Umbraco.Cms.Web.UI
{
    public class YuzuComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.Services.AddSingleton<IYuzuConfiguration>((factory) => new DefaultUmbracoConfig(new Umb9Factory(factory), assembly));
            builder.Services.AddSingleton<IYuzuViewmodelsBuilderConfig>((factory) => new DefaultUmbracoVmBuilderConfig(new Umb9Factory(factory)));
            builder.Services.AddSingleton<IYuzuDeliveryImportConfiguration>((factory) => new DefaultUmbracoImportConfig(factory, assembly));

            builder.RegisterYuzuMapping(assembly);

            builder.RegisterFormStrategies(assembly);

            //choose only one!
            builder.RegisterGridStrategies(assembly);
            builder.RegisterBlockListStrategies(assembly);
        }
    }
}