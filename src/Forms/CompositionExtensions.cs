using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using YuzuDelivery.Umbraco.Forms;

#if NETCOREAPP
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models.Blocks;
#else
using Umbraco.Core.Composing;
#endif

namespace YuzuDelivery.Umbraco.Core
{

    public static class CompositionExtensions
    {
#if NETCOREAPP
        public static void RegisterFormStrategies(this IUmbracoBuilder builder, Assembly assembly)
        {
            builder.RegisterAll<IFormFieldMappings>(assembly);
            builder.RegisterAll<IFormFieldPostProcessor>(assembly);
        }
#else
        public static void RegisterFormStrategies(this Composition composition, Assembly assembly)
        {
            composition.RegisterAll<IFormFieldMappings>(assembly);
            composition.RegisterAll<IFormFieldPostProcessor>(assembly);
        }
#endif
    }
}