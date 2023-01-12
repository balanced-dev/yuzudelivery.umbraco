using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{
    public class GroupedConfigMappings : IConfigureOptions<ManualMapping>
    {
        private readonly IStoredConfigAsService storedConfigAsService;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly IOptions<ImportSettings> importConfig;
        private readonly IVmGetterService vmGetterService;

        public GroupedConfigMappings(IStoredConfigAsService storedConfigAsService, IOptions<YuzuConfiguration> config, IOptions<ImportSettings> importConfig, IVmGetterService vmGetterService)
        {
            this.storedConfigAsService = storedConfigAsService;
            this.config = config;
            this.importConfig = importConfig;
            this.vmGetterService = vmGetterService;
        }

        public void Configure(ManualMapping options)
        {
            var groupedConfigs = storedConfigAsService.GetAll<GroupStoreContentAs>();

            foreach(var group in groupedConfigs)
            {
                var vmName = group.Key;
                var documentTypeAlias = group.Value.DocumentTypeAlias;
                var groupSettings = group.Value.StoreContentAs.As<GroupStoreContentAs>();

                var groupName = group.Value.StoreContentAs.GroupName;
                if (string.IsNullOrEmpty(groupName))
                    groupName = importConfig.Value.DefaultPropertyGroup;

                var parentPropertyName = groupSettings.ParentPropertyName;

                var sourceType = config.Value.CMSModels.Where(x => x.Name.ToLower() == documentTypeAlias.ToLower()).FirstOrDefault();
                var destParent = config.Value.ViewModels.Where(x => x.Name == groupSettings.ParentPropertyType).FirstOrDefault();
                var destChild = config.Value.ViewModels.Where(x => x.Name == vmName).FirstOrDefault();

                if(sourceType != null && destParent != null && destChild != null)
                    options.ManualMaps.AddGroup(sourceType, destParent, destChild, parentPropertyName, groupName);
            }
        }
    }
}
