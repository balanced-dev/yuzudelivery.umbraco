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
    public class GroupedConfigMappings : YuzuMappingConfig
    {
        public GroupedConfigMappings(IStoredConfigAsService storedConfigAsService, IYuzuConfiguration config, IYuzuDeliveryImportConfiguration importConfig, IVmGetterService vmGetterService)
        {
            var groupedConfigs = storedConfigAsService.GetAll<GroupStoreContentAs>();

            foreach(var group in groupedConfigs)
            {
                var vmName = group.Key;
                var documentTypeAlias = group.Value.DocumentTypeAlias;

                var groupName = group.Value.StoreContentAs.GroupName;
                if (string.IsNullOrEmpty(groupName))
                    groupName = importConfig.DefaultPropertyGroup;

                var parentPropertyName = group.Value.StoreContentAs.As<GroupStoreContentAs>().ParentPropertyName;

                var sourceType = config.CMSModels.Where(x => x.Name.ToLower() == documentTypeAlias.ToLower()).FirstOrDefault();
                var destParent = config.ViewModels.Where(x => x.Name == vmGetterService.GetVmTypeNameFromDocumentTypeAlias(documentTypeAlias)).FirstOrDefault();
                var destChild = config.ViewModels.Where(x => x.Name == vmName).FirstOrDefault();

                if(sourceType != null && destParent != null && destChild != null)
                    ManualMaps.AddGroup(sourceType, destParent, destChild, groupName, groupName);
            }

        }
    }
}
