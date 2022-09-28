using System;
using System.Linq;
using System.Collections.Generic;
using Skybrud.Umbraco.GridData.Dtge;
using Newtonsoft.Json;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using Skybrud.Umbraco.GridData.Models;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridService : IGridService
    {
        private IMapper mapper;
        private readonly IEnumerable<IGridItem> gridItems;
        private readonly IEnumerable<IGridItemInternal> gridItemsInternal;
        private readonly IEnumerable<IAutomaticGridConfig> automaticGridConfig;

        public GridService(IMapper mapper, IEnumerable<IGridItem> gridItems, IEnumerable<IGridItemInternal> gridItemsInternal, IEnumerable<IAutomaticGridConfig> automaticGridConfig)
        {
            this.mapper = mapper;
            this.gridItems = gridItems;
            this.gridItemsInternal = gridItemsInternal;
            this.automaticGridConfig = automaticGridConfig;
        }


        public vmBlock_DataRows CreateRows<TConfig>(GridDataModel grid, UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataRows()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(y =>
                    {
                        var rowConfig = y.Config.ToDictionary(y.HasConfig).AddCalculatedRowConfig(y, automaticGridConfig);
                        var area = y.Areas.FirstOrDefault();

                        return new vmSub_DataRowsRow()
                        {
                            Config = mapper.Map<TConfig>(rowConfig),
                            Items = area.Controls.Select(control =>
                            {
                                var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                return Content(new GridItemData() { Control = control, ContextItems = context.Items, RowConfig = rowConfig });
                            }).Where(x => x != null).ToList()
                        };
                    }).ToList() : new List<vmSub_DataRowsRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataRows CreateRows(GridDataModel grid, UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataRows()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(y =>
                    {
                        var rowConfig = y.Config.ToDictionary(y.HasConfig).AddCalculatedRowConfig(y, automaticGridConfig);
                        var area = y.Areas.FirstOrDefault();

                        return new vmSub_DataRowsRow()
                        {
                            Items = area.Controls.Select(control =>
                            {
                                var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                return Content(new GridItemData() { Control = control, ContextItems = context.Items, RowConfig = rowConfig });
                            }).Where(x => x != null).ToList()
                        };
                    }).ToList() : new List<vmSub_DataRowsRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGrid CreateRowsColumns<TConfig>(GridDataModel grid, UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataGrid()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(row =>
                    {
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row, automaticGridConfig);

                        return new vmSub_DataGridRow()
                        {
                            Config = mapper.Map<TConfig>(rowConfig),
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area, automaticGridConfig);

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = area.Grid,
                                    Config = mapper.Map<TConfig>(columnConfig),
                                    Items = area.Controls.Select(control =>
                                    {
                                        var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                        return Content(new GridItemData() { Control = control, ContextItems = context.Items, RowConfig = rowConfig, ColConfig = columnConfig });
                                    }).Where(x => x != null).ToList()
                                };

                            }).ToList()
                        };
                    }).ToList() : new List<vmSub_DataGridRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGrid CreateRowsColumns<TConfigRow, TConfigCol>(GridDataModel grid, UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataGrid()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(row =>
                    {
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row, automaticGridConfig);

                        return new vmSub_DataGridRow()
                        {
                            Config = mapper.Map<TConfigRow>(rowConfig),
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area, automaticGridConfig);

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = area.Grid,
                                    Config = mapper.Map<TConfigCol>(columnConfig),
                                    Items = area.Controls.Select(control =>
                                    {
                                        var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                        return Content(new GridItemData() { Control = control, ContextItems = context.Items, RowConfig = rowConfig, ColConfig = columnConfig });
                                    }).Where(x => x != null).ToList()
                                };

                            }).ToList()
                        };
                    }).ToList() : new List<vmSub_DataGridRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGrid CreateRowsColumns(GridDataModel grid,UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataGrid()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(row =>
                    {
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row, automaticGridConfig);

                        return new vmSub_DataGridRow()
                        {
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area, automaticGridConfig);

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = area.Grid,
                                    Items = area.Controls.Select(control =>
                                    {
                                        var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                        return Content(new GridItemData() { Control = control, ContextItems = context.Items, RowConfig = rowConfig, ColConfig = columnConfig });
                                    }).Where(x => x != null).ToList()
                                };

                            }).ToList()
                        };
                    }).ToList() : new List<vmSub_DataGridRow>()
                };
            }
            else
                return null;
        }

        public object Content(GridItemData data)
        {
            foreach (var i in gridItems)
            {
                if (i.IsValid(data.Control.Editor.Alias, data.Control))
                    return i.Apply(data);
            }

            foreach (var i in gridItemsInternal)
            {
                if (i.IsValid(data.Control.Editor.Alias, data.Control))
                    return i.Apply(data);
            }

            return null;
        }
    }
}
