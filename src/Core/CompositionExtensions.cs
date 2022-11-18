using System;
using System.Linq;
using System.Reflection;
using YuzuDelivery.Core;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.AutoMapper;

namespace YuzuDelivery.Umbraco.Core
{
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

        // ReSharper disable once UnusedMember.Global - Used by downstream projects (and templates)
        public static void RegisterYuzuAutoMapping(this IServiceCollection services, Assembly profileAssembly)
            => services.AddSingleton(sp => sp.GetRequiredService<DefaultYuzuMapperFactory>().Create(
                (settings, cfg, mapContext) =>
                {
                    settings.MappingAssemblies.Add(profileAssembly);
                    cfg.AddProfilesForAttributes(settings, mapContext, sp);
                }));

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
}
