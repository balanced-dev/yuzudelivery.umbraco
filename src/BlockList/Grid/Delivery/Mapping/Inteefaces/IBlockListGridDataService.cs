using System.Collections.Generic;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;

#if NETCOREAPP
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web;
#else
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    public interface IBlockListGridDataService
    {
        vmBlock_DataGridRowItem CreateContentAndConfig(GridItemData data);
        vmBlock_DataRows CreateRows(BlockListModel grid, UmbracoMappingContext context);
        vmBlock_DataGrid CreateRowsColumns(BlockListModel grid, UmbracoMappingContext context);
        object CreateVm(IPublishedElement model, IDictionary<string, object> context);
        object GetContentSettingsVm(GridItemData data);
    }
}