using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Umbraco.Cms.Core.Models.Blocks;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;


namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListItemAutoMapping : IConfigureOptions<ManualMapping>
    {
        private readonly IVmPropertyMappingsFinder vmPropertyMappingsFinder;

        public BlockListItemAutoMapping(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
        {
            this.vmPropertyMappingsFinder = vmPropertyMappingsFinder;
        }

        public void Configure(ManualMapping options)
        {
            var mappings = vmPropertyMappingsFinder.GetBlockMappings(typeof(BlockListItem<>));

            foreach (var i in mappings)
            {
                if (i.SourceType != null)
                {
                    var sourcePropertyType = i.SourceProperty.PropertyType;
                    var genericType = sourcePropertyType.GetGenericArguments().First();
                    var destPropertyType = i.DestProperty.PropertyType;

                    Type convertorType = typeof(BlockListItemToTypeConvertor<,>).MakeGenericType(genericType, destPropertyType);

                    options.ManualMaps.Add(new YuzuTypeConvertorMapperSettings()
                    {
                        Mapper = typeof(IYuzuTypeReplaceMapper<UmbracoMappingContext>),
                        Convertor = convertorType
                    });
                }
            }
        }
    }
}
