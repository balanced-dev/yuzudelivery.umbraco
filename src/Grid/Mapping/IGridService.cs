using AutoMapper;
using Skybrud.Umbraco.GridData;

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IGridService
    {
        object Content(GridItemData data);
        vmBlock_DataRows CreateRows(GridDataModel grid, ResolutionContext context);
        vmBlock_DataGrid CreateRowsColumns(GridDataModel grid, ResolutionContext context);
        vmBlock_DataGrid CreateRowsColumns<TConfig>(GridDataModel grid, ResolutionContext context);
        vmBlock_DataGrid CreateRowsColumns<TConfigRow, TConfigCol>(GridDataModel grid, ResolutionContext context);
        vmBlock_DataRows CreateRows<TConfig>(GridDataModel grid, ResolutionContext context);
    }
}