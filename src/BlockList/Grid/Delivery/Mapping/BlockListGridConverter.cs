using YuzuDelivery.Umbraco.Core;
using Umbraco.Core.Models.Blocks;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListGridConverter : IYuzuTypeConvertor<BlockListModel, vmBlock_DataGrid>
    {
        private readonly BlockListGridDataService gridService;

        public BlockListGridConverter(BlockListGridDataService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGrid Convert(BlockListModel source, UmbracoMappingContext context)
        {
            return gridService.CreateRowsColumns(source, context);
        }
    }

}