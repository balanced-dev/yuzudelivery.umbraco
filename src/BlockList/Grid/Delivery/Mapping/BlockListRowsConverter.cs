using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.Blocks;
#else
using Umbraco.Core.Models.Blocks;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListRowsConverter : IYuzuTypeConvertor<BlockGridModel, vmBlock_DataRows>
    {
        private readonly IBlockGridDataService gridService;

        public BlockListRowsConverter(IBlockGridDataService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataRows Convert(BlockGridModel source, UmbracoMappingContext context)
        {
            return gridService.CreateRows(source, context);
        }
    }

}
