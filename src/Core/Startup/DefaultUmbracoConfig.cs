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
    public static class YuzuConfigurationExtensions
    {
        private static void AddRange<T>(this IList<T> t, IEnumerable<T> items)
        {
            if (t is List<T> c)
            {
                c.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    t.Add(item);
                }
            }
        }

        public static void AddToModelRegistry(this YuzuConfiguration cfg, Assembly assemby)
        {
            cfg.ViewModelAssemblies.Add(assemby);

            var concreteModels = assemby.GetTypes().Where(x => x.GetCustomAttribute<PublishedModelAttribute>() != null);
            cfg.CMSModels.AddRange(concreteModels);
            //add compositions;
            cfg.CMSModels.AddRange(concreteModels.SelectMany(x => x.GetInterfaces()));

            cfg.ViewModels.AddRange(assemby.GetTypes().Where(y => y.Name.IsVm()));

            cfg.MappingAssemblies.Add(assemby);
        }

        public static void AddInstalledManualMaps(this YuzuConfiguration cfg, Assembly assembly)
        {
            cfg.AddInstalledManualMap<IYuzuTypeAfterConvertor>(assembly, false, false);
            cfg.AddInstalledManualMap<IYuzuTypeConvertor>(assembly, false, false);
            //Can't do this yet, automapper AddTransofrm bug
            //AddInstalledManualMap<IYuzuPropertyAfterResolver>(asm, true);
            cfg.AddInstalledManualMap<IYuzuPropertyReplaceResolver>(assembly, true, false);
            cfg.AddInstalledManualMap<IYuzuTypeFactory>(assembly, false, true);
        }

        private static void AddInstalledManualMap<I>(this YuzuConfiguration cfg, Assembly asm, bool isMember, bool isFactory)
        {
            var propertyResolvers = asm.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(I)));
            foreach (var p in propertyResolvers)
            {
                var d = p.GetInterfaces().FirstOrDefault().GetGenericArguments();

                if(isMember)
                    cfg.InstalledManualMaps.Add(new ManualMapInstalledType() { Interface = typeof(I), Concrete = p, SourceType = d[0], DestMemberType = d[1] });
                if (isFactory)
                    cfg.InstalledManualMaps.Add(new ManualMapInstalledType() { Interface = typeof(I), Concrete = p, DestType = d[0] });
                else
                    cfg.InstalledManualMaps.Add(new ManualMapInstalledType() { Interface = typeof(I), Concrete = p, SourceType = d[0], DestType = d[1] });
            }
        }

    }
}
