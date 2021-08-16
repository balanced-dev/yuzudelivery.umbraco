using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Core.Composing;
using System.Reflection;
using YuzuDelivery.Umbraco.BlockList;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterBlockListStrategies(this Composition composition, Assembly assembly)
        {
            composition.RegisterAll<IGridItem>(assembly);
        }
    }
}
