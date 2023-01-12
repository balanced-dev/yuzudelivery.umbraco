using System.Linq;
using System;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{

    /// <summary>
    /// Adds manual mapping actions added in Yuzu Delivery import admin
    /// </summary>
    public class ManualMappingsMappings : IConfigureOptions<ManualMapping>
    {
        private readonly ICustomManualMappersService manualMappersConfigService;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly IVmHelperService vmHelper;

        public ManualMappingsMappings(ICustomManualMappersService manualMappersConfigService, IOptions<YuzuConfiguration> config, IVmHelperService vmHelper)
        {
            this.manualMappersConfigService = manualMappersConfigService;
            this.config = config;
            this.vmHelper = vmHelper;
        }

        public void Configure(ManualMapping options)
        {
            foreach(var m in manualMappersConfigService.Mappers)
            {
                var manualMap = config.Value.InstalledManualMaps.FirstOrDefault(x => x.Concrete.Name == m.Mapper);
                var link = vmHelper.Get(m.Dest);

                if(manualMap != null)
                {
                    if (manualMap.Concrete.HasInterface<IYuzuTypeAfterConvertor>())
                        options.ManualMaps.AddTypeAfterMap(manualMap.Concrete);

                    if (manualMap.Concrete.HasInterface<IYuzuTypeConvertor>())
                        options.ManualMaps.AddTypeReplace(manualMap.Concrete);

                    //Can't do this yet, automapper AddTransofrm bug
                    //if (manualMap.Concrete.HasInterface<IYuzuPropertyAfterResolver>())
                    //    ManualMaps.AddPropertyAfter(manualMap.Concrete, destVm, m.DestMember, m.Group);

                    if (manualMap.Concrete.HasInterface<IYuzuPropertyReplaceResolver>())
                        options.ManualMaps.AddPropertyReplace(manualMap.Concrete, link.Viewmodel, m.DestMember, m.Group);

                    if (manualMap.Concrete.HasInterface<IYuzuTypeFactory>() && !string.IsNullOrEmpty(m.DestMember))
                        options.ManualMaps.AddPropertyFactory(manualMap.Concrete, link.CMSModel, link.Viewmodel, m.DestMember);

                    if (manualMap.Concrete.HasInterface<IYuzuTypeFactory>() && string.IsNullOrEmpty(m.DestMember))
                        options.ManualMaps.AddTypeFactory(manualMap.Concrete, link.Viewmodel);
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

