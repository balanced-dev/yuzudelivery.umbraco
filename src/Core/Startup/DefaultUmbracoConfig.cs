using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using System.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Extensions;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Umbraco.Core.Settings;
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
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IOptionsMonitor<CoreSettings> _coreSettings;
        private readonly IAppPolicyCache _appPolicyCache;
        private readonly Lazy<IYuzuDefinitionTemplateSetup> _templateSetup; // TODO: There's a circular dependency here, YuzuDefinitionTemplateSetup depends on IYuzuConfiguration.

        public DefaultUmbracoConfig(
            IHostEnvironment hostEnvironment,
            IOptionsMonitor<CoreSettings> coreSettings,
            IOptionsMonitor<VmGenerationSettings> vmGenerationSettings,
            IEnumerable<IUpdateableConfig> updateableConfigs,
            IAppPolicyCache appPolicyCache,
            Lazy<IYuzuDefinitionTemplateSetup> templateSetup,
            Assembly localAssembly)
            : base(updateableConfigs)
        {
            _hostEnvironment = hostEnvironment;
            _coreSettings = coreSettings;
            _appPolicyCache = appPolicyCache;
            _templateSetup = templateSetup;

            var pagesLocation = coreSettings.CurrentValue.Pages;
            var partialsLocation = coreSettings.CurrentValue.Partials;

            ViewModelAssemblies = new Assembly[] { localAssembly };

            CMSModels = localAssembly.GetTypes().Where(x => x.GetCustomAttribute<PublishedModelAttribute>() != null).ToList();

            //add compositions;
            CMSModels = CMSModels.Union(CMSModels.SelectMany(x => x.GetInterfaces()).ToList());

            TemplateLocations = new List<ITemplateLocation>()
                {
                    new TemplateLocation()
                    {
                        Name = "Pages",
                        Path = _hostEnvironment.MapPathContentRoot(pagesLocation),
                        Schema = _hostEnvironment.MapPathContentRoot(pagesLocation.Replace("src", "schema")),
                        RegisterAllAsPartials = false,
                        SearchSubDirectories = false
                    },
                    new TemplateLocation()
                    {
                        Name = "Partials",
                        Path = _hostEnvironment.MapPathContentRoot(partialsLocation),
                        Schema = _hostEnvironment.MapPathContentRoot(partialsLocation.Replace("src", "schema")),
                        RegisterAllAsPartials = true,
                        SearchSubDirectories = true
                    }
                };

            GetTemplatesCache = () => {
                return _appPolicyCache.Get("feTemplates") as Dictionary<string, Func<object, string>>;
            };

            SetTemplatesCache = () => {
                return _appPolicyCache.Get("feTemplates", () => _templateSetup.Value.RegisterAll()) as Dictionary<string, Func<object, string>>;
            };

            GetRenderedHtmlCache = (IRenderSettings renderSettings) => {
                return _appPolicyCache.Get(renderSettings.CacheName) as string;
            };

            SetRenderedHtmlCache = (IRenderSettings renderSettings, string html) => {
                _appPolicyCache.Insert(renderSettings.CacheName, () => { return html; }, new TimeSpan(0, 0, renderSettings.CacheExpiry));
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
