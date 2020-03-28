using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using Newtonsoft.Json;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridService : IGridService
    {
        private IMapper mapper;
        private readonly IGridItem[] gridItems;
        private readonly IGridItemInternal[] gridItemsInternal;

        public GridService(IMapper mapper, IGridItem[] gridItems, IGridItemInternal[] gridItemsInternal)
        {
            this.mapper = mapper;
            this.gridItems = gridItems;
            this.gridItemsInternal = gridItemsInternal;
        }


        public vmBlock_DataRows CreateRows<TConfig>(GridDataModel grid, UmbracoMappingContext context)
        {
            if (grid != null)
            {
                return new vmBlock_DataRows()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(y =>
                    {
                        var rowConfig = y.Config.ToDictionary(y.HasConfig).AddCalculatedRowConfig(y);
                        var area = y.Areas.FirstOrDefault();

                        return new vmSub_DataRowsRow()
                        {
                            Config = mapper.Map<TConfig>(rowConfig),
                            Items = area.Controls.Select(control =>
                            {
                                var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                return Content(new GridItemData() { Control = control, ContextItems = context.Items });
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
                        var rowConfig = y.Config.ToDictionary(y.HasConfig).AddCalculatedRowConfig(y);
                        var area = y.Areas.FirstOrDefault();

                        return new vmSub_DataRowsRow()
                        {
                            Items = area.Controls.Select(control =>
                            {
                                var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                return Content(new GridItemData() { Control = control, ContextItems = context.Items });
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
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row);

                        return new vmSub_DataGridRow()
                        {
                            Config = mapper.Map<TConfig>(rowConfig),
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area);

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = area.Grid,
                                    Config = mapper.Map<TConfig>(columnConfig),
                                    Items = area.Controls.Select(control =>
                                    {
                                        var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                        return Content(new GridItemData() { Control = control, ContextItems = context.Items });
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
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row);

                        return new vmSub_DataGridRow()
                        {
                            Config = mapper.Map<TConfigRow>(rowConfig),
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area);

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = area.Grid,
                                    Config = mapper.Map<TConfigCol>(columnConfig),
                                    Items = area.Controls.Select(control =>
                                    {
                                        var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                        return Content(new GridItemData() { Control = control, ContextItems = context.Items });
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
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row);

                        return new vmSub_DataGridRow()
                        {
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area);

                                return new vmSub_DataGridColumn()
                                {
                                    GridSize = area.Grid,
                                    Items = area.Controls.Select(control =>
                                    {
                                        var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                        return Content(new GridItemData() { Control = control, ContextItems = context.Items });
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
