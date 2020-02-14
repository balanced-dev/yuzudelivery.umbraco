using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using System.Configuration;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;
using System.Web.Mvc;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultUmbracoConfig : YuzuConfiguration
    {
        public DefaultUmbracoConfig(IFactory factory, Assembly localAssembly)
            : base(factory.GetAllInstances<IUpdateableConfig>())
        {
            var Server = HttpContext.Current.Server;

            var pagesLocation = ConfigurationManager.AppSettings["YuzuPages"];
            var partialsLocation = ConfigurationManager.AppSettings["YuzuPartials"];

            ViewModelAssemblies = new Assembly[] { localAssembly };

            CMSModels = localAssembly.GetTypes().Where(x => x.BaseType == typeof(PublishedElementModel) || x.BaseType == typeof(PublishedContentModel)).ToList();

            TemplateLocations = new List<ITemplateLocation>()
                {
                    new TemplateLocation()
                    {
                        Name = "Pages",
                        Path = Server.MapPath(pagesLocation),
                        Schema = Server.MapPath(pagesLocation.Replace("src", "schema")),
                        RegisterAllAsPartials = false,
                        SearchSubDirectories = false
                    },
                    new TemplateLocation()
                    {
                        Name = "Partials",
                        Path = Server.MapPath(partialsLocation),
                        Schema = Server.MapPath(partialsLocation.Replace("src", "schema")),
                        RegisterAllAsPartials = true,
                        SearchSubDirectories = true
                    }
                };

            SchemaMetaLocations = new List<IDataLocation>()
                {
                    new DataLocation()
                    {
                        Name = "Main",
                        Path = Server.MapPath(ConfigurationManager.AppSettings["YuzuSchemaMeta"])
                    }
                };

            GetTemplatesCache = () => {
                return Current.AppCaches.RuntimeCache.Get("feTemplates") as Dictionary<string, Func<object, string>>;
            };

            SetTemplatesCache = () => {
                var templateService = DependencyResolver.Current.GetService<IYuzuDefinitionTemplateSetup>();
                return Current.AppCaches.RuntimeCache.Get("feTemplates", () => templateService.RegisterAll()) as Dictionary<string, Func<object, string>>;
            };

            GetRenderedHtmlCache = (IRenderSettings settings) => {
                return Current.AppCaches.RuntimeCache.Get(settings.CacheName) as string;
            };

            SetRenderedHtmlCache = (IRenderSettings settings, string html) => {
                Current.AppCaches.RuntimeCache.Insert(settings.CacheName, () => { return html; }, new TimeSpan(0, 0, settings.CacheExpiry));
            };
        }
    }
}
