using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Web;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    [PluginController("YuzuDeliveryUmbraco")]
    [IsBackOffice]
    public class ViewModelBuilderApiController : UmbracoApiController
    {
        [System.Web.Http.HttpGet]
        [ApiBasicAuthFilter("settings")]
        public HttpResponseMessage CurrentSetup()
        {
            var data = new CurrentSetup()
            {
                PageLocation = Yuzu.Configuration.TemplateLocations.Where(x => x.Name == "Pages").Select(x => x.Schema).FirstOrDefault(),
                BlockLocation = Yuzu.Configuration.TemplateLocations.Where(x => x.Name == "Partials").Select(x => x.Schema).FirstOrDefault(),
                ExcludedViewModels = Yuzu.Configuration.ExcludeViewmodelsAtGeneration.ToArray(),
                ViewmodelNamespaces  = Yuzu.Configuration.AddNamespacesAtGeneration.ToArray()
            };
            return Request.CreateResponse<CurrentSetup>(HttpStatusCode.OK, data, Configuration.Formatters.JsonFormatter);
        }
    }

    public class CurrentSetup
    {
        public string PageLocation { get; set; }
        public string BlockLocation { get; set; }
        public string[] ExcludedViewModels { get; set; }
        public string[] ViewmodelNamespaces { get; set; }
    }

}
