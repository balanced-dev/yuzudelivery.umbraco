using YuzuDelivery.Umbraco.Core;

#if NETCOREAPP
using Skybrud.Umbraco.GridData.Models;
#else
using Skybrud.Umbraco.GridData;
#endif

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