﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using YuzuDelivery.Core;

#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core;
#else
using Umbraco.Core.Composing;
using Umbraco.Core;
#endif

namespace YuzuDelivery.Umbraco.Core
{

#if NETCOREAPP
    public static class CompositionExtensions
    {
        public static void RegisterAll<T>(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(T)));
            foreach (var f in types)
            {
                services.AddTransient(typeof(T), f);
            }
        }

        public static void RegisterYuzuAutoMapping(this IServiceCollection services, Assembly profileAssembly)
            => services.AddSingleton<IMapper>((factory) =>
            {
                return factory.GetService<DefaultUmbracoMappingFactory>().Create(profileAssembly, new Umb9Factory(factory));
            });
    
        public static void RegisterYuzuManualMapping(this IServiceCollection services, Assembly profileAssembly)
        {
            var types = profileAssembly.GetTypes();
            var allowedTypes = new Type[] { typeof(YuzuMappingConfig) };
            var allowedInterfaces = new Type[] { typeof(IYuzuTypeAfterConvertor), typeof(IYuzuTypeConvertor), typeof(IYuzuTypeFactory), typeof(IYuzuPropertyAfterResolver), typeof(IYuzuPropertyReplaceResolver), typeof(IYuzuFullPropertyResolver) };

            foreach (var i in types.Where(x => allowedTypes.Contains(x.BaseType) || allowedInterfaces.Intersect(x.GetInterfaces()).Any()))
            {
                if (i.BaseType == typeof(YuzuMappingConfig))
                {
                    services.AddSingleton(typeof(YuzuMappingConfig), i);
                }
                else
                {
                    if (i.GetInterfaces().Any(x => x == typeof(IYuzuTypeFactory)))
                        services.AddSingleton(typeof(IYuzuTypeFactory), i);

                    services.AddSingleton(i);
                }
            }
        }
    }

        
#else
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
                var allowedInterfaces = new Type[] { typeof(IYuzuTypeAfterConvertor), typeof(IYuzuTypeConvertor), typeof(IYuzuTypeFactory), typeof(IYuzuPropertyAfterResolver), typeof(IYuzuPropertyReplaceResolver), typeof(IYuzuFullPropertyResolver) };

                foreach (var i in types.Where(x => allowedTypes.Contains(x.BaseType) || allowedInterfaces.Intersect(x.GetInterfaces()).Any()))
                {
                    if(i.BaseType == typeof(YuzuMappingConfig))
                    {
                        composition.Register(typeof(YuzuMappingConfig), i);
                    }
                    else
                    {
                        if (i.GetInterfaces().Any(x => x == typeof(IYuzuTypeFactory)))
                            composition.Register(typeof(IYuzuTypeFactory), i);

                        composition.Register(i);
                    }
                }

                return factory.GetInstance<DefaultUmbracoMappingFactory>().Create(profileAssembly, new Umb8Factory(factory));
            }, Lifetime.Singleton);
    }
#endif

}
