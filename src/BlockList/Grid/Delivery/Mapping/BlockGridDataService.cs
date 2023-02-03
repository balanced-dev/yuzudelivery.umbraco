using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockGridDataService : IBlockGridDataService
    {
        private readonly IMapper mapper;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly string[] sectionAliases;

        private readonly IEnumerable<IGridItem> gridItems;
        private readonly IEnumerable<IGridItemInternal> gridItemsInternal;
        private readonly IPublishedValueFallback publishedValueFallback;

        private IEnumerable<Type> viewmodelTypes;

        public BlockGridDataService(
            IMapper mapper,
            IOptions<YuzuConfiguration> config,
            IOptions<ImportSettings> importConfig,
            IEnumerable<IGridItem> gridItems,
            IEnumerable<IGridItemInternal> gridItemsInternal,
            IPublishedValueFallback publishedValueFallback)
        {
            this.mapper = mapper;
            this.config = config;
            this.sectionAliases = importConfig.Value.GridRowConfigs.Select(x => x.Name.FirstCharacterToLower()).ToArray();

            this.gridItems = gridItems;
            this.gridItemsInternal = gridItemsInternal;
            this.publishedValueFallback = publishedValueFallback;

            this.viewmodelTypes = config.Value.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix) || x.Name.StartsWith(YuzuConstants.Configuration.SubPrefix));
        }

        public vmBlock_DataRows CreateRows(BlockGridModel grid, UmbracoMappingContext context)
        {
            if (grid == null)
            {
                return null;
            }

            var result = new vmBlock_DataRows
            {
                Rows = grid.Select(x => MapRowRow(x, context)).ToList()
            };

            return result;
        }

        public vmBlock_DataGrid CreateRowsColumns(BlockGridModel grid, UmbracoMappingContext context)
        {
            if (grid == null)
            {
                return null;
            }

            var result = new vmBlock_DataGrid()
            {
                Rows = grid.Select(x => MapGridRow(x, context)).ToList()
            };

            return result;
        }

        private vmSub_DataRowsRow MapRowRow(BlockGridItem input, UmbracoMappingContext context)
        {
            var row = new vmSub_DataRowsRow();
            context.Items[_BlockList_Constants.ContextRow] = row;

            row.Config = GetContainerSettings(input, context);
            row.Items = input.Areas.First().Select(x => MapItem(x, context)).ToList();

            return row;
        }

        private vmSub_DataGridRow MapGridRow(BlockGridItem input, UmbracoMappingContext context)
        {
            var row = new vmSub_DataGridRow();
            context.Items[_BlockList_Constants.ContextRow] = row;

            row.ColumnCount = input.Areas.First().Count;
            row.Config = GetContainerSettings(input, context);
            row.Columns = input.Areas.First().Select((x, index) => MapColumn(index, x, context)).ToList();

            return row;
        }

        private vmSub_DataGridColumn MapColumn(int index, BlockGridItem input, UmbracoMappingContext context)
        {
            var col = new vmSub_DataGridColumn();
            context.Items[_BlockList_Constants.ContextColumn] = col;

            col.GridSize = input.ColumnSpan;
            col.Index = index;
            col.Config = GetContainerSettings(input, context);
            col.Items = input.Areas.First().Select(x => MapItem(x, context)).ToList();

            return col;
        }

        private vmBlock_DataGridRowItem MapItem(BlockGridItem input, UmbracoMappingContext context)
        {
            var item = new vmBlock_DataGridRowItem
            {
                Content = CreateVm(input.Content, context.Items)
            };

            return item;
        }

        private object GetContainerSettings(BlockGridItem container, UmbracoMappingContext context)
        {
            context.Items.Remove(_BlockList_Constants.ContainerSettings);

            var vm = CreateVm(container.Settings, context.Items);
            if (vm != null)
                context.Items[_BlockList_Constants.ContainerSettings] = vm;

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

}