﻿using System.Linq;
using YuzuDelivery.Core;

#if NETCOREAPP
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.PropertyEditors;
#else
using Umbraco.Core.Logging;
using Umbraco.Web.PropertyEditors;
#endif

namespace YuzuDelivery.Umbraco.Import
{
    public class BlockListGridCreationService : IGridSchemaCreationService
    {
        private readonly BlockListGridRowConfigToContent gridRowConfigToContent;
        private readonly BlockListDataTypeFactory blockListDataTypeFactory;

        public const string SectionCustomView = "~/App_Plugins/YuzuBlockList/GridContentSection.html";
        public const string ConfigCustomView = "~/App_Plugins/YuzuBlockList/GridContentColumnsSettings.html";

        public BlockListGridCreationService(BlockListGridRowConfigToContent gridRowConfigToContent, BlockListDataTypeFactory blockListDataTypeFactory, IYuzuDeliveryImportConfiguration importConfig)
        {
            this.gridRowConfigToContent = gridRowConfigToContent;
            this.blockListDataTypeFactory = blockListDataTypeFactory;
        }

        public IDataType Create(VmToContentPropertyMap data)
        {
            var contentEditor = CreatOrUpdateContentEditor(data);
            var columnSettingsEditor = CreatOrUpdateColumnSettingsEditor(data);

            return CreatOrUpdateSectionEditor(data, contentEditor.Id, columnSettingsEditor?.Id);
        }

        public IDataType Update(VmToContentPropertyMap data, IDataType dataTypeDefinition)
        {
            var contentEditor = CreatOrUpdateContentEditor(data);
            var columnSettingsEditor = CreatOrUpdateColumnSettingsEditor(data);

            return CreatOrUpdateSectionEditor(data, contentEditor.Id, columnSettingsEditor?.Id);
        }

        private IDataType CreatOrUpdateColumnSettingsEditor(VmToContentPropertyMap data)
        {
            if (data.Config.Grid.ColumnConfigOfType != null)
            {
                var subBlocks = new string[] { data.Config.Grid.ColumnConfigOfType };
                var name = $"{GetDataTypeName(data)} Column Settings";

                var options = new BlockListDataTypeFactory.Options();
                options.CustomView = ConfigCustomView;
                options.Min = 0;
                options.Max = 1;

                return blockListDataTypeFactory.CreateOrUpdate(name, subBlocks, options);
            }
            else
                return null;
        }

        private IDataType CreatOrUpdateContentEditor(VmToContentPropertyMap data)
        {
           var subBlocks = data.Config.AllowedTypes;
            var name = $"{GetDataTypeName(data)} Grid Content";

            var options = new BlockListDataTypeFactory.Options();
            options.Config = new BlockListConfiguration()
            {
                UseLiveEditing = true,
                MaxPropertyWidth = "100%"
            };

            return blockListDataTypeFactory.CreateOrUpdate(name, subBlocks, options);
        }

        private IDataType CreatOrUpdateSectionEditor(VmToContentPropertyMap data, int contentDataTypeId, int? settingsDataType)
        {
            var dataTypeName = $"{GetDataTypeName(data)} Grid Sections";

            var options = new BlockListDataTypeFactory.Options();
            options.ForceHideContentEditor = true;
            options.SettingsSubBlock = data.Config.Grid.RowConfigOfType;
            options.CustomView = SectionCustomView;

            options.Config = new BlockListConfiguration()
            {
                MaxPropertyWidth = "100%"
            };

            var gridconfigs = gridRowConfigToContent.GetSectionBlocks(data.Config);
            var sectionProperties = gridRowConfigToContent.GetProperties(gridconfigs);
            var sectionNames = sectionProperties.Select(x => x.Type).Distinct().ToArray();

            options.CreateContentTypeAction = (string name, IContentTypeService contentTypeService) =>
            {
                return contentTypeService.Create(name, name.AsAlias(), true);
            };

            options.GetContentTypeAction = (string name, IContentTypeService contentTypeService) =>
            {
                return contentTypeService.GetByAlias(name.AsAlias());
            };

            options.CreatePropertiesAction = (IContentType contentType, IDocumentTypePropertyService documentTypePropertyService) =>
            {
                var properties = sectionProperties.Where(x => x.Type == contentType.Name);
                foreach (var p in properties)
                {
                    if (!p.IsSettings)
                        documentTypePropertyService.Create(contentType, p.GroupName, p.Name, p.Alias, contentDataTypeId);
                    if (p.IsSettings && settingsDataType != null)
                        documentTypePropertyService.Create(contentType, p.GroupName, p.Name, p.Alias, settingsDataType.Value);
                }
            };

            return blockListDataTypeFactory.CreateOrUpdate(dataTypeName, sectionNames, options);
        }

        public string GetDataTypeAlias(VmToContentPropertyMap data)
        {
            return data.Config.Grid.OfType;
        }

        public string GetDataTypeName(VmToContentPropertyMap data)
        {
            return data.Config.Grid.OfType.FirstCharacterToUpper().CamelToSentenceCase();
        }
    }

}
