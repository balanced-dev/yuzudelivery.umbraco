using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData;

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IAutomaticGridConfig
    {
        Dictionary<string, object> AddAreaSettings(Dictionary<string, object> settings, GridRow row, GridArea currentArea);
        Dictionary<string, object> AddRowConfig(Dictionary<string, object> settings, GridRow row);
    }
}
