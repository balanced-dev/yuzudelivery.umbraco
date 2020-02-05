using AutoMapper;
using Skybrud.Umbraco.GridData;

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IGridService
    {
        object Content(GridItemData data);
        vmBlock_DataGridRows CreateRows(GridDataModel grid, ResolutionContext context);
        vmBlock_DataGridRowsColumns CreateRowsColumns(GridDataModel grid, ResolutionContext context);
        vmBlock_DataGridRowsColumns CreateRowsColumns<TConfig>(GridDataModel grid, ResolutionContext context);
        vmBlock_DataGridRowsColumns CreateRowsColumns<TConfigRow, TConfigCol>(GridDataModel grid, ResolutionContext context);
        vmBlock_DataGridRows CreateRows<TConfig>(GridDataModel grid, ResolutionContext context);
    }
}