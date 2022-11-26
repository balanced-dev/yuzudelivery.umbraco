using System.Reflection;
using YuzuDelivery.Umbraco.BlockList;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterBlockListStrategies(this IServiceCollection services, Assembly assembly)
        {
            services.RegisterAll<IGridItem>(assembly);
        }
    }
}
