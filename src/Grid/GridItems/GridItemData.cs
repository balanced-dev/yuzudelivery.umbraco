using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skybrud.Umbraco.GridData;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridItemData
    {
        public GridControl Control { get; set; }
        public IDictionary<string, object> ContextItems { get; set; }
        public Dictionary<string, object> RowConfig { get; set; }
        public Dictionary<string, object> AreaSettings { get; set; }
    }
}
