using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.BlockList;

namespace Yuzy.BlockListEditor
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