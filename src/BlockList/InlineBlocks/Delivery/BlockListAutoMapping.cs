using System;
using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Cms.Core.Models.Blocks;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;


namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListAutoMapping : IConfigureOptions<ManualMapping>
    {
        private readonly IVmPropertyMappingsFinder vmPropertyMappingsFinder;

        public BlockListAutoMapping(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
        {
            this.vmPropertyMappingsFinder = vmPropertyMappingsFinder;
        }

        public void Configure(ManualMapping options)
        {
            var mappings = vmPropertyMappingsFinder.GetBlockMappings(typeof(BlockListModel));

            foreach (var i in mappings)
            {
                if (i.SourceType != null)
                {
                    var destPropertyType = i.DestProperty.PropertyType;
                    var generics = destPropertyType.GetGenericArguments();

                    Type convertorType = null;
                    if (generics.Any())
                        convertorType = typeof(BlockListToListOfTypesConvertor<>).MakeGenericType(generics.FirstOrDefault());
                    else
                        convertorType = typeof(BlockListToTypeConvertor<>).MakeGenericType(destPropertyType);

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
