﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{
    public class GroupedConfigMappings : YuzuMappingConfig
    {
        public GroupedConfigMappings(IStoredConfigAsService storedConfigAsService, IOptions<YuzuConfiguration> config, IOptions<ImportSettings> importConfig, IVmGetterService vmGetterService)
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
                    ManualMaps.AddGroup(sourceType, destParent, destChild, parentPropertyName, groupName);
            }
        }
    }
}
