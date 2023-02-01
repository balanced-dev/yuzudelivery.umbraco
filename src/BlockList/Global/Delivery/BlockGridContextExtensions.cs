using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.BlockList;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core
{
    public static class BlockGridContextExtensions
    {

        public static bool IsInPreview(this UmbracoMappingContext context)
        {
            return context.Items.ContainsKey(_BlockList_Constants.IsInPreview) && context.Items[_BlockList_Constants.IsInPreview].ToBool();
        }

        public static vmSub_DataRowsRow ContextRow(this UmbracoMappingContext context)
        {
            return context.Items[_BlockList_Constants.ContextRow] as vmSub_DataRowsRow;
        }

        public static vmSub_DataGridRow ContextGridRow(this UmbracoMappingContext context)
        {
            return context.Items[_BlockList_Constants.ContextRow] as vmSub_DataGridRow;
        }

        public static vmSub_DataGridColumn ContextColumn(this UmbracoMappingContext context)
        {
            return context.Items[_BlockList_Constants.ContextColumn] as vmSub_DataGridColumn;
        }

        public static T RowSettings<T>(this UmbracoMappingContext context)
            where T : class
        {
            return GetSettings<T>(context, _BlockList_Constants.RowSettings);
        }

        public static T ColumnsSettings<T>(this UmbracoMappingContext context)
            where T : class
        {
            return GetSettings<T>(context, _BlockList_Constants.ColumnSettings);
        }

        public static T ContentSettings<T>(this UmbracoMappingContext context)
            where T : class
        {
            return GetSettings<T>(context, _BlockList_Constants.ContentSettings);
        }

        public static T GetSettings<T>(UmbracoMappingContext context, string name)
            where T : class
        {
            return context.Items.ContainsKey(name) ? context.Items[name] as T : null;
        }

    }
}
