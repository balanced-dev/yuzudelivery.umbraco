using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using System.Reflection;
using YuzuDelivery.Umbraco.Forms;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterFormStrategies(this Composition composition, Assembly assembly)
        {
            composition.RegisterAll<IFormFieldMappings>(assembly);
            composition.RegisterAll<IFormFieldPostProcessor>(assembly);
        }
    }
}