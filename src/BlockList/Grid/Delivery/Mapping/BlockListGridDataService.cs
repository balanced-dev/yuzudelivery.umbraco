using System;
using System.Collections.Generic;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web;
#else
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListGridDataService
    {
        private readonly IMapper mapper;
        private readonly IYuzuConfiguration config;
        private readonly string[] sectionAliases;

        private readonly IEnumerable<IGridItem> gridItems;
        private readonly IEnumerable<IGridItemInternal> gridItemsInternal;
#if NETCOREAPP
        private readonly IPublishedValueFallback publishedValueFallback;
#endif

        private IEnumerable<Type> viewmodelTypes;

        public BlockListGridDataService(IMapper mapper, IYuzuConfiguration config, IYuzuDeliveryImportConfiguration importConfig, IEnumerable<IGridItem> gridItems, IEnumerable<IGridItemInternal> gridItemsInternal
#if NETCOREAPP
            , IPublishedValueFallback publishedValueFallback
#endif
            )
        {
            this.mapper = mapper;
            this.config = config;
            this.sectionAliases = importConfig.GridRowConfigs.Select(x => x.Name.FirstCharacterToLower()).ToArray();

            this.gridItems = gridItems;
            this.gridItemsInternal = gridItemsInternal;
#if NETCOREAPP
            this.publishedValueFallback = publishedValueFallback;
#endif

            this.viewmodelTypes = config.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix) || x.Name.StartsWith(YuzuConstants.Configuration.SubPrefix));
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

                        return new vmSub_DataRowsRow()
                        {
                            Config = rowSettingsVm,
                            Items = columnContent?
                                .Select(cell => CreateContentAndConfig(new GridItemData(cell, context.Items)))
                                .Where(x => x != null).ToList()
                        };
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
                        var rowContent = rowBlockList.Content;
                        var rowSettingsVm = GetRowSettingsVm(rowBlockList, context);

                        var columns = rowContent.Properties.Where(y => !y.Alias.EndsWith("Settings"));

                        return new vmSub_DataGridRow()
                        {
                            Config = rowSettingsVm,
                            Columns = columns.Select(columnProperty => {

#if NETCOREAPP
                                var columnContent = columnProperty.Value<BlockListModel>(publishedValueFallback);
#else
                                var columnContent = columnProperty.Value<BlockListModel>();
#endif

                                var columnSettingsVm = GetColumnSettingsVm(rowContent, columnProperty, context);

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = 12 / columns.Count(),
                                    Config = columnSettingsVm,
                                    Items = columnContent?
                                        .Select(cell => CreateContentAndConfig(new GridItemData(cell, context.Items)))
                                        .Where(x => x != null).ToList()
                                };

                            }).ToList()
                        };
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
            if(vm != null)
                context.Items[_BlockList_Constants.RowSettings] = vm;

            return vm;
        }

        private object GetColumnSettingsVm(IPublishedElement rowContent, IPublishedProperty columnProperty, UmbracoMappingContext context)
        {
            context.Items.Remove(_BlockList_Constants.ColumnSettings);

            var columnConfig = rowContent.Value<BlockListModel>(columnProperty.Alias + "Settings")
           .Select(x => x.Content).FirstOrDefault();

            var vm = CreateVm(columnConfig, context.Items);
            if(vm != null)
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

}