using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace YuzuDelivery.Umbraco.Forms
{
    public class YuzuDeliveryFormsConfiguration : IYuzuDeliveryFormsConfiguration
    {
        public YuzuDeliveryFormsConfiguration()
        {
            FormElementAssemblies = new Assembly[] { };
        }

        public Assembly[] FormElementAssemblies { get; set; }
    }
}
