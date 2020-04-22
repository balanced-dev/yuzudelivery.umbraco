using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Forms.Web.Models;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormMappingConfig : YuzuMappingConfig
    {
        public FormMappingConfig()
        {
            ManualMaps.AddTypeReplace<FormTypeConvertor>();
            ManualMaps.AddTypeReplace<FormBuilderTypeConverter>();
        }
    }
}
