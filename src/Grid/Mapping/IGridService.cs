using YuzuDelivery.Umbraco.Core;
using Skybrud.Umbraco.GridData.Models;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IGridService
    {
        object Content(GridItemData data);
        vmBlock_DataRows CreateRows(GridDataModel grid, UmbracoMappingContext context);
        vmBlock_DataGrid CreateRowsColumns(GridDataModel grid, UmbracoMappingContext context);
        vmBlock_DataGrid CreateRowsColumns<TConfig>(GridDataModel grid, UmbracoMappingContext context);
        vmBlock_DataGrid CreateRowsColumns<TConfigRow, TConfigCol>(GridDataModel grid, UmbracoMappingContext context);
        vmBlock_DataRows CreateRows<TConfig>(GridDataModel grid, UmbracoMappingContext context);
    }
}