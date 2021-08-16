using YuzuDelivery.Umbraco.Core;
using Umbraco.Core.Models.Blocks;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListRowsConverter : IYuzuTypeConvertor<BlockListModel, vmBlock_DataRows>
    {
        private readonly BlockListGridDataService gridService;

        public BlockListRowsConverter(BlockListGridDataService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataRows Convert(BlockListModel source, UmbracoMappingContext context)
        {
            return gridService.CreateRows(source, context);
        }
    }

}