using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;
using YuzuDelivery.Core.AutoMapper.Mappers;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public class SubBlocksMappings : YuzuMappingConfig
    {
        public SubBlocksMappings(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
        {
            var listOfObjectsMappings = vmPropertyMappingsFinder.GetMappings<object>();

            foreach (var i in listOfObjectsMappings)
            {
                if (i.SourceType != null && i.SourceProperty != null && i.SourceProperty.PropertyType == typeof(IPublishedContent))
                {
                    var resolverType = typeof(SubBlocksObjectResolver<,>).MakeGenericType(i.SourceType, i.DestType);

                    ManualMaps.Add(new YuzuFullPropertyMapperSettings()
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
        private readonly IYuzuConfiguration config;

        public SubBlocksObjectResolver(IMapper mapper, IYuzuConfiguration config)
        {
            this.mapper = mapper;
            this.config = config;
        }

        public object Resolve(Source source, Dest destination, IPublishedContent sourceMember, string destPropertyName, UmbracoMappingContext context)
        {
            var cmsModel = config.CMSModels.Where(c => c.Name == sourceMember.ContentType.Alias.FirstCharacterToUpper()).FirstOrDefault();
            var viewmodel = config.ViewModels.Where(x => x.Name == $"{YuzuConstants.Configuration.BlockPrefix}{cmsModel.Name}" || x.Name == $"{YuzuConstants.Configuration.PagePrefix}{cmsModel.Name}").FirstOrDefault();

            return mapper.Map(sourceMember, cmsModel, viewmodel, context.Items);
        }
    }
}
