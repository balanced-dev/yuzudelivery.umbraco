using System;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;
using YuzuDelivery.Core.AutoMapper.Mappers;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Cms.Core.Models.Blocks;


namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListAutoMapping : YuzuMappingConfig
    {
        public BlockListAutoMapping(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
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

                    ManualMaps.Add(new YuzuTypeConvertorMapperSettings()
                    {
                        Mapper = typeof(IYuzuTypeConvertorMapper<UmbracoMappingContext>),
                        Convertor = convertorType
                    });
                }
            }
        }
    }
}
