using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using YuzuDelivery.Umbraco.Grid;

#if NETCOREAPP
using Umbraco.Cms.Core.DependencyInjection;
#else
using Umbraco.Core.Composing;
#endif

namespace YuzuDelivery.Umbraco.Core
{
#if NETCOREAPP
    public static class CompositionExtensions
    {
        public static void RegisterGridStrategies(this IUmbracoBuilder composition, Assembly assembly)
        {
            composition.RegisterAll<IGridItem>(assembly);
            composition.RegisterAll<IAutomaticGridConfig>(assembly);
        }
    }
#else
    public static class CompositionExtensions
    {
        public static void RegisterGridStrategies(this Composition composition, Assembly assembly)
        {
            composition.RegisterAll<IGridItem>(assembly);
            composition.RegisterAll<IAutomaticGridConfig>(assembly);
        }
    }
#endif
}