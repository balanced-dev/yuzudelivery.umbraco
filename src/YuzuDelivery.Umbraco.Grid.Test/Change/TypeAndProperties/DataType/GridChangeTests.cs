using System;
using System.Linq;
using System.Collections.Generic;
using Umbraco.Web.PropertyEditors;
using NUnit.Framework;
using Autofac;
using YuzuDelivery.Umbraco.Grid;
using Rhino.Mocks;
using Newtonsoft.Json.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;

namespace YuzuDelivery.Umbraco.Grid.Test.Change.TypeAndProperties.DataType
{
    [Category("Change / Property and DataType")]
    [UseReporter(typeof(DiffReporter))]
    public class GridChangeTests : BaseTestSetup
    {
        public class vmBlock_With_Rows
        {
            public vmBlock_DataRows Rows { get; set; }
        }

        public class vmBlock_With_Grid
        {
            public vmBlock_DataGrid Grid { get; set; }
        }

        public SchemaChangeController svc;
        public VmToContentPropertyMap map;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            BaseFixtureSetup();
        }

        [SetUp]
        public void Setup()
        {
            BaseSetup();

            svc = container.Resolve<SchemaChangeController>();

            umb.ContentType.ForUpdating<vmBlock_With_Rows>();
            umb.ContentType.ForUpdating<vmBlock_With_Grid>();

            umb.ImportConfig.IgnoreViewmodels.Add("vmBlock_DataGrid");
            umb.ImportConfig.IgnoreViewmodels.Add("vmBlock_DataRows");
        }

        #region RowBuilder Basics

        [Test]
        public void can_create_row_builder_data_type()
        {
            SetupRowBuilder();

            umb.DataType.AddAndStubCreate(1, "Row Builder", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Rows>(x => x.Rows, 1, 1);

            svc.ChangeProperty(map);

            Assert.IsInstanceOf<GridConfiguration>(umb.DataType.Current.Configuration);
            Assert.AreEqual("Row Builder", umb.DataType.Current.Name);

            Approvals.AssertEquals(CreateRowConfig("100").ToJson(), umb.DataType.Current.Configuration.ToJson());
        }

        [Test]
        public void can_update_row_builder_data_type()
        {
            SetupRowBuilder();

            umb.DataType.AddAndStubUpdate(1, "Row Builder", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Rows>(x => x.Rows, 1, 1);

            umb.DataType.Current.Configuration = CreateRowConfig("100");
            map.ContentDataTypeId = 1;
            umb.ImportConfig.GridRowConfigs[0].Name = "100 Updated";

            svc.ChangeProperty(map);

            Approvals.AssertEquals(CreateRowConfig("100 Updated").ToJson(), umb.DataType.Current.Configuration.ToJson());
        }

        [Test]
        public void can_create_row_builder_property()
        {
            SetupRowBuilder();

            umb.DataType.AddAndStubUpdate(1, "Row Builder", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Rows>(x => x.Rows, 1, 1);

            svc.ChangeProperty(map);

            umb.PropertyType.WasCreated<vmBlock_With_Rows>(x => x.Rows);
        }

        private void SetupRowBuilder()
        {
            umb.Config.AddViewModel<vmBlock_With_Rows>();

            map = new VmToContentPropertyMap()
            {
                VmName = "vmBlock_With_Rows",
                VmPropertyName = "Rows",
                Config = new ContentPropertyConfig()
                {
                    TypeName = "vmBlock_DataRows",
                    IsGrid = true,
                    Grid = new ContentPropertyConfigGrid()
                    {
                        HasColumns = false,
                        OfType = "rowBuilder",
                        Sizes = new string[] { "12" }
                    }
                }
            };

            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(true, "100", "12", "12"));
        }

        #endregion

        #region Grid Basics

        [Test]
        public void can_create_grid_data_type()
        {
            SetupGrid();

            umb.DataType.AddAndStubCreate(1, "Grid", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Grid>(x => x.Grid, 1, 1);

            svc.ChangeProperty(map);

            Assert.IsInstanceOf<GridConfiguration>(umb.DataType.Current.Configuration);
            Assert.AreEqual("Grid", umb.DataType.Current.Name);

            Approvals.AssertEquals(CreateGridConfig("100").ToJson(), umb.DataType.Current.Configuration.ToJson());
        }

        [Test]
        public void can_update_grid_data_type()
        {
            SetupGrid();

            umb.DataType.AddAndStubUpdate(1, "Grid", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Grid>(x => x.Grid, 1, 1);

            umb.DataType.Current.Configuration = CreateGridConfig("100");
            map.ContentDataTypeId = 1;
            umb.ImportConfig.GridRowConfigs[0].Name = "100 Updated";

            svc.ChangeProperty(map);

            Approvals.AssertEquals(CreateGridConfig("100 Updated").ToJson(), umb.DataType.Current.Configuration.ToJson());
        }

        [Test]
        public void can_create_grid_property()
        {
            SetupGrid();

            umb.DataType.AddAndStubUpdate(1, "Grid", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Grid>(x => x.Grid, 1, 1);

            svc.ChangeProperty(map);

            umb.PropertyType.WasCreated<vmBlock_With_Grid>(x => x.Grid);
        }

        private void SetupGrid()
        {
            umb.Config.AddViewModel<vmBlock_With_Grid>();

            map = new VmToContentPropertyMap()
            {
                VmName = "vmBlock_With_Grid",
                VmPropertyName = "Grid",
                Config = new ContentPropertyConfig()
                {
                    TypeName = "vmBlock_DataGrid",
                    IsGrid = true,
                    Grid = new ContentPropertyConfigGrid()
                    {
                        HasColumns = true,
                        OfType = "grid",
                        Sizes = new string[] { "12", "6" }
                    }
                }
            };

            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(true, "100", "12", "12"));
            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(false, "50/50", "6", "6,6"));
        }

        #endregion

        #region DTGE config

        [Test]
        public void can_create_grid_config_settings_for_DTGE()
        {
            SetupGrid();

            map.Config.AllowedTypes = new string[] { "vmBlock_Rte", "vmBlock_GridImage" };

            umb.DataType.AddAndStubCreate(1, "Grid", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Grid>(x => x.Grid, 1, 1);

            svc.ChangeProperty(map);

            var config = umb.DTGEService.Configs.FirstOrDefault();

            Approvals.AssertEquals(CreateDTGEConfig("Grid", "grid", new string[] { "rte", "gridImage" }).ToJson(), config.ToJson());
        }

        [Test]
        public void can_update_grid_config_settings_for_DTGE()
        {
            SetupGrid();

            map.Config.AllowedTypes = new string[] { "vmBlock_Rte", "vmBlock_GridImage", "vmBlock_Summary" };

            umb.DataType.AddAndStubUpdate(1, "Grid", "Umbraco.Grid")
                .PropertyType.ForCreating<vmBlock_With_Grid>(x => x.Grid, 1, 1);

            umb.DTGEService.Configs.Add(CreateDTGEConfig("Grid", "grid", new string[] { "rte", "gridImage" }));
            map.ContentDataTypeId = 1;

            svc.ChangeProperty(map);

            var config = umb.DTGEService.Configs.FirstOrDefault();

            Approvals.AssertEquals(CreateDTGEConfig("Grid", "grid", new string[] { "rte", "gridImage", "summary" }).ToJson(), config.ToJson());
        }


        #endregion

        public DTGEService.GridConfig CreateDTGEConfig(string name, string alias, string[] allowedTypes)
        {
            return new DTGEService.GridConfig()
            {
                Name = name,
                Alias = alias,
                View = "/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.html",
                Render = "/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditor.cshtml",
                Icon = "icon-item-arrangement",
                Config = new DTGEService.DTGEConfig()
                {
                    AllowedTypes = allowedTypes.Select(x => $"\\b{x}\\b").ToArray(),
                    EnablePreview = true,
                    ViewPath = "/Views/Partials/Grid/Editors/DocTypeGridEditor/",
                    PreviewViewPath = "/Views/Partials/Grid/Editors/DocTypeGridEditor/Previews/",
                    PreviewCssFilePath = (string)null,
                    PreviewJsFilePath = (string)null
                }
            };
        }

        public object CreateRowConfig(string name)
        {
            return new
            {
                Items = new
                {
                    styles = new string[] { },
                    config = new string[] { },
                    columns = 12,
                    templates = new object[]
                    {
                        new {
                            name = "1 column layout",
                            sections = new object[]
                            {
                                new
                                {
                                    grid = 12,
                                    allowAll = true,
                                }
                            }
                        }
                    },
                    layouts = new object[]
                    {
                        new {
                            name = name,
                            areas = new object[]
                            {
                                new
                                {
                                    grid = "12",
                                    allowAll = false,
                                    allowed = new string[] { "rowBuilder" }
                                }
                            }
                        }
                    }
                },
                Rte = (string)null,
                IgnoreUserStartNodes = false,
                MediaParentId = (string)null
            };

        }

        public object CreateGridConfig(string name)
        {
            return new
            {
                Items = new
                {
                    styles = new string[] { },
                    config = new string[] { },
                    columns = 12,
                    templates = new object[]
                    {
                        new {
                            name = "1 column layout",
                            sections = new object[]
                            {
                                new
                                {
                                    grid = 12,
                                    allowAll = true,
                                }
                            }
                        }
                    },
                    layouts = new object[]
                    {
                        new {
                            name = name,
                            areas = new object[]
                            {
                                new
                                {
                                    grid = "12",
                                    allowAll = false,
                                    allowed = new string[] { "grid" }
                                }
                            }
                        },
                        new {
                            name = "50/50",
                            areas = new object[]
                            {
                                new
                                {
                                    grid = "6",
                                    allowAll = false,
                                    allowed = new string[] { "grid" }
                                },
                                new
                                {
                                    grid = "6",
                                    allowAll = false,
                                    allowed = new string[] { "grid" }
                                }
                            }
                        }
                    }
                },
                Rte = (string)null,
                IgnoreUserStartNodes = false,
                MediaParentId = (string)null
            };

        }
    }
}