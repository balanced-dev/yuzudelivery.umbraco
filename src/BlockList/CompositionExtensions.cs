using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using YuzuDelivery.Umbraco.BlockList;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models.Blocks;

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
