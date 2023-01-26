using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using YuzuDelivery.Umbraco.BlockList.Global;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.BlockList.Grid.ContentCreation;

public class BlockGridContentMapper : IContentMapper
{
    private readonly BlockListDbModelFactory _blockListDbModelFactory;
    private readonly IContentTypeService _contentTypeService;
    private readonly IDataTypeService _dataTypeService;
    private readonly GuidFactory _guidFactory;

    public BlockGridContentMapper(
        BlockListDbModelFactory blockListDbModelFactory,
        IContentTypeService contentTypeService,
        IDataTypeService dataTypeService,
        GuidFactory guidFactory)
    {
        _blockListDbModelFactory = blockListDbModelFactory;
        _contentTypeService = contentTypeService;
        _dataTypeService = dataTypeService;
        _guidFactory = guidFactory;
    }
    public bool IsValid(string propertyEditorAlias, ContentPropertyConfig config)
    {
        return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.BlockGrid)
               && config.IsGrid
               && config.Grid.HasColumns;
    }

    public string GetImportValue(VmToContentPropertyLink mapping, ContentPropertyConfig settings, string content, string source,
        IContentMapperFactory factory, IContentImportPropertyService import)
    {
        var dt = _dataTypeService.Get(mapping.CmsPropertyType.DataTypeId);
        var config = dt.Umb().ConfigurationAs<BlockGridConfiguration>()!;

        var rowType = _contentTypeService.GetByAlias("gridRow");
        var rowConfig = config.Blocks.Single(x => x.ContentElementTypeKey == rowType.Umb().Key);

        var columnType = _contentTypeService.GetByAlias("gridColumn");
        var columnConfig = config.Blocks.Single(x => x.ContentElementTypeKey == columnType.Umb().Key);

        var model = JsonConvert.DeserializeObject<vmBlock_DataGrid>(content);
        var result = new BlockGridDbModel();

        foreach (var row in model.Rows)
        {
            var rowLayout = AddRow(result, row, rowConfig, settings, import);
            result.Layout.BlockGrid.Add(rowLayout);

            foreach (var col in row.Columns)
            {
                var columnLayout = AddColumn(result, col, columnConfig, settings, import);
                rowLayout.AddChild(columnLayout);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var item in col.Items)
                {
                    var json = JObject.FromObject(item.Content ?? item.AdditionalProperties);
                    var contentBlock = _blockListDbModelFactory.GetObject(json, import);
                    result.ContentData.Add(JObject.FromObject(contentBlock));

                    var contentLayout = new BlockGridDbModel.GridLayoutItem((Udi)contentBlock["udi"]);
                    columnLayout.AddChild(contentLayout);
                }
            }
        }

        return JsonConvert.SerializeObject(result, Formatting.Indented);
    }

    private BlockGridDbModel.GridLayoutItem AddRow(
        BlockGridDbModel model,
        vmSub_DataGridRow row,
        BlockGridConfiguration.BlockGridBlockConfiguration config,
        ContentPropertyConfig settings,
        IContentImportPropertyService import)
    {
        var rowBlock = new BlockGridDbModel.Container(config, _guidFactory);
        model.ContentData.Add(JObject.FromObject(rowBlock));

        Udi rowSettingsUdi = null;
        // ReSharper disable once InvertIf
        if (row.Config != null)
        {
            var rowSettings = _blockListDbModelFactory.GetObject(JObject.FromObject(row.Config), import, vmName: settings.Grid.RowConfigOfType);
            rowSettingsUdi = (Udi) rowSettings["udi"];
            model.SettingsData.Add(JObject.FromObject(rowSettings));
        }

        return new BlockGridDbModel.GridLayoutItem(config, rowBlock.Udi, rowSettingsUdi);
    }

    private BlockGridDbModel.GridLayoutItem AddColumn(
        BlockGridDbModel model,
        vmSub_DataGridColumn col,
        BlockGridConfiguration.BlockGridBlockConfiguration config,
        ContentPropertyConfig settings,
        IContentImportPropertyService import)
    {
        var columnBlock = new BlockGridDbModel.Container(config, _guidFactory);
        model.ContentData.Add(JObject.FromObject(columnBlock));

        Udi columnSettingsUdi = null;
        // ReSharper disable once InvertIf
        if (col.Config != null)
        {
            var colSettings = _blockListDbModelFactory.GetObject(JObject.FromObject(col.Config), import, vmName: settings.Grid.ColumnConfigOfType);
            columnSettingsUdi = (Udi) colSettings["udi"];
            model.SettingsData.Add(JObject.FromObject(colSettings));
        }

        return new BlockGridDbModel.GridLayoutItem(config, columnBlock.Udi, columnSettingsUdi)
        {
            ColumnSpan = col.GridSize?.ToString().ToInt() ?? 12
        };
    }
}
