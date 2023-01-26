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

        public BlockListGridDataService(
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
                Rows = grid.Select(x => MapRow(x, context)).ToList()
            };

            return result;
        }

        private vmSub_DataRowsRow MapRowRow(BlockGridItem input, UmbracoMappingContext context)
        {
            var row = new vmSub_DataRowsRow
            {
                Config = GetContainerSettings(input, context),
                Items = input.Areas.First().Select(x => MapItem(x, context)).ToList()
            };
            context.Items[_BlockList_Constants.ContextRow] = row;

            return row;
        }
        private vmSub_DataGridRow MapRow(BlockGridItem input, UmbracoMappingContext context)
        {
            var row = new vmSub_DataGridRow
            {
                Config = GetContainerSettings(input, context),
                ColumnCount = input.Areas.First().Count,
                Columns = input.Areas.First().Select(x => MapColumn(x, context)).ToList()
            };
            context.Items[_BlockList_Constants.ContextRow] = row;

            return row;
        }

        private vmSub_DataGridColumn MapColumn(BlockGridItem input, UmbracoMappingContext context)
        {
            var col = new vmSub_DataGridColumn
            {
                Config = GetContainerSettings(input, context),
                Items = input.Areas.First().Select(x => MapItem(x, context)).ToList(),
                GridSize = input.ColumnSpan
            };
            context.Items[_BlockList_Constants.ContextColumn] = col;

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
