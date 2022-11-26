using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.BlockList;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListGridAutoMapping : YuzuMappingConfig
    {
        public BlockListGridAutoMapping()
        {
            ManualMaps.AddTypeReplace<BlockListRowsConverter>();
            ManualMaps.AddTypeReplace<BlockListGridConverter>();
        }
    }

}