using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Umbraco.Forms
{
    public class YuzuFormViewModel
    {
        public YuzuFormViewModel()
        {
            HtmlFormAttributes = new Dictionary<string, object>();
            HtmlFormAttributes.Add("novalidate", "");
            Template = "parFormBuilder";
            AddAntiForgeryToken = true;
        }

        public Dictionary<string, object> HtmlFormAttributes { get; set; }
        public Type Controller { get; set; }
        public string Action { get; set; }
        public string Template { get; set; }
        public vmBlock_DataFormBuilder Form { get; set; }
        public bool AddAntiForgeryToken { get; set; }
    }
}
