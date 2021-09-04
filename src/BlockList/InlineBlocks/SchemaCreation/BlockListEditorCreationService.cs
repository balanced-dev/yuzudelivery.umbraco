using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Web.PropertyEditors;
using Umbraco.Core.Logging;
using YuzuDelivery.Core;
using Umb = Umbraco.Core.Services;

namespace YuzuDelivery.Umbraco.Import
{
    public class BlockListEditorCreationService : IInlineBlockCreator
    {
        private readonly BlockListDataTypeFactory blockListDataTypeFactory;
        private readonly ILogger logger;

        public BlockListEditorCreationService(BlockListDataTypeFactory blockListDataTypeFactory, ILogger logger)
        {
            this.blockListDataTypeFactory = blockListDataTypeFactory;
            this.logger = logger;
        }

        public virtual IDataType Create(VmToContentPropertyMap data)
        {
            logger.Info<SchemaChangeController>(_LoggerConstants.Schema_DataType_Creating, "Block List", data.VmName, data.VmPropertyName);

            var dataTypeName = GetDataTypeName(data);

            var buildOptions = new BlockListDataTypeFactory.Options();
            buildOptions.CustomView = string.Empty;
            buildOptions.Config = new BlockListConfiguration()
            {
                UseInlineEditingAsDefault = true
            };

            if(data.Config.IsList)
            {
                buildOptions.Min = 0;
            }
            else
            {
                buildOptions.Min = 0;
                buildOptions.Max = 1;
            }

            var subBlocks = data.Config.AllowedTypes.Any() ? data.Config.AllowedTypes : new string[] { data.Config.TypeName };

            return blockListDataTypeFactory.CreateOrUpdate(dataTypeName, subBlocks, buildOptions);
        }

        public virtual string GetDataTypeName(VmToContentPropertyMap data)
        {
            var config = data.Config;
            var dataTypeName = string.Empty;

            if (!config.AllowedTypes.Any())
            {
                var contentTypeName = config.TypeName.RemoveAllVmPrefixes();
                dataTypeName = contentTypeName.CamelToSentenceCase();
            }
            else
            {
                var propertyTypeName = data.VmPropertyName;
                dataTypeName = propertyTypeName.CamelToSentenceCase();
            }

            if (config.IsList)
                dataTypeName = dataTypeName.MakePluralName();

            return dataTypeName;
        }
    }

}
