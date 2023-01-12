using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{
    public class GlobalConfigMappings : IConfigureOptions<ManualMapping>
    {
        private readonly IStoredConfigAsService storedConfigAsService;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly IVmHelperService vmHelperService;

        public GlobalConfigMappings(IStoredConfigAsService storedConfigAsService, IOptions<YuzuConfiguration> config, IVmHelperService vmHelperService)
        {
            this.storedConfigAsService = storedConfigAsService;
            this.config = config;
            this.vmHelperService = vmHelperService;
        }

        public void Configure(ManualMapping options)
        {
            var globalConfigs = storedConfigAsService.GetAll<GlobalStoreContentAs>();

            foreach(var global in globalConfigs)
            {
                var vmName = global.Key;

                var documentTypeAlias = global.Value.DocumentTypeAlias;
                if (string.IsNullOrEmpty(documentTypeAlias))
                    documentTypeAlias = vmHelperService.Get(vmName)?.ContentType.Alias;

                var groupName = global.Value.StoreContentAs.GroupName;

                var sourceType = config.Value.CMSModels.FirstOrDefault(x => x.Name.ToLower() == documentTypeAlias.ToLower());
                var dest = config.Value.ViewModels.FirstOrDefault(x => x.Name == vmName);

                if (sourceType != null && dest != null)
                    options.ManualMaps.AddGlobal(sourceType, dest, groupName);
            }
        }
    }
}
