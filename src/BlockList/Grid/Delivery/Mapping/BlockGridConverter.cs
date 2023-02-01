using YuzuDelivery.Umbraco.Core;
using Umbraco.Cms.Core.Models.Blocks;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockGridConverter : IYuzuTypeConvertor<BlockGridModel, vmBlock_DataGrid>
    {
        private readonly IBlockGridDataService gridService;

        public BlockGridConverter(IBlockGridDataService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGrid Convert(BlockGridModel source, UmbracoMappingContext context)
        {
            return gridService.CreateRowsColumns(source, context);
        }
    }

}
