﻿using System.Linq;
using YuzuDelivery.Core;

#if NETCOREAPP
using Umbraco.Extensions;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.PropertyEditors;
#else
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web.PropertyEditors;
#endif

namespace YuzuDelivery.Umbraco.Import
{
    public class BlockListEditorCreationService : IInlineBlockCreator
    {
        private readonly BlockListDataTypeFactory blockListDataTypeFactory;
        protected ILogger<SchemaChangeController> logger;

        public BlockListEditorCreationService(BlockListDataTypeFactory blockListDataTypeFactory, ILogger<SchemaChangeController> logger)
        {
            this.blockListDataTypeFactory = blockListDataTypeFactory;
            this.logger = logger;
        }

        public virtual IDataType Create(VmToContentPropertyMap data)
        {
            logger.Info(_LoggerConstants.Schema_DataType_Creating, "Block List", data.VmName, data.VmPropertyName);

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
                if(data.Config.AllowedTypes.Any())
                {
                    buildOptions.Min = 0;
                    buildOptions.Max = 1;
                }
                else
                {
                    buildOptions.Config.UseSingleBlockMode = true;
                    buildOptions.Min = 1;
                    buildOptions.Max = 1;
                }
            }

            var subBlocks = data.Config.AllowedTypes.Any() ? data.Config.AllowedTypes : new string[] { data.Config.TypeName };

            return blockListDataTypeFactory.CreateOrUpdate(data.VmName, dataTypeName, subBlocks, buildOptions);
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
