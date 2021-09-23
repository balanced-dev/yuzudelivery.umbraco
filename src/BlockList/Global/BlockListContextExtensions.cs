using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Web.PropertyEditors;
using Umbraco.Core.Models.PublishedContent;
using Umb = Umbraco.Core.Services;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core;
using Umbraco.Core.Logging;

namespace YuzuDelivery.Umbraco.Core
{
    public static class BlockListContextExtensions
    {
        
        public static bool IsInPreview(this UmbracoMappingContext context)
        {
            return context.Items.ContainsKey(_BlockList_Constants.IsInPreview) && context.Items[_BlockList_Constants.IsInPreview].ToBool();
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
