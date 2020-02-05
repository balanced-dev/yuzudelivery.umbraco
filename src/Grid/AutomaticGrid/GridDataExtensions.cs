using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData;
using System.Web.Mvc;

namespace YuzuDelivery.Umbraco.Grid
{
    public static class GridDataExtensions
    {

        public static Dictionary<string, object> AddCalculatedAreaSettings(this Dictionary<string, object> settings, GridRow row, GridArea currentArea)
        {
            var automaticGridConfig = DependencyResolver.Current.GetServices<IAutomaticGridConfig>();
            foreach (var i in automaticGridConfig)
            {
                i.AddAreaSettings(settings, row, currentArea);
            }
            return settings;
        }

        public static Dictionary<string, object> AddCalculatedRowConfig(this Dictionary<string, object> settings, GridRow row)
        {
            var automaticGridConfig = DependencyResolver.Current.GetServices<IAutomaticGridConfig>();
            foreach (var i in automaticGridConfig)
            {
                i.AddRowConfig(settings, row);
            }
            return settings;
        }
        public static Dictionary<string, object> ToDictionary(this GridDictionary values, bool hasValues)
        {
            if(values != null && hasValues)
            {
                return values.JObject.ToObject<Dictionary<string, object>>();
            }
            return new Dictionary<string, object>();
        }

    }
}
