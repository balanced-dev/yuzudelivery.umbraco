using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core.ViewModelBuilder;
using System.Web;
using System.Configuration;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultUmbracoVmBuilderConfig : YuzuViewmodelsBuilderConfig
    {
        public DefaultUmbracoVmBuilderConfig(IFactory factory)
            : base(factory.GetAllInstances<IUpdateableVmBuilderConfig>())
        {
            var Server = HttpContext.Current.Server;

            EnableViewmodelsBuilder = ConfigurationManager.AppSettings["YuzuViewmodelBuilderActive"] == "true";
            GeneratedViewmodelsOutputFolder = Server.MapPath("/App_Data/ViewModels");
            AddNamespacesAtGeneration.Add("using YuzuDelivery.UmbracoModels;");
        }
    }
}
