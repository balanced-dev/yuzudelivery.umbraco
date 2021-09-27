using System;
using System.Linq;
using System.Collections.Generic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
#endif

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
                        Mapper = typeof(IYuzuTypeConvertorMapper),
                        Convertor = convertorType
                    });
                }
            }
        }
    }
}