using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP
using Skybrud.Umbraco.GridData.Models;
#else
using Skybrud.Umbraco.GridData;
#endif

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IAutomaticGridConfig
    {
        Dictionary<string, object> AddAreaSettings(Dictionary<string, object> settings, GridRow row, GridArea currentArea);
        Dictionary<string, object> AddRowConfig(Dictionary<string, object> settings, GridRow row);
    }
}
