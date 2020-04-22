using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{
    public class GlobalConfigMappings : YuzuMappingConfig
    {
        public GlobalConfigMappings(IStoredConfigAsService storedConfigAsService, IYuzuConfiguration config, IYuzuDeliveryImportConfiguration importConfig, IVmHelperService vmHelperService)
        {
            var globalConfigs = storedConfigAsService.GetAll<GlobalStoreContentAs>();

            foreach(var global in globalConfigs)
            {
                var vmName = global.Key;

                var documentTypeAlias = global.Value.DocumentTypeAlias;
                if (string.IsNullOrEmpty(documentTypeAlias))
                    documentTypeAlias = vmHelperService.Get(vmName).ContentType.Alias;

                var groupName = global.Value.StoreContentAs.GroupName;
                if (string.IsNullOrEmpty(groupName))
                    groupName = importConfig.DefaultPropertyGroup;


                var sourceType = config.CMSModels.Where(x => x.Name.ToLower() == documentTypeAlias.ToLower()).FirstOrDefault();
                var dest = config.ViewModels.Where(x => x.Name == vmName).FirstOrDefault();

                if (sourceType != null && dest != null)
                    ManualMaps.AddGlobal(sourceType, dest, groupName);
            }
        }
    }
}
