using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Web.PropertyEditors;
using Umb = Umbraco.Core.Services;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core;
using Umbraco.Core.Logging;

namespace YuzuDelivery.Umbraco.Import
{
    public class BlockListDataTypeFactory
    {
        private readonly IDataTypeService dataTypeService;
        private readonly IContentTypeService contentTypeService;
        private readonly IContentTypeForVmService contentTypeForVmTypeService;
        private readonly IDocumentTypePropertyService documentTypePropertyService;
        public const string DataEditorName = "Umbraco.BlockList";

        public const string DefaultCustomView = "~/App_Plugins/YuzuBlockList/GridContentItem.html";

        public BlockListDataTypeFactory(IDataTypeService dataTypeService, IDocumentTypePropertyService documentTypePropertyService, IContentTypeService contentTypeService, IContentTypeForVmService contentTypeForVmTypeService)
        {
            this.dataTypeService = dataTypeService;
            this.documentTypePropertyService = documentTypePropertyService;
            this.contentTypeService = contentTypeService;
            this.contentTypeForVmTypeService = contentTypeForVmTypeService;
        }

        public IDataType CreateOrUpdate(string dataTypeName, string[] subBlocks, Options options = null)
        {
            var blockListConfig = CreateBlockListConfig(subBlocks, options);

            var dataTypeDefinition = dataTypeService.GetByName(dataTypeName);
            if(dataTypeDefinition == null) dataTypeDefinition = dataTypeService.CreateDataType(dataTypeName, DataEditorName);
            dataTypeDefinition.Name = dataTypeName;
            dataTypeDefinition.Configuration = blockListConfig;

            return dataTypeService.Save(dataTypeDefinition);
        }

        private BlockListConfiguration CreateBlockListConfig(string[] subBlocks, Options options)
        {
            options = options == null ? options = new Options() : options; 
            var blockListConfig = options.Config == null ? new BlockListConfiguration() : options.Config;

            blockListConfig.ValidationLimit = new BlockListConfiguration.NumberRange();
            blockListConfig.ValidationLimit.Min = options.Min;
            blockListConfig.ValidationLimit.Max = options.Max;

            var contentTypes = new List<BlockListConfiguration.BlockConfiguration>();
            foreach (var subBlock in subBlocks)
            {
                contentTypes.Add(CreateBlockConfig(subBlock, options));
            }
            blockListConfig.Blocks = contentTypes.ToArray();

            return blockListConfig;
        }

        private BlockListConfiguration.BlockConfiguration CreateBlockConfig(string blockName, Options options)
        {
            var contentType = CreateContentType(blockName, options);
            var settingsType = CreateSettingsType(options);

            return new BlockListConfiguration.BlockConfiguration()
            {
                Label = blockName.RemoveAllVmPrefixes().CamelToSentenceCase(),
                View = options.CustomView == null ? DefaultCustomView : options.CustomView,
                EditorSize = "medium",
                ContentElementTypeKey = contentType.Key,
                SettingsElementTypeKey = settingsType?.Key,
                ForceHideContentEditorInOverlay = options.ForceHideContentEditor
            };
        }

        private IContentType CreateContentType(string blockName, Options options)
        {
            var contentType = options.CreateContentTypeAction != null ?
                options.CreateContentTypeAction(blockName, contentTypeService)
                :
                contentTypeForVmTypeService.CreateOrUpdate(blockName, null, true);

            if (options.CreatePropertiesAction != null) options.CreatePropertiesAction(contentType, documentTypePropertyService);

            return contentType;
        }

        private IContentType CreateSettingsType(Options options)
        {
            return options.SettingsSubBlock != null ? contentTypeForVmTypeService.CreateOrUpdate(options.SettingsSubBlock, null, true) : null;
        }

        public class Options
        {
            public BlockListConfiguration Config { get; set; }
            public int? Min { get; set; }
            public int? Max  { get; set; }
            public string CustomView { get; set; }
            public string SettingsSubBlock { get; set; }
            public bool ForceHideContentEditor { get; set; }

            public Func<string, IContentTypeService, IContentType> CreateContentTypeAction { get; set; }
            public Action<IContentType, IDocumentTypePropertyService> CreatePropertiesAction { get; set; }
        }

    }
}
