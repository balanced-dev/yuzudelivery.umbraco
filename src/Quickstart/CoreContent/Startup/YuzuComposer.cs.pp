using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Grid;
using YuzuDelivery.Umbraco.Forms;
using YuzuDelivery.Umbraco.Members;
using Umbraco.Core.Models.PublishedContent;

namespace $rootnamespace$
{

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeBefore(typeof(YuzuStartup))]
    [ComposeBefore(typeof(YuzuGridStartup))]
    [ComposeBefore(typeof(YuzuFormsStartup))]
    [ComposeBefore(typeof(YuzuMembersStartup))]
    public class YuzuComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var Server = HttpContext.Current.Server;

            var pagesLocation = ConfigurationManager.AppSettings["YuzuPages"];
            var partialsLocation = ConfigurationManager.AppSettings["YuzuPartials"];

            var localAssembly = Assembly.GetAssembly(typeof(YuzuImportsComposer));
            var gridAssembly = Assembly.GetAssembly(typeof(YuzuGridStartup));

            Yuzu.Initialize(new YuzuConfiguration()
            {
                ViewModelAssemblies = new Assembly[] { localAssembly, gridAssembly },
                CMSModels = localAssembly.GetTypes().Where(x => x.BaseType == typeof(PublishedElementModel)).ToList(),
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
                },
                SchemaMetaLocations = new List<IDataLocation>()
                {
                    new DataLocation()
                    {
                        Name = "Main",
                        Path = Server.MapPath(ConfigurationManager.AppSettings["YuzuSchemaMeta"])
                    }
                },
                GetTemplatesCache = GetTemplatesCache,
                SetTemplatesCache = SetTemplatesCache,
                GetRenderedHtmlCache = GetRenderedHtmlCache,
                SetRenderedHtmlCache = SetRenderedHtmlCache
            });

            Yuzu.Configuration.AddNamespacesAtGeneration.Add("using YuzuDelivery.UmbracoModels;");

        }

        private Dictionary<string, Func<object, string>> GetTemplatesCache()
        {
            return Current.AppCaches.RuntimeCache.Get("feTemplates") as Dictionary<string, Func<object, string>>;
        }

        private Dictionary<string, Func<object, string>> SetTemplatesCache()
        {
            var templateService = DependencyResolver.Current.GetService<IYuzuDefinitionTemplateSetup>();
            return Current.AppCaches.RuntimeCache.Get("feTemplates", () => templateService.RegisterAll()) as Dictionary<string, Func<object, string>>;
        }

        private string GetRenderedHtmlCache(IRenderSettings settings)
        {
            return Current.AppCaches.RuntimeCache.Get(settings.CacheName) as string;
        }

        private void SetRenderedHtmlCache(IRenderSettings settings, string html)
        {
            Current.AppCaches.RuntimeCache.Insert(settings.CacheName, () => { return html; }, new TimeSpan(0, 0, settings.CacheExpiry));
        }
    }

}
