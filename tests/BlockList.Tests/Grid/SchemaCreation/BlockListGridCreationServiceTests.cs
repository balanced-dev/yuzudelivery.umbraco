using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Mod = Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Newtonsoft.Json;
using Umb = Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Logging;
using ApprovalTests;
using ApprovalTests.Reporters;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Autofac;

namespace YuzuDelivery.Umbraco.BlockList.Tests.Grid
{
    [Category("BlockListEditor")]
    [UseReporter(typeof(DiffReporter))]
    public class BlockListGridCreationServiceTests : BaseTestSetup
    {
        public BlockListGridRowConfigToContent gridRowConfigToContent;
        public BlockListGridCreationService svc;

        public BlockListConfigurationBuilder builder;
        public VmToContentPropertyMapBuilder propertyMapBuilder;

        public BlockListGridRowConfigToContent.Property[] properties;

        public string propertyName;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            BaseFixtureSetup();
        }

        [SetUp]
        public void Setup()
        {
            BaseSetup(new BlockListTestBuilder());

            gridRowConfigToContent = container.Resolve<BlockListGridRowConfigToContent>();
            svc = container.Resolve<BlockListGridCreationService>();

            builder = new BlockListConfigurationBuilder(umb);
            propertyMapBuilder = new VmToContentPropertyMapBuilder(umb, names);
            propertyMapBuilder.AddGroup("Content");

        }

        [Test]
        public void can_create_section_data_type_config()
        {
            CreateData();

            SectionProperties(properties, assign: true);

            var output = svc.Create(propertyMapBuilder.CurrentProperty).Umb().Configuration;

            SectionProperties(properties, assert: true);

            builder.Expected.MaxPropertyWidth = "100%";

            Approvals.AssertEquals(builder.Expected.ToJson(), output.ToJson());

        }

        [Test]
        public void can_create_content_data_type_config()
        {
            umb.ContentType.ForCreating("Test");

            CreateData(blocks: new string[] { "Test" });

            SectionProperties(properties, assign: true);

            svc.Create(propertyMapBuilder.CurrentProperty);

            builder.AddBlock("Test", "Test", null);
            builder.Expected.UseLiveEditing = true;
            builder.Expected.MaxPropertyWidth = "100%";

            var config = umb.DataType.Added["Test Grid Content"].Configuration;

            Approvals.AssertEquals(builder.Expected.ToJson(), config.ToJson());

        }

        [Test]
        public void can_add_data_types()
        {
            CreateData();

            SectionProperties(properties, assign: true);

            svc.Create(propertyMapBuilder.CurrentProperty);

            SectionProperties(properties, assert: true);

            Assert.IsTrue(umb.DataType.Saved.Contains("Test Grid Content"));
            Assert.IsTrue(umb.DataType.Saved.Contains("Test Grid Sections"));
        }

        [Test]
        public void Can_add_row_item()
        {
            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(true, "RowItem", "100", "100"));

            CreateData(defaultGridRows: false, isGrid: false);

            SectionProperties(properties, assign: true);

            svc.Create(propertyMapBuilder.CurrentProperty);

