using System.Linq;
using System;
using System.Collections.Generic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{

    /// <summary>
    /// Adds manual mapping actions added in Yuzu Delivery import admin
    /// </summary>
    public class ManualMappingsMappings : YuzuMappingConfig
    {
        public ManualMappingsMappings(ICustomManualMappersService manualMappersConfigService, IYuzuConfiguration config, IVmHelperService vmHelper)
        {
            foreach(var m in manualMappersConfigService.Mappers)
            {
                var manualMap = config.InstalledManualMaps.Where(x => x.Concrete.Name == m.Mapper).FirstOrDefault();
                var link = vmHelper.Get(m.Dest);

                if(manualMap != null)
                {
                    if (manualMap.Concrete.HasInterface<IYuzuTypeAfterConvertor>())
                        ManualMaps.AddTypeAfterMap(manualMap.Concrete);

                    if (manualMap.Concrete.HasInterface<IYuzuTypeConvertor>())
                        ManualMaps.AddTypeReplace(manualMap.Concrete);

                    //Can't do this yet, automapper AddTransofrm bug
                    //if (manualMap.Concrete.HasInterface<IYuzuPropertyAfterResolver>())
                    //    ManualMaps.AddPropertyAfter(manualMap.Concrete, destVm, m.DestMember, m.Group);

                    if (manualMap.Concrete.HasInterface<IYuzuPropertyReplaceResolver>())
                        ManualMaps.AddPropertyReplace(manualMap.Concrete, link.Viewmodel, m.DestMember, m.Group);

                    if (manualMap.Concrete.HasInterface<IYuzuTypeFactory>() && !string.IsNullOrEmpty(m.DestMember))
                        ManualMaps.AddPropertyFactory(manualMap.Concrete, link.CMSModel, link.Viewmodel, m.DestMember);

                    if (manualMap.Concrete.HasInterface<IYuzuTypeFactory>() && string.IsNullOrEmpty(m.DestMember))
                        ManualMaps.AddTypeFactory(manualMap.Concrete, link.Viewmodel);
                }
            }
        }
    }

    public static class ReflectionExtensions
    {
        public static bool HasInterface<I>(this Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(I));
        }
    }

}

