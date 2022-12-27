using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Extensions;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Settings;
using Umbraco.Cms.Core.Models.PublishedContent;


namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultUmbracoConfig : YuzuConfiguration
    {
        private readonly IHostEnvironment _hostEnvironment;

        public DefaultUmbracoConfig(
            IHostEnvironment hostEnvironment,
            IOptionsMonitor<CoreSettings> coreSettings,
            IEnumerable<IUpdateableConfig> updateableConfigs,
            IEnumerable<IBaseSiteConfig> baseSiteConfigs = null,
            Assembly assembly = null)
            : base(updateableConfigs)
        {
            _hostEnvironment = hostEnvironment;

            AddToModelRegistry(assembly);
            AddInstalledManualMaps(assembly);

            foreach(var childSiteConfig in baseSiteConfigs)
            {
                childSiteConfig.Setup(this);
            }
        }

        public void AddToModelRegistry(Assembly assemby)
        {
            ViewModelAssemblies.Add(assemby);

            var concreteModels = assemby.GetTypes().Where(x => x.GetCustomAttribute<PublishedModelAttribute>() != null);
            CMSModels.AddRange(concreteModels);
            //add compositions;
            CMSModels.AddRange(concreteModels.SelectMany(x => x.GetInterfaces()));

            ViewModels.AddRange(assemby.GetTypes().Where(y => y.Name.IsVm()));

            MappingAssemblies.Add(assemby);
        }

        public void AddInstalledManualMaps(Assembly assembly)
        {
            AddInstalledManualMap<IYuzuTypeAfterConvertor>(assembly, false, false);
            AddInstalledManualMap<IYuzuTypeConvertor>(assembly, false, false);
            //Can't do this yet, automapper AddTransofrm bug
            //AddInstalledManualMap<IYuzuPropertyAfterResolver>(asm, true);
            AddInstalledManualMap<IYuzuPropertyReplaceResolver>(assembly, true, false);
            AddInstalledManualMap<IYuzuTypeFactory>(assembly, false, true);
        }

        private void AddInstalledManualMap<I>(Assembly asm, bool isMember, bool isFactory)
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
