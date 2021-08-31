using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using YuzuDelivery.Umbraco.Grid;
using YuzuDelivery.Umbraco.Forms;

namespace $rootnamespace$
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var assembly = Assembly.GetExecutingAssembly();

            composition.Register<IYuzuConfiguration>((factory) => new DefaultUmbracoConfig(factory, assembly), Lifetime.Singleton);
            composition.Register<IYuzuViewmodelsBuilderConfig>((factory) => new DefaultUmbracoVmBuilderConfig(factory), Lifetime.Singleton);
            composition.Register<IYuzuDeliveryImportConfiguration>((factory) => new DefaultUmbracoImportConfig(factory, assembly), Lifetime.Singleton);

            composition.RegisterYuzuMapping(assembly);

            composition.RegisterFormStrategies(assembly);
            //swap out for block list if using!
            composition.RegisterGridStrategies(assembly);
        }
    }
}