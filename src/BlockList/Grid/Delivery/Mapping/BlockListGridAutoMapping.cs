using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.BlockList;

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