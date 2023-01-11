using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{
    public class GlobalConfigMappings : YuzuMappingConfig
    {
        public GlobalConfigMappings(IStoredConfigAsService storedConfigAsService, IOptions<YuzuConfiguration> config, IVmHelperService vmHelperService)
        {
            var globalConfigs = storedConfigAsService.GetAll<GlobalStoreContentAs>();

            foreach(var global in globalConfigs)
            {
                var vmName = global.Key;

                var documentTypeAlias = global.Value.DocumentTypeAlias;
                if (string.IsNullOrEmpty(documentTypeAlias))
                    documentTypeAlias = vmHelperService.Get(vmName)?.ContentType.Alias;

                var groupName = global.Value.StoreContentAs.GroupName;

                var sourceType = config.Value.CMSModels.Where(x => x.Name.ToLower() == documentTypeAlias.ToLower()).FirstOrDefault();
                var dest = config.Value.ViewModels.Where(x => x.Name == vmName).FirstOrDefault();

                if (sourceType != null && dest != null)
                    ManualMaps.AddGlobal(sourceType, dest, groupName);
            }
        }
    }
}
