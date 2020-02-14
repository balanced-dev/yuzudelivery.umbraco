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

namespace $rootnamespace$
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var localAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name == "$rootnamespace$").FirstOrDefault();
            var localMappingAssembly = Assembly.GetExecutingAssembly();

            composition.Register<IYuzuConfiguration>((factory) => new DefaultUmbracoConfig(factory, localAssembly), Lifetime.Singleton);
            composition.Register<IYuzuViewmodelsBuilderConfig>((factory) => new DefaultUmbracoVmBuilderConfig(factory), Lifetime.Singleton);
            composition.Register<IYuzuDeliveryImportConfiguration>((factory) => new DefaultUmbracoImportConfig(factory, localAssembly), Lifetime.Singleton);

            composition.RegisterYuzuMapping(localMappingAssembly);

            composition.RegisterFormStrategies(localMappingAssembly);
            composition.RegisterGridStrategies(localMappingAssembly);
        }
    }
}