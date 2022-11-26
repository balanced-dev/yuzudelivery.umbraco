using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        // ReSharper disable once UnusedMember.Global - Used by downstream projects (and templates)
        public static void RegisterYuzuAutoMapping(this IServiceCollection services, Assembly profileAssembly)
            => services.AddSingleton(sp => sp.GetRequiredService<DefaultYuzuMapperFactory>().Create(
                (settings, cfg, mapContext) =>
                {
                    settings.MappingAssemblies.Add(profileAssembly);
                    cfg.AddProfilesForAttributes(settings, mapContext, sp);
                }));
    }
}
