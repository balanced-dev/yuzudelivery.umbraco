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
                        var rowConfig = rowBlockList.Settings;

                        var columnProperty = rowContent.Properties.FirstOrDefault();
                        var columnContent = rowContent.Value<BlockListModel>(columnProperty.Alias);

                        return new vmSub_DataRowsRow()
                        {
                            Config = CreateVm(rowConfig, context.Items),
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
                    Rows = grid.Any() ? grid.Where(x => sectionAliases.Contains(x.Content.ContentType.Alias)).Select(row =>
                    {
                        var rowContent = row.Content;
                        var rowConfig = row.Settings;
                        var columns = rowContent.Properties.Where(y => !y.Alias.EndsWith("Settings"));

                        return new vmSub_DataGridRow()
                        {
                            Config = CreateVm(rowConfig, context.Items),
                            Columns = columns.Select(columnProperty => {

#if NETCOREAPP
                                var columnContent = columnProperty.Value<BlockListModel>(publishedValueFallback);
#else
                                var columnContent = columnProperty.Value<BlockListModel>();
#endif
                                var columnConfig = rowContent.Value<BlockListModel>(columnProperty.Alias + "Settings")
                                    .Select(x => x.Content).FirstOrDefault();

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = 12 / columns.Count(),
                                    Config = CreateVm(columnConfig, context.Items),
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

        public vmBlock_DataGridRowItem CreateContentAndConfig(GridItemData data)
        {
            return new vmBlock_DataGridRowItem()
            {
                Content = CreateVm(data.Content, data.ContextItems),
                Config = CreateVm(data.Config, data.ContextItems)
            };
        }


        public object CreateVm(IPublishedElement model, IDictionary<string, object> context)
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