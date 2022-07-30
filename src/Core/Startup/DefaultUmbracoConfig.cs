using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using System.Configuration;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Cache;
#else
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Cache;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultUmbracoConfig : YuzuConfiguration
    {
        public DefaultUmbracoConfig(IFactory factory, Assembly localAssembly)
            : base(factory.GetAllInstances<IUpdateableConfig>())
        {
            var mappath = factory.GetInstance<MapPathAbstraction>();
            var settings = factory.GetInstance<SettingsAbstraction>();

            var pagesLocation = settings.Pages;
            var partialsLocation = settings.Partials;

            ViewModelAssemblies = new Assembly[] { localAssembly };

            CMSModels = localAssembly.GetTypes().Where(x => x.GetCustomAttribute<PublishedModelAttribute>() != null).ToList();

            //add compositions;
            CMSModels = CMSModels.Union(CMSModels.SelectMany(x => x.GetInterfaces()).ToList());

            TemplateLocations = new List<ITemplateLocation>()
                {
                    new TemplateLocation()
                    {
                        Name = "Pages",
                        Path = mappath.Get(pagesLocation),
                        Schema = mappath.Get(pagesLocation.Replace("src", "schema")),
                        RegisterAllAsPartials = false,
                        SearchSubDirectories = false
                    },
                    new TemplateLocation()
                    {
                        Name = "Partials",
                        Path = mappath.Get(partialsLocation),
                        Schema = mappath.Get(partialsLocation.Replace("src", "schema")),
                        RegisterAllAsPartials = true,
                        SearchSubDirectories = true
                    }
                };

            SchemaMetaLocations = new List<IDataLocation>()
                {
                    new DataLocation()
                    {
                        Name = "Main",
                        Path = mappath.Get(settings.SchemaMeta)
                    }
                };

            GetTemplatesCache = () => {
                var cache = factory.GetInstance<IAppPolicyCache>();
                return cache.Get("feTemplates") as Dictionary<string, Func<object, string>>;
            };

            SetTemplatesCache = () => {
                var cache = factory.GetInstance<IAppPolicyCache>();
                var templateService = factory.GetInstance<IYuzuDefinitionTemplateSetup>();
                return cache.Get("feTemplates", () => templateService.RegisterAll()) as Dictionary<string, Func<object, string>>;
            };

            GetRenderedHtmlCache = (IRenderSettings renderSettings) => {
                var cache = factory.GetInstance<IAppPolicyCache>();
                return cache.Get(renderSettings.CacheName) as string;
            };

            SetRenderedHtmlCache = (IRenderSettings renderSettings, string html) => {
                var cache = factory.GetInstance<IAppPolicyCache>();
                cache.Insert(renderSettings.CacheName, () => { return html; }, new TimeSpan(0, 0, renderSettings.CacheExpiry));
            };

            foreach (var asm in ViewModelAssemblies)
            {
                AddInstalledManualMap<IYuzuTypeAfterConvertor>(asm, false, false);
                AddInstalledManualMap<IYuzuTypeConvertor>(asm, false, false);
                //Can't do this yet, automapper AddTransofrm bug
                //AddInstalledManualMap<IYuzuPropertyAfterResolver>(asm, true);
                AddInstalledManualMap<IYuzuPropertyReplaceResolver>(asm, true, false);
                AddInstalledManualMap<IYuzuTypeFactory>(asm, false, true);
            }
        }

        public void AddInstalledManualMap<I>(Assembly asm, bool isMember, bool isFactory)
        {
            var propertyResolvers = asm.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(I)));
            foreach (var p in propertyResolvers)
            {
                var d = p.GetInterfaces().FirstOrDefault().GetGenericArguments();

                if(isMember)
                    InstalledManualMaps.Add(new ManualMapInstalledType() { Interface = typeof(I), Concrete = p, SourceType = d[0], DestMemberType = d[1] });
                if (isFactory)
                    InstalledManualMaps.Add(new ManualMapInstalledType() { Interface = typeof(I), Concrete = p, DestType = d[0] });
                else
                    InstalledManualMaps.Add(new ManualMapInstalledType() { Interface = typeof(I), Concrete = p, SourceType = d[0], DestType = d[1] });
            }
        }

    }
}
