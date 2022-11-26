using System.Reflection;
using YuzuDelivery.Umbraco.Grid;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterGridStrategies(this IServiceCollection services, Assembly assembly)
        {
            services.RegisterAll<IGridItem>(assembly);
            services.RegisterAll<IAutomaticGridConfig>(assembly);
        }
    }
}
