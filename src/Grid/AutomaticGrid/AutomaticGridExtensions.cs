using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData.Dtge;

#if NETCOREAPP
using Skybrud.Umbraco.GridData.Models;
#else
using Skybrud.Umbraco.GridData;
#endif

namespace YuzuDelivery.Umbraco.Grid
{
    public static class AutomaticGridExtensions
    {

        public static object Setting(this Dictionary<string, object> rowConfig, Dictionary<string, object> areaSettings, string propertyName)
        {
            var row = rowConfig.Setting(propertyName);
            var area = areaSettings.Setting(propertyName);

            if (!string.IsNullOrEmpty(area.ToString()))
                return area;
            return row;
        }

        public static object Setting(this Dictionary<string, object> values, string propertyName)
        {
            if (values != null && values.ContainsKey(propertyName))
            {
                return values[propertyName];
            }
            return string.Empty;
        }

        public static int CurrentIndex(this GridArea[] areas, GridArea currentArea)
        {
            return areas.ToList().FindIndex(x => x == currentArea);
        }

        public static List<GridArea> OtherAreas(this GridRow row, GridArea currentArea)
        {
            return row.Areas.Except(new List<GridArea>() { currentArea }).ToList();
        }

        public static bool HasEditor(this GridArea area, string editorName)
        {
            return area.Controls.Any(y => y.IsEditor(editorName));
        }

        public static bool IsEditor(this GridControl control, string editorName)
        {
            return control.Editor.Alias == editorName;
        }

        public static bool HasContentType(this GridArea area, string contentTypeAlias)
        {
            return area.Controls.Any(y => y.IsContentType(contentTypeAlias));
        }

        public static bool IsContentType(this GridControl control, string contentTypeAlias)
        {
            return control.GetValue<GridControlDtgeValue>()?.Content?.ContentType?.Alias == contentTypeAlias;
        }

    }
}
