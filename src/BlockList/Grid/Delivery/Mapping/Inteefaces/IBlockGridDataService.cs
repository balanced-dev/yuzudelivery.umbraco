using System.Collections.Generic;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web;

namespace YuzuDelivery.Umbraco.BlockList
{
    public interface IBlockGridDataService
    {
        vmBlock_DataRows CreateRows(BlockGridModel grid, UmbracoMappingContext context);
        vmBlock_DataGrid CreateRowsColumns(BlockGridModel grid, UmbracoMappingContext context);
        object CreateVm(IPublishedElement model, IDictionary<string, object> context);
    }
}
