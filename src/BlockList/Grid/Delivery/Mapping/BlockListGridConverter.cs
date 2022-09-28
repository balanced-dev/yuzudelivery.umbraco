using YuzuDelivery.Umbraco.Core;
using Umbraco.Cms.Core.Models.Blocks;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListGridConverter : IYuzuTypeConvertor<BlockListModel, vmBlock_DataGrid>
    {
        private readonly IBlockListGridDataService gridService;

        public BlockListGridConverter(IBlockListGridDataService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGrid Convert(BlockListModel source, UmbracoMappingContext context)
        {
            return gridService.CreateRowsColumns(source, context);
        }
    }

}