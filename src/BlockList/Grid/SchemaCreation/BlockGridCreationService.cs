using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using YuzuDelivery.Core;
using Umbraco.Cms.Core.PropertyEditors;
using YuzuDelivery.Import.Settings;
using BlockConfiguration = Umbraco.Cms.Core.PropertyEditors.BlockGridConfiguration.BlockGridBlockConfiguration;

namespace YuzuDelivery.Umbraco.Import
{
    public class BlockGridCreationService : IGridSchemaCreationService
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentTypeForVmService _contentTypeForVmService;
        private readonly ISchemaMetaService _schemaMetaService;
        private readonly IOptionsMonitor<ImportSettings> _importSettings;

        private static readonly Guid LayoutGroup = new("264d1010-120d-45b4-be7a-943b6252994c");
        private static readonly Guid ContentGroup = new("6cf43fb8-adf9-4664-a52d-4e22d0eca4bc");

        public BlockGridCreationService(
            IDataTypeService dataTypeService,
            IContentTypeService contentTypeService,
            IContentTypeForVmService contentTypeForVmService,
            ISchemaMetaService schemaMetaService,
            IOptionsMonitor<ImportSettings> importSettings)
        {
            _dataTypeService = dataTypeService;
            _contentTypeService = contentTypeService;
            _contentTypeForVmService = contentTypeForVmService;
            _schemaMetaService = schemaMetaService;
            _importSettings = importSettings;
        }

        public IDataType Create(VmToContentPropertyMap data)
        {
            var dataTypeName = GetDataTypeName(data);

            var dataType = _dataTypeService.CreateDataType(dataTypeName, Constants.PropertyEditors.Aliases.BlockGrid);
            dataType.Umb().Configuration = GetConfig(data);
            _dataTypeService.Save(dataType);
            return dataType;
        }

        public IDataType Update(VmToContentPropertyMap data, IDataType dataTypeDefinition)
        {
            dataTypeDefinition.Umb().Configuration = GetConfig(data);
            _dataTypeService.Save(dataTypeDefinition);
            return dataTypeDefinition;
        }

        public string GetDataTypeName(VmToContentPropertyMap data)
        {
            return data.Config.Grid.OfType.FirstCharacterToUpper().CamelToSentenceCase();
        }

        private BlockGridConfiguration GetConfig(VmToContentPropertyMap data)
        {
            var defaultBlocks = GetDefaultBlocks(data);
            var contentBlocks = GetContentBlocks(data);

            return new BlockGridConfiguration
            {
                BlockGroups = new []
                {
                    new BlockGridConfiguration.BlockGridGroupConfiguration
                    {
                        Name = "Layouts",
                        Key = LayoutGroup,
                    },
                    new BlockGridConfiguration.BlockGridGroupConfiguration
                    {
                        Name = "Content",
                        Key = ContentGroup,
                    }
                },
                Blocks = defaultBlocks.Concat(contentBlocks).ToArray()
            };
        }

        private IEnumerable<BlockConfiguration> GetDefaultBlocks(VmToContentPropertyMap data)
        {
            var columnSettingsType = string.IsNullOrEmpty(data.Config.Grid.ColumnConfigOfType)
                ? null
                : _contentTypeForVmService.CreateOrUpdate(data.Config.Grid.ColumnConfigOfType, null, true, new []{"Grid"});
            var columnContentType = _contentTypeService.Create("Grid Column", "gridColumn", true, new[] { "Grid" });
            var columnConfig = CreateLayoutBlockConfiguration("Column", columnContentType.Umb().Key, columnSettingsType?.Umb()?.Key);

            if (data.Config.Grid.HasColumns)
            {
                columnConfig.ColumnSpanOptions = _importSettings.CurrentValue.GridColumnSpanSizes
                    .Select(x => new BlockGridConfiguration.BlockGridColumnSpanOption { ColumnSpan = x })
                    .ToArray();
            }

            var rowSettingsType = string.IsNullOrEmpty(data.Config.Grid.ColumnConfigOfType)
                ? null
                : _contentTypeForVmService.CreateOrUpdate(data.Config.Grid.RowConfigOfType, null, true, new []{"Grid"});
            var rowContentType = _contentTypeService.Create("Grid Row", "gridRow", true, new[] { "Grid" });
            var rowConfig = CreateLayoutBlockConfiguration("Row", rowContentType.Umb().Key, rowSettingsType?.Umb()?.Key);
            rowConfig.AllowAtRoot = true;
            rowConfig.Areas.First().SpecifiedAllowance = new[]
            {
                new BlockGridConfiguration.BlockGridAreaConfigurationSpecifiedAllowance
                {
                    ElementTypeKey = columnContentType.Umb().Key
                }
            };

            yield return rowConfig;
            yield return columnConfig;
        }

        private IEnumerable<BlockConfiguration> GetContentBlocks(VmToContentPropertyMap data)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var block in data.Config.AllowedTypes)
            {
                if (!_schemaMetaService.TryGetPathSegments(block, out var pathSegments))
                {
                    pathSegments = _schemaMetaService.GetPathSegments(data.VmName);
                }
                var ct = _contentTypeForVmService.CreateOrUpdate(block, forceIsElement: true, pathSegments: pathSegments);

                var contentTypeName = ct.Name.RemoveAllVmPrefixes();
                var label = contentTypeName.CamelToSentenceCase();

                yield return CreateContentBlockConfiguration(label, ct.Umb().Key);
            }
        }

        private BlockConfiguration CreateLayoutBlockConfiguration(
            string label,
            Guid contentTypeKey,
            Guid? settingsTypeKey = null)
        {
            return new BlockGridConfiguration.BlockGridBlockConfiguration
            {
                Label = label,
                ContentElementTypeKey = contentTypeKey,
                SettingsElementTypeKey = settingsTypeKey,
                AllowAtRoot = false,
                AllowInAreas = false,
                RowMinSpan = 1,
                RowMaxSpan = 1,
                GroupKey = LayoutGroup.ToString(),
                EditorSize = "medium",
                ColumnSpanOptions = new []
                {
                    new BlockGridConfiguration.BlockGridColumnSpanOption
                    {
                        ColumnSpan = 12
                    }
                },
                Areas = new[]
                {
                    new BlockGridConfiguration.BlockGridAreaConfiguration
                    {
                        Alias = "content",
                        ColumnSpan = 12,
                        RowSpan = 1,
                        Key = Guid.NewGuid()
                    }
                }
            };
        }

        private BlockConfiguration CreateContentBlockConfiguration(
            string label,
            Guid contentTypeKey,
            Guid? settingsTypeKey = null)
        {
            return new BlockGridConfiguration.BlockGridBlockConfiguration
            {
                Label = label,
                ContentElementTypeKey = contentTypeKey,
                SettingsElementTypeKey = settingsTypeKey,
                AllowAtRoot = false,
                AllowInAreas = true,
                RowMinSpan = 1,
                RowMaxSpan = 1,
                GroupKey = ContentGroup.ToString(),
                EditorSize = "medium",
                ColumnSpanOptions = new []
                {
                    new BlockGridConfiguration.BlockGridColumnSpanOption
                    {
                        ColumnSpan = 12
                    }
                }
            };
        }
    }
}
