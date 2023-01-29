using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umb = Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Logging;


namespace YuzuDelivery.Umbraco.Import
{
    public class BlockListDataTypeFactory
    {
        private readonly IDataTypeService dataTypeService;
        private readonly IContentTypeService contentTypeService;
        private readonly IContentTypeForVmService contentTypeForVmTypeService;
        private readonly IVmGetterService _vmGetterService;
        private readonly ISchemaMetaService _schemaMetaService;
        private readonly IDocumentTypePropertyService documentTypePropertyService;
        public const string DataEditorName = "Umbraco.BlockList";

        public const string DefaultCustomView = "~/App_Plugins/YuzuBlockList/GridContentItem.html";

        public BlockListDataTypeFactory(
            IDataTypeService dataTypeService,
            IDocumentTypePropertyService documentTypePropertyService,
            IContentTypeService contentTypeService,
            IContentTypeForVmService contentTypeForVmTypeService,
            ISchemaMetaService schemaMetaService)
        {
            this.dataTypeService = dataTypeService;
            this.documentTypePropertyService = documentTypePropertyService;
            this.contentTypeService = contentTypeService;
            this.contentTypeForVmTypeService = contentTypeForVmTypeService;
            _schemaMetaService = schemaMetaService;
        }

        public IDataType CreateOrUpdate(string viewModelName, string dataTypeName, string[] subBlocks, Options options = null)
        {
            var blocks = new List<BlockListConfiguration.BlockConfiguration>();

            var dataTypeDefinition = dataTypeService.GetByName(dataTypeName);
            if (dataTypeDefinition == null)
            {
                dataTypeDefinition = dataTypeService.CreateDataType(dataTypeName, DataEditorName, "YuzuDelivery.Umbraco.BlockList");
            }
            else
            {
                var config = dataTypeDefinition.Umb().Configuration as BlockListConfiguration;
                if(config != null)
                {
                    blocks = config.Blocks.ToList();
                }
            }

            var blockListConfig = CreateBlockListConfig(subBlocks, blocks, options, viewModelName);

            dataTypeDefinition.Umb().Name = dataTypeName;
            dataTypeDefinition.Umb().Configuration = blockListConfig;

            return dataTypeService.Save(dataTypeDefinition);
        }

        private BlockListConfiguration CreateBlockListConfig(string[] subBlocks, List<BlockListConfiguration.BlockConfiguration> blocks, Options options, string vmName)
        {
            options = options == null ? options = new Options() : options;
            var blockListConfig = options.Config == null ? new BlockListConfiguration() : options.Config;

            blockListConfig.ValidationLimit = new BlockListConfiguration.NumberRange();
            blockListConfig.ValidationLimit.Min = options.Min;
            blockListConfig.ValidationLimit.Max = options.Max;

            foreach (var subBlock in subBlocks)
            {
                //This just doesn't make sense
                /*if (!_schemaMetaService.TryGetPathSegments(subBlock, out var pathSegments))
                {
                    pathSegments = _schemaMetaService.GetPathSegments(vmName);
                }*/

                _schemaMetaService.TryGetPathSegments(subBlock, out var pathSegments);

                if (!DoesBlockAlreadyExist(subBlock, blocks, options))
                    blocks.Add(CreateBlockConfig(subBlock, options, pathSegments));
            }
            blockListConfig.Blocks = blocks.ToArray();


            return blockListConfig;
        }

        private bool DoesBlockAlreadyExist(string subBlock, List<BlockListConfiguration.BlockConfiguration> blocks, Options options)
        {
            var contentType = GetContentType(subBlock, options);
            return contentType != null && blocks.Any(x => x.ContentElementTypeKey == contentType.Umb().Key);
        }

        private BlockListConfiguration.BlockConfiguration CreateBlockConfig(string blockName, Options options, string[] pathSegments)
        {
            var contentType = CreateContentType(blockName, options, pathSegments);
            var settingsType = CreateSettingsType(options);

            return new BlockListConfiguration.BlockConfiguration()
            {
                Label = blockName.RemoveAllVmPrefixes().CamelToSentenceCase(),
                View = options.CustomView == null ? DefaultCustomView : options.CustomView,
                EditorSize = "medium",
                ContentElementTypeKey = contentType.Umb().Key,
                SettingsElementTypeKey = settingsType?.Umb().Key,
                ForceHideContentEditorInOverlay = options.ForceHideContentEditor
            };
        }

        private IContentType CreateContentType(string blockName, Options options, string[] pathSegments)
        {
            var contentType = contentTypeForVmTypeService.CreateOrUpdate(blockName, null, true, pathSegments);


            return contentType;
        }

        private IContentType GetContentType(string blockName, Options options)
        {
            return contentTypeForVmTypeService.Get(blockName);
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

        }

    }
}
