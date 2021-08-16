using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.BlockList;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListGridMapping : YuzuMappingConfig
    {
        public BlockListGridMapping()
        {
            ManualMaps.AddTypeReplace<BlockListRowsConverter>();
            ManualMaps.AddTypeReplace<BlockListGridConverter>();
        }
    }

}