            SectionProperties(properties, assert: true);

        }

        [TestCase(1, TestName = "Can add grid full width column")]
        [TestCase(2, TestName = "Can add grid full and two column")]
        [TestCase(3, TestName = "Can add grid full, two and three column")]
        [TestCase(3, TestName = "Can add grid full, two, three and four column")]
        [Test]
        public void adding_grid_rows(int rowCount)
        {
            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(true, "FullWidthSection", "100", "100"));
            if (rowCount > 1) umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(false, "TwoColumnSection", "50", "50,50"));
            if (rowCount > 2) umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(false, "ThreeColumnSection", "33", "33,33,33"));
            if (rowCount > 3) umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(false, "FourColumnSection", "25", "25,25,25,25"));

            CreateData(defaultGridRows: false);

            SectionProperties(properties, assign: true);

            svc.Create(propertyMapBuilder.CurrentProperty);

            SectionProperties(properties, assert: true);

        }

        [TestCase(1, null, null, TestName = "Can add column without config")]
        [TestCase(1, "RowConfig", null, TestName = "Can add column with row config")]
        [TestCase(1, "RowConfig", "ColConfig", TestName = "Can add column with row and column config")]
        [Test]
        public void adding_configs(int rowCount, string rowConfig, string colConfig)
        {
            CreateData(rowConfig, colConfig);

            if (rowConfig != null) umb.ContentType.ForCreating(rowConfig);
            if (colConfig != null) umb.ContentType.ForCreating(colConfig);

            SectionProperties(properties, rowConfig, colConfig, assign: true);

            svc.Create(propertyMapBuilder.CurrentProperty);

            if (rowConfig != null) umb.ContentType.WasCreated(rowConfig);
            if (colConfig != null)
            {
                umb.ContentType.WasCreated(colConfig);
                Assert.IsTrue(umb.DataType.Saved.Contains("Test Column Settings"));

                builder.AddBlock("Col Config", "ColConfig", customView: BlockListGridCreationService.ConfigCustomView);
                builder.Expected.ValidationLimit.Min = 0;
                builder.Expected.ValidationLimit.Max = 1;

                var config = umb.DataType.Added["Test Column Settings"].Configuration;
                Approvals.AssertEquals(builder.Expected.ToJson(), config.ToJson());
            }
        }

        [Test]
        public void Update_can_add_new_block()
        {
            umb.ContentType.ForUpdating("Test");
            umb.ContentType.ForCreating("Test2");

            var assignBuilder = new BlockListConfigurationBuilder(umb);
            assignBuilder.AddBlock("Test", "Test");

            CreateData(defaultGridRows: false, dataTypeToUpdate: true, blocks: new string[] { "Test", "Test2" });

            umb.DataType.Added["Test Grid Content"].Configuration = assignBuilder.Expected;

            SectionProperties(properties, null, null, assign: true);

            svc.Update(propertyMapBuilder.CurrentProperty, umb.DataType.Added["Test Grid Sections"].Yuzu());

            umb.ContentType.WasNotCreated("Test");
            umb.ContentType.WasCreated("Test2");

            builder.AddBlock("Test", "Test");
            builder.AddBlock("Test2", "Test2");
            builder.Expected.UseLiveEditing = true;
            builder.Expected.MaxPropertyWidth = "100%";

            var config = umb.DataType.Added["Test Grid Content"].Configuration;
            Approvals.AssertEquals(builder.Expected.ToJson(), config.ToJson());

        }

        [TestCase("contentTypeCreation", TestName = "Only creates content type for added row")]
        [TestCase("thumbnails", TestName = "Doesn't update thumbnail when blocklist updates")]
        public void Update_can_add_new_row_type(string mode)
        {
            umb.ContentType.ForUpdating("FullWidthSection");

            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(true, "FullWidthSection", "100", "100"));
            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(false, "TwoColumnSection", "50", "50,50"));

            CreateData(defaultGridRows: false, dataTypeToUpdate: true);

            var assignBuilder = new BlockListConfigurationBuilder(umb);
            if (mode == "thumbnails")
            {
                assignBuilder.AddBlock("FullWidthSection", "FullWidthSection", thumbnail: "thumbnail");
            }
            umb.DataType.Added["Test Grid Sections"].Configuration = assignBuilder.Expected;

            SectionProperties(properties, null, null, assign: true);

            svc.Update(propertyMapBuilder.CurrentProperty, umb.DataType.Added["Test Grid Sections"].Yuzu());

            if(mode == "contentTypeCreation")
            {
                umb.ContentType.WasNotCreated("FullWidthSection");
                umb.ContentType.WasCreated("TwoColumnSection");
            }
            else if(mode == "thumbnails")
            {
                var config = umb.DataType.Added["Test Grid Sections"].Configuration as BlockListConfiguration;
                Assert.AreEqual("thumbnail", config.Blocks[0].Thumbnail);
                Assert.AreEqual(null, config.Blocks[1].Thumbnail); ;
            }

        }

        public void SectionProperties(BlockListGridRowConfigToContent.Property[] properties, string rowConfig = null, string colConfig = null, bool assign = false, bool assert = false)
        {
            var sections = properties.Select(x => x.Type).Distinct();
            foreach(var section in sections)
            {
                var sectionProperties = properties.Where(x => x.Type == section);
                if(assign)
                {
                    umb.ContentType.ForCreating(section);
                    foreach(var p in sectionProperties)
                    {
                        if (!p.IsSettings)
                            umb.PropertyType.ForCreating(section, p.Name, alias: p.Alias);
                        if (p.IsSettings && colConfig != null)
                            umb.PropertyType.ForCreating(section, p.Name, alias: p.Alias);
                    }
                }
                else
                {
                    builder.AddBlock(section.CamelToSentenceCase(), section, rowConfig, true, customView: BlockListGridCreationService.SectionCustomView);
                    umb.ContentType.WasCreated(section);
                    foreach (var p in sectionProperties)
                    {
                        if (!p.IsSettings)
                            umb.PropertyType.WasCreated(section, p.Name, p.GroupName);
                        if (p.IsSettings && colConfig != null)
                            umb.PropertyType.WasCreated(section, p.Name, p.GroupName);

                    }
                }
            }
        }

        public void CreateData(string rowConfigType = null, string colConfigType = null, bool dataTypeToUpdate = false, bool defaultGridRows = true, bool isGrid = true, params string[] blocks)
        {
            var map = new VmToContentPropertyMap();
            map.Config.IsGrid = isGrid;
            map.Config.Grid.OfType = "Test";
            map.Config.Grid.Sizes = umb.ImportConfig.GridRowConfigs.SelectMany(x => x.DefinedSizes).ToArray();

            map.Config.AllowedTypes = blocks;

            if (rowConfigType != null) map.Config.Grid.RowConfigOfType = rowConfigType;
            if (colConfigType != null) map.Config.Grid.ColumnConfigOfType = colConfigType;

            propertyMapBuilder.AddProperty(map);

            if(!dataTypeToUpdate)
            {
                umb.DataType.AddAndStubCreate(1, "Test Grid Content", BlockListDataTypeFactory.DataEditorName);
                umb.DataType.AddAndStubCreate(1, "Test Column Settings", BlockListDataTypeFactory.DataEditorName);
                umb.DataType.AddAndStubCreate(1, "Test Grid Sections", BlockListDataTypeFactory.DataEditorName);
            }
            else
            {
                umb.DataType.AddAndStubUpdate(1, "Test Grid Content", BlockListDataTypeFactory.DataEditorName);
                umb.DataType.AddAndStubUpdate(1, "Test Column Settings", BlockListDataTypeFactory.DataEditorName);
                umb.DataType.AddAndStubUpdate(1, "Test Grid Sections", BlockListDataTypeFactory.DataEditorName);
            }

            if (defaultGridRows)
                umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(true, "FullWidthSection", "100", "100"));
            properties = gridRowConfigToContent.GetProperties(umb.ImportConfig.GridRowConfigs.ToArray());
        }

    }

}
