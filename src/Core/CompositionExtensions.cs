using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using System.Reflection;
using Umbraco.Core;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterAll<T>(this Composition composition, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(T)));
            foreach (var f in types)
            {
                composition.Register(typeof(T), f);
            }
        }

        public static void RegisterYuzuMapping(this Composition composition, Assembly profileAssembly)
            => composition.Register<IMapper>((factory) =>
            {
                var types = profileAssembly.GetTypes();
                var allowedTypes = new Type[] { typeof(YuzuMappingConfig) };
                var allowedInterfaces = new Type[] { typeof(IYuzuAfterMapResolver), typeof(IYuzuTypeConvertor), typeof(IYuzuPropertyResolver), typeof(IYuzuFullPropertyResolver) };

                foreach (var i in types.Where(x => allowedTypes.Contains(x.BaseType) || allowedInterfaces.Intersect(x.GetInterfaces()).Any()))
                {
                    if(i.BaseType == typeof(YuzuMappingConfig))
                    {
                        composition.Register(typeof(YuzuMappingConfig), i);
                    }
                    else
                    {
                        composition.Register(i);
                    }

                }

                return factory.GetInstance<DefaultUmbracoMappingFactory>().Create(profileAssembly, factory);
            }, Lifetime.Singleton);
    }
}
