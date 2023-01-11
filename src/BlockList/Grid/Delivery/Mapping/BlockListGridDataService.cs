using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListGridDataService : IBlockListGridDataService
    {
        private readonly IMapper mapper;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly string[] sectionAliases;

        private readonly IEnumerable<IGridItem> gridItems;
        private readonly IEnumerable<IGridItemInternal> gridItemsInternal;
        private readonly IPublishedValueFallback publishedValueFallback;

        private IEnumerable<Type> viewmodelTypes;

        public BlockListGridDataService(IMapper mapper, IOptions<YuzuConfiguration> config, IOptions<ImportSettings> importConfig, IEnumerable<IGridItem> gridItems, IEnumerable<IGridItemInternal> gridItemsInternal, IPublishedValueFallback publishedValueFallback)
        {
            this.mapper = mapper;
            this.config = config;
            this.sectionAliases = importConfig.Value.GridRowConfigs.Select(x => x.Name.FirstCharacterToLower()).ToArray();

            this.gridItems = gridItems;
            this.gridItemsInternal = gridItemsInternal;
            this.publishedValueFallback = publishedValueFallback;

            this.viewmodelTypes = config.Value.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix) || x.Name.StartsWith(YuzuConstants.Configuration.SubPrefix));
        }

        public vmBlock_DataRows CreateRows(BlockListModel grid, UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataRows()
                {
                    Rows = grid.Any() ? grid.Where(x => sectionAliases.Contains(x.Content.ContentType.Alias)).Select(rowBlockList =>
                    {
                        var rowContent = rowBlockList.Content;
                        var rowSettingsVm = GetRowSettingsVm(rowBlockList, context);

                        var columnProperty = rowContent.Properties.FirstOrDefault();
                        var columnContent = rowContent.Value<BlockListModel>(columnProperty.Alias);

                        var row = new vmSub_DataRowsRow()
                        {
                            Config = rowSettingsVm,
                            Items = columnContent?
                                .Select(cell => CreateContentAndConfig(new GridItemData(cell, context.Items)))
                                .Where(x => x != null).ToList()
                        };

                        context.Items[_BlockList_Constants.ContextRow] = row;

                        return row;

                    }).ToList() : new List<vmSub_DataRowsRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGrid CreateRowsColumns(BlockListModel grid, UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataGrid()
                {
                    Rows = grid.Any() ? grid.Where(x => sectionAliases.Contains(x.Content.ContentType.Alias)).Select(rowBlockList =>
                    {
                        var row = new vmSub_DataGridRow();
                        context.Items[_BlockList_Constants.ContextRow] = row;

                        var rowContent = rowBlockList.Content;
                        var columns = rowContent.Properties.Where(y => !y.Alias.EndsWith("Settings"));

                        row.Config = GetRowSettingsVm(rowBlockList, context);
                        row.ColumnCount = columns.Count();

                        int index = 0;
                        row.Columns = columns.Select(columnProperty =>
                        {
                            var column = new vmSub_DataGridColumn()
                            {
                                GridSize = 12 / columns.Count(),
                            };

                            context.Items[_BlockList_Constants.ContextColumn] = column;
                            var columnContent = columnProperty.Value<BlockListModel>(publishedValueFallback);

                            column.Index = index;
                            column.Config = GetColumnSettingsVm(rowContent, columnProperty, context);
                            column.Items = columnContent?
                                    .Select(cell => CreateContentAndConfig(new GridItemData(cell, context.Items)))
                                    .Where(x => x != null).ToList();

                            index++;

                            return column;

                        }).ToList();

                        return row;

                    }).ToList() : new List<vmSub_DataGridRow>()
                };
            }
            else
                return null;
        }

        private object GetRowSettingsVm(BlockListItem rowBlockList, UmbracoMappingContext context)
        {
            context.Items.Remove(_BlockList_Constants.RowSettings);

            var rowConfig = rowBlockList.Settings;

            var vm = CreateVm(rowConfig, context.Items);
            if (vm != null)
                context.Items[_BlockList_Constants.RowSettings] = vm;

            return vm;
        }

        private object GetColumnSettingsVm(IPublishedElement rowContent, IPublishedProperty columnProperty, UmbracoMappingContext context)
        {
            context.Items.Remove(_BlockList_Constants.ColumnSettings);

            var columnConfig = rowContent.Value<BlockListModel>(columnProperty.Alias + "Settings")
           .Select(x => x.Content).FirstOrDefault();

            var vm = CreateVm(columnConfig, context.Items);
            if (vm != null)
                context.Items[_BlockList_Constants.ColumnSettings] = vm;

            return vm;
        }

        public object GetContentSettingsVm(GridItemData data)
        {
            data.ContextItems.Remove(_BlockList_Constants.ContentSettings);

            var vm = CreateVm(data.Config, data.ContextItems);
            if (vm != null)
                data.ContextItems[_BlockList_Constants.ContentSettings] = vm;
            return vm;
        }

        public virtual vmBlock_DataGridRowItem CreateContentAndConfig(GridItemData data)
        {
            var settingsVm = GetContentSettingsVm(data);
            var contentVm = CreateVm(data.Content, data.ContextItems);

            return new vmBlock_DataGridRowItem()
            {
                Content = contentVm,
                Config = settingsVm
            };
        }


        public virtual object CreateVm(IPublishedElement model, IDictionary<string, object> context)
        {
            foreach (var i in gridItems)
            {
                if (i.IsValid(model))
                    return i.Apply(model, context);
            }

            foreach (var i in gridItemsInternal)
            {
                if (i.IsValid(model))
                    return i.Apply(model, context);
            }

            return null;
        }


    }

    public class GridContext
    {
        public List<vmSub_DataGridRow> Rows { get; set; }

        public vmSub_DataGridRow CurrentRow { get; set; }

        public vmSub_DataGridColumn CurrentColumns {  get; set; }
    }

}
