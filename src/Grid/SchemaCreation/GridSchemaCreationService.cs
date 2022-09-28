using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Grid;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace YuzuDelivery.Umbraco.Import
{
    public class GridSchemaCreationService : IGridSchemaCreationService
    {
        private readonly IDataTypeService dataTypeService;
        private readonly ILogger<SchemaChangeController> logger;
        private readonly IYuzuDeliveryImportConfiguration importConfig;
        private readonly IDTGEService dgteService;

        public const string DataEditorName = "Umbraco.Grid";

        public GridSchemaCreationService(IDataTypeService dataTypeService, IYuzuDeliveryImportConfiguration importConfig, IDTGEService dgteService, ILogger<SchemaChangeController> logger)
        {
            this.dataTypeService = dataTypeService;
            this.importConfig = importConfig;
            this.dgteService = dgteService;
            this.logger = logger;
        }

        public virtual IDataType Create(VmToContentPropertyMap data)
        {
            logger.Info(_LoggerConstants.Schema_DataType_Creating, "Grid Layout", data.VmName, data.VmPropertyName);

            var dataTypeName = GetDataTypeName(data);
            var dataTypeAlias = GetDataTypeAlias(data);

            var gridConfig = new GridConfiguration();

            gridConfig.Items = JObject.FromObject(CreateConfigItems(data.Config, dataTypeAlias));


            var dataTypeDefinition = dataTypeService.CreateDataType(dataTypeName, DataEditorName);
            dataTypeDefinition.Umb().Configuration = gridConfig;

            dgteService.CreateOrUpdate(dataTypeName, dataTypeAlias, data.Config.AllowedTypes);

            return dataTypeService.Save(dataTypeDefinition);
        }

        public virtual IDataType Update(VmToContentPropertyMap data, IDataType dataTypeDefinition)
        {
            logger.Info(_LoggerConstants.Schema_DataType_Updating, "Grid Layout", data.VmName, data.VmPropertyName);

            var dataTypeName = GetDataTypeName(data);
            var dataTypeAlias = GetDataTypeAlias(data);

            var gridConfig = new GridConfiguration();

            gridConfig.Items = JObject.FromObject(CreateConfigItems(data.Config, dataTypeAlias));

            dataTypeDefinition.Umb().Configuration = gridConfig;

            dgteService.CreateOrUpdate(dataTypeName, dataTypeAlias, data.Config.AllowedTypes);

            return dataTypeService.Save(dataTypeDefinition);
        }

        public object CreateConfigItems(ContentPropertyConfig config, string dtgeTypeGridName)
        {
            return new
            {
                styles = new object[] { },
                config = CreateConfigConfigs(config),
                columns = 12,
                templates = CreateTemplate(),
                layouts = CreateConfigLayouts(config, dtgeTypeGridName)
            };
        }

        public object CreateTemplate()
        {
            return new object[] {
                new
                {
                    name = "1 column layout",
                    sections = new object[]
                    {
                        new
                        {
                            grid = 12,
                            allowAll = true
                        }
                    }
                }
            };
        }

        public object CreateConfigLayouts(ContentPropertyConfig config, string dtgeTypeGridName)
        {
            var sizes = config.Grid.Sizes;
            var output = new List<object>();

            foreach(var f in importConfig.GridRowConfigs)
            {
                if (f.IsDefault && !sizes.Any())
                    output.Add(CreateConfigLayout(f.Name, f.ActualSizes, dtgeTypeGridName));
                else
                {
                    if(sizes.Intersect(f.DefinedSizes).Any())
                    {
                        output.Add(CreateConfigLayout(f.Name, f.ActualSizes, dtgeTypeGridName));
                    }
                } 
            }

            return output;
        }

        public object CreateConfigLayout(string name, string[] gridSizes, string dtgeTypeGridName)
        {
            return new
            {
                name,
                areas = gridSizes.Select(x => new
                {
                    grid = x,
                    allowAll = false,
                    allowed = new string[]
                    {
                        dtgeTypeGridName
                    }
                })
            };
        }

        public object CreateConfigConfigs(ContentPropertyConfig config)
        {
            var output = new List<object>();

            foreach (var i in config.Grid.Config)
            {
                output.Add(CreateConfigConfig(i));
            }

            return output;
        }


        public object CreateConfigConfig(KeyValuePair<string, ContentPropertyConfigGrid.ConfigValueItem> item)
        {
            return new
            {
                label = item.Key,
                key = item.Key.FirstCharacterToLower(),
                view = item.Value.Editor,
                applyTo = GetConfigConfigApplyTo(item.Value),
                prevalues = item.Value.Prevalues
            };
        }

        public string GetConfigConfigApplyTo(ContentPropertyConfigGrid.ConfigValueItem config)
        {
            if (config.Row && config.Column)
                return null;
            else if (config.Row)
                return "row";
            else if (config.Column)
                return "cell";
            return string.Empty;
        }

        public virtual string GetDataTypeName(VmToContentPropertyMap data)
        {
            return data.Config.Grid.OfType.FirstCharacterToUpper().CamelToSentenceCase();
        }

        public virtual string GetDataTypeAlias(VmToContentPropertyMap data)
        {
            return data.Config.Grid.OfType;
        }

    }

}
