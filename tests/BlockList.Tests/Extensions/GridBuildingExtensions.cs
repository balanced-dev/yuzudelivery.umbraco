using System.Collections.Generic;
using YuzuDelivery.Umbraco.BlockList.Tests.Grid.ContentCreation;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.BlockList.Tests.Extensions;

public static class GridBuildingExtensions
{
    public static vmSub_DataGridRow AddRow(this vmBlock_DataGrid grid)
    {
        var row = new vmSub_DataGridRow();
        grid.Rows.Add(row);
        return row;
    }

    public static vmSub_DataGridRow WithSettings(this vmSub_DataGridRow row, int id)
    {
        row.Config = new BlockGridContentMapperTests.vmBlock_SettingsBlock
        {
            Id = id.ToString()
        };
        return row;
    }

    public static vmSub_DataGridColumn AddCol(this vmSub_DataGridRow row)
    {
        var col = new vmSub_DataGridColumn();
        row.Columns.Add(col);
        return col;
    }

    public static vmSub_DataGridColumn WithSettings(this vmSub_DataGridColumn col, int id)
    {
        col.Config = new BlockGridContentMapperTests.vmBlock_SettingsBlock
        {
            Id = id.ToString()
        };
        return col;
    }

    public static vmSub_DataGridColumn WithSize(this vmSub_DataGridColumn col, int size)
    {
        col.GridSize = size;
        return col;
    }

    public static vmSub_DataGridColumn AddItem(this vmSub_DataGridColumn col, string title)
    {
        var rowItem = new vmBlock_DataGridRowItem
        {
            Content = new BlockGridContentMapperTests.vmBlock_ContentBlock
            {
                Title = title
            }
        };
        var item = rowItem;
        col.Items.Add(item);
        return col;
    }

    public static VmToContentPropertyLink PropertyLinkForEditor(int editorId)
    {
        return new VmToContentPropertyLink
        {
            CmsPropertyType = new InternalPropertyType
            {
                DataTypeId = editorId
            }
        };
    }
}
