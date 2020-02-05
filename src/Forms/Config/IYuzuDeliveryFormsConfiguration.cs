using System;
using System.Collections.Generic;
using System.Reflection;

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IYuzuDeliveryFormsConfiguration
    {
        Assembly[] FormElementAssemblies { get; set; }
    }
}