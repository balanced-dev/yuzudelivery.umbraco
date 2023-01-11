using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public static class AutomapperConfigExtensions
    {
        public static void AddProfilesForAttributes(this MapperConfigurationExpression cfg, YuzuConfiguration config, AddedMapContext mapContext, IServiceProvider serviceProvider)
        {
            var allTypes = config.MappingAssemblies
                                 .Where(a => a != typeof(YuzuMapAttribute).Assembly)
                                 .Where(a => !a.IsDynamic)
                                 .SelectMany(a => a.DefinedTypes);

            var importConfig = serviceProvider.GetRequiredService<IOptions<ImportSettings>>();

            foreach (var viewModels in allTypes)
            {
                foreach (var attribute in viewModels.GetCustomAttributes<YuzuMapAttribute>())
                {
                    var cmsModel = config.CMSModels.FirstOrDefault(x => x.Name == attribute.SourceTypeName);

                    if (cmsModel == null)
                    {
                        continue;
                    }

                    if (importConfig.Value.IgnoreUmbracoModelsForAutomap.Contains(cmsModel.Name))
                    {
                        continue;
                    }

                    if(mapContext.Has(cmsModel, viewModels))
                    {
                        continue;
                    }

                    cfg.CreateMap(cmsModel, viewModels);
                }
            }
        }
    }

}
