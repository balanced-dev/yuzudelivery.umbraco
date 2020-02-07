using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using AutoMapper;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using Newtonsoft.Json;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridService : IGridService
    {
        private IMapper mapper;

        public GridService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public vmBlock_DataGridRows CreateRows<TConfig>(GridDataModel grid, ResolutionContext context)
        {
            var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

            if (grid != null)
            {
                return new vmBlock_DataGridRows()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(y =>
                    {
                        var rowConfig = y.Config.ToDictionary(y.HasConfig).AddCalculatedRowConfig(y);
                        var area = y.Areas.FirstOrDefault();

                        return new vmSub_DataGridRowsRow()
                        {
                            Config = mapper.Map<TConfig>(rowConfig),
                            Items = area.Controls.Select(control =>
                            {
                                var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                return Content(new GridItemData() { Control = control, ContextItems = context.Items });
                            }).Where(x => x != null).ToList()
                        };
                    }).ToList() : new List<vmSub_DataGridRowsRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGridRows CreateRows(GridDataModel grid, ResolutionContext context)
        {
            var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

            if (grid != null)
            {
                return new vmBlock_DataGridRows()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(y =>
                    {
                        var rowConfig = y.Config.ToDictionary(y.HasConfig).AddCalculatedRowConfig(y);
                        var area = y.Areas.FirstOrDefault();

                        return new vmSub_DataGridRowsRow()
                        {
                            Items = area.Controls.Select(control =>
                            {
                                var dtgeEditor = control.GetValue<GridControlDtgeValue>();
                                return Content(new GridItemData() { Control = control, ContextItems = context.Items });
                            }).Where(x => x != null).ToList()
                        };
                    }).ToList() : new List<vmSub_DataGridRowsRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGridRowsColumns CreateRowsColumns<TConfig>(GridDataModel grid, ResolutionContext context)
        {
            var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

            if (grid != null)
            {
                return new vmBlock_DataGridRowsColumns()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(row =>
                    {
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row);

                        return new vmSub_DataGridRowsColumnsRow()
                        {
                            Config = mapper.Map<TConfig>(rowConfig),
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area);

                                return new vmSub_DataGridRowsColumnsColumn()
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
                    }).ToList() : new List<vmSub_DataGridRowsColumnsRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGridRowsColumns CreateRowsColumns<TConfigRow, TConfigCol>(GridDataModel grid, ResolutionContext context)
        {
            var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

            if (grid != null)
            {
                return new vmBlock_DataGridRowsColumns()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(row =>
                    {
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row);

                        return new vmSub_DataGridRowsColumnsRow()
                        {
                            Config = mapper.Map<TConfigRow>(rowConfig),
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area);

                                return new vmSub_DataGridRowsColumnsColumn()
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
                    }).ToList() : new List<vmSub_DataGridRowsColumnsRow>()
                };
            }
            else
                return null;
        }

        public vmBlock_DataGridRowsColumns CreateRowsColumns(GridDataModel grid, ResolutionContext context)
        {
            var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

            if (grid != null)
            {
                return new vmBlock_DataGridRowsColumns()
                {
                    Rows = grid.Sections.Any() ? grid.Sections.FirstOrDefault().Rows.Where(x => x.Areas.Any()).Select(row =>
                    {
                        var rowConfig = row.Config.ToDictionary(row.HasConfig).AddCalculatedRowConfig(row);

                        return new vmSub_DataGridRowsColumnsRow()
                        {
                            Columns = row.Areas.Select(area => {
                                var columnConfig = area.Config.ToDictionary(area.HasConfig).AddCalculatedAreaSettings(row, area);

                                return new vmSub_DataGridRowsColumnsColumn()
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
                    }).ToList() : new List<vmSub_DataGridRowsColumnsRow>()
                };
            }
            else
                return null;
        }

        public object Content(GridItemData data)
        {
            var overrideGridItems = DependencyResolver.Current.GetServices<IGridItem>();

            foreach (var i in overrideGridItems)
            {
                if (i.IsValid(data.Control.Editor.Alias, data.Control))
                    return i.Apply(data);
            }

            var defaultGridItems = DependencyResolver.Current.GetService<IGridItem[]>();

            foreach (var i in defaultGridItems)
            {
                if (i.IsValid(data.Control.Editor.Alias, data.Control))
                    return i.Apply(data);
            }

            return null;
        }
    }
}
