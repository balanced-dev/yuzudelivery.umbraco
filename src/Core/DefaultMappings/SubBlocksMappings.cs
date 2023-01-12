using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Cms.Core.Models.PublishedContent;


namespace YuzuDelivery.Umbraco.Core
{
    public class SubBlocksMappings : IConfigureOptions<ManualMapping>
    {
        private readonly IVmPropertyMappingsFinder vmPropertyMappingsFinder;

        public SubBlocksMappings(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
        {
            this.vmPropertyMappingsFinder = vmPropertyMappingsFinder;
        }

        public void Configure(ManualMapping options)
        {
            var listOfObjectsMappings = vmPropertyMappingsFinder.GetMappings<object>();

            foreach (var i in listOfObjectsMappings)
            {
                if (i.SourceType != null && i.SourceProperty != null && i.SourceProperty.PropertyType == typeof(IPublishedContent))
                {
                    var resolverType = typeof(SubBlocksObjectResolver<,>).MakeGenericType(i.SourceType, i.DestType);

                    options.ManualMaps.Add(new YuzuFullPropertyMapperSettings()
                    {
                        Mapper = typeof(IYuzuFullPropertyMapper<UmbracoMappingContext>),
                        Resolver = resolverType,
                        SourcePropertyName = i.SourcePropertyName,
                        DestPropertyName = i.DestPropertyName
                    });
                }
            }
        }
    }

    public class SubBlocksObjectResolver<Source, Dest> : IYuzuFullPropertyResolver<Source, Dest, IPublishedContent, object>
    {
        private readonly IMapper mapper;
        private readonly IOptions<YuzuConfiguration> config;

        public SubBlocksObjectResolver(IMapper mapper, IOptions<YuzuConfiguration> config)
        {
            this.mapper = mapper;
            this.config = config;
        }

        public object Resolve(Source source, Dest destination, IPublishedContent sourceMember, string destPropertyName, UmbracoMappingContext context)
        {
            var cmsModel = config.Value.CMSModels.FirstOrDefault(c => c.Name == sourceMember.ContentType.Alias.FirstCharacterToUpper());
            var viewmodel = config.Value.ViewModels.FirstOrDefault(x => x.Name == $"{YuzuConstants.Configuration.BlockPrefix}{cmsModel.Name}" || x.Name == $"{YuzuConstants.Configuration.PagePrefix}{cmsModel.Name}");

            return mapper.Map(sourceMember, cmsModel, viewmodel, context.Items);
        }
    }

}
