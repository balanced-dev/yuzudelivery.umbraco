using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using System.Reflection;
using YuzuDelivery.Umbraco.Grid;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterGridStrategies(this Composition composition, Assembly assembly)
        {
            composition.RegisterAll<IGridItem>(assembly);
            composition.RegisterAll<IAutomaticGridConfig>(assembly);
        }
    }
}