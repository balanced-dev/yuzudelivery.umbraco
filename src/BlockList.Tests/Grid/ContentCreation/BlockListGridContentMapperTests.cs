using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Umbraco.Core.Models;
using Umbraco.Web.PropertyEditors;
using Newtonsoft.Json;
using Umb = Umbraco.Core.Services;
using Umbraco.Core.Logging;
using YuzuDelivery.Umbraco.Import;
using ApprovalTests;
using ApprovalTests.Reporters;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Autofac;
using System.Linq.Expressions;

namespace YuzuDelivery.Umbraco.BlockList.Tests.Inline
{
    [Category("BlockListEditor")]
    [UseReporter(typeof(DiffReporter))]
    public partial class BlockListGridContentMapperTests : BaseTestSetup
    {
        public IContentMapperFactory contentMapperFactory;
        public IContentImportPropertyService contentImportPropertyService;

        public BlockListGridContentMapper svc;

        public BlockListGridRowConfigToContent.Property[] properties;

        public VmToContentPropertyMapBuilder propertyMapBuilder;

        public BlockListGridRowConfigToContent gridRowConfigToContent;

        public ContentPropertyConfig config;

        public vmBlock_DataRows dataRows;
        public vmBlock_DataGrid dataGrid;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            BaseFixtureSetup();
        }

        [SetUp]
        public void Setup()
        {
            BaseSetup(new BlockListTestBuilder());

            svc = container.Resolve<BlockListGridContentMapper>();
            contentMapperFactory = container.Resolve<IContentMapperFactory>();
            contentImportPropertyService = container.Resolve<IContentImportPropertyService>();

            dataRows = new vmBlock_DataRows();
            dataGrid = new vmBlock_DataGrid();

            config = new ContentPropertyConfig();

            propertyMapBuilder = new VmToContentPropertyMapBuilder(umb, names);
            propertyMapBuilder.AddGroup("Content");

            gridRowConfigToContent = container.Resolve<BlockListGridRowConfigToContent>();

            umb.Config.AddViewModel<vmBlock_ContentBlock>();
            umb.ContentType.ForUpdating<vmBlock_ContentBlock>()
                .PropertyType.ForUpdating<vmBlock_ContentBlock>(x => x.Title, groupName: "Content");

            umb.Config.AddViewModel<vmBlock_SettingsBlock>();
            umb.ContentType.ForUpdating<vmBlock_SettingsBlock>()
                .PropertyType.ForUpdating<vmBlock_SettingsBlock>(x => x.Id, groupName: "Content");

        }

        [Test]
        public void DataRow_multiple_items_in_one_row()
        {
            CreateData();

            SectionProperties(properties, AssignSections);

            var outer = CreateBuilder();
            var innerRow = CreateBuilder();

            dataRows.Rows.Add(new vmSub_DataRowsRow());
            dataRows.Rows[0].Items.Add(new vmBlock_DataGridRowItem());
            dataRows.Rows[0].Items[0].Content = CreateContentBlockVm(innerRow, "title");

            dataRows.Rows[0].Items.Add(new vmBlock_DataGridRowItem());
            dataRows.Rows[0].Items[1].Content = CreateContentBlockVm(innerRow, "title2");

            StubFullWidthRow(outer, innerRow);

            ActAndAssert(dataRows, outer);
        }

        [Test]
        public void DataRow_multiple_items_in_two_rows()
        {
            CreateData();

            SectionProperties(properties, AssignSections);

            var outer = CreateBuilder();
            var innerRow1 = CreateBuilder();

            dataRows.Rows.Add(new vmSub_DataRowsRow());
            dataRows.Rows[0].Items.Add(new vmBlock_DataGridRowItem());
            dataRows.Rows[0].Items[0].Content = CreateContentBlockVm(innerRow1, "title");

            StubFullWidthRow(outer, innerRow1);

            var innerRow2 = CreateBuilder();

            dataRows.Rows.Add(new vmSub_DataRowsRow());
            dataRows.Rows[1].Items.Add(new vmBlock_DataGridRowItem());
            dataRows.Rows[1].Items[0].Content = CreateContentBlockVm(innerRow2, "title2");

            StubFullWidthRow(outer, innerRow2);

            ActAndAssert(dataRows, outer);
        }

        [Test]
        public void DataRow_row_config()
        {
            CreateData();

            SectionProperties(properties, AssignSections);

            var outer = CreateBuilder();
            var innerRow = CreateBuilder();

            dataRows.Rows.Add(new vmSub_DataRowsRow());
            dataRows.Rows[0].Items.Add(new vmBlock_DataGridRowItem());
            dataRows.Rows[0].Items[0].Content = CreateContentBlockVm(innerRow, "title");
            dataRows.Rows[0].Config = CreateSettingsBlockVm(outer, "hero");

            StubFullWidthRow(outer, innerRow);

            ActAndAssert(dataRows, outer);
        }

        [Test]
        public void DataRow_item_config()
        {
            CreateData();

            SectionProperties(properties, AssignSections);

            var outer = CreateBuilder();
            var innerRow = CreateBuilder();

            dataRows.Rows.Add(new vmSub_DataRowsRow());
            dataRows.Rows[0].Items.Add(new vmBlock_DataGridRowItem());
            dataRows.Rows[0].Items[0].Content = CreateContentBlockVm(innerRow, "title");
            dataRows.Rows[0].Items[0].Config = CreateSettingsBlockVm(innerRow, "hero");

            StubFullWidthRow(outer, innerRow);

            ActAndAssert(dataRows, outer);
        }

        [Test]
        public void DataGrid_single_column_item()
        {
            CreateData();

            SectionProperties(properties, AssignSections);

            var outer = CreateBuilder();
            var innerRow = CreateBuilder();

            dataGrid.Rows.Add(new vmSub_DataGridRow());
            dataGrid.Rows[0].Columns.Add(new vmSub_DataGridColumn());
            dataGrid.Rows[0].Columns[0].Items.Add(new vmBlock_DataGridRowItem());
            dataGrid.Rows[0].Columns[0].Items[0].Content = CreateContentBlockVm(innerRow, "title");

            StubFullWidthRow(outer, innerRow);

            ActAndAssert(dataGrid, outer);
        }

        [Test]
        public void DataGrid_two_columns_items()
        {
            umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(false, "twoColumnSection", "50", "50,50"));

            CreateData(defaultGridRows: false);

            SectionProperties(properties, AssignSections);

            var outer = CreateBuilder();
            var innerRow1 = CreateBuilder();
            var innerRow2 = CreateBuilder();

            dataGrid.Rows.Add(new vmSub_DataGridRow());
            dataGrid.Rows[0].Columns.Add(new vmSub_DataGridColumn());
            dataGrid.Rows[0].Columns[0].Items.Add(new vmBlock_DataGridRowItem());
            dataGrid.Rows[0].Columns[0].Items[0].Content = CreateContentBlockVm(innerRow1, "title");
            dataGrid.Rows[0].Columns.Add(new vmSub_DataGridColumn());
            dataGrid.Rows[0].Columns[1].Items.Add(new vmBlock_DataGridRowItem());
            dataGrid.Rows[0].Columns[1].Items[0].Content = CreateContentBlockVm(innerRow2, "title2");

            StubTwoColumnWidthRow(outer, innerRow1, innerRow2);

            ActAndAssert(dataGrid, outer);
        }

        [Test]
        public void DataGrid_column_config()
        {
            CreateData();

            SectionProperties(properties, AssignSections);

            var outer = CreateBuilder();
            var innerRowContent = CreateBuilder();
            var innerRowSettings = CreateBuilder();

            dataGrid.Rows.Add(new vmSub_DataGridRow());
            dataGrid.Rows[0].Columns.Add(new vmSub_DataGridColumn());
            dataGrid.Rows[0].Columns[0].Items.Add(new vmBlock_DataGridRowItem());
            dataGrid.Rows[0].Columns[0].Items[0].Content = CreateContentBlockVm(innerRowContent, "title");
            dataGrid.Rows[0].Columns[0].Config = CreateSettingsAsContentBlockVm(innerRowSettings, "hero");

            StubFullWidthRow(outer, innerRowContent, innerRowSettings);

            ActAndAssert(dataGrid, outer);
        }

        public BlockListDbModelBuilder CreateBuilder()
        {
            return new BlockListDbModelBuilder(umb, names, container);
        }

        public object CreateContentBlockVm(BlockListDbModelBuilder builder, string value)
        {
            var vm = new vmBlock_ContentBlock() { Title = value };
            StubContentBlock<vmBlock_ContentBlock>(builder, x => x.Title, value);
            return vm;
        }

        public object CreateSettingsAsContentBlockVm(BlockListDbModelBuilder builder, string value)
        {
            var vm = new vmBlock_SettingsBlock() { Id = value };
            StubContentBlock<vmBlock_SettingsBlock>(builder, x => x.Id, value);
            return vm;
        }

        public object CreateSettingsBlockVm(BlockListDbModelBuilder builder, string value)
        {
            var vm = new vmBlock_SettingsBlock() { Id = value };
            StubSettingsBlock<vmBlock_SettingsBlock>(builder, x => x.Id, value);
            return vm;
        }

        public void StubFullWidthRow(BlockListDbModelBuilder outer, BlockListDbModelBuilder innerContent, BlockListDbModelBuilder innerSettings = null)
        {
            outer.AddContentData("fullWidthSection");
            outer.AddContentProperty("w100", innerContent.Expected);
            if(innerSettings != null)
            {
                outer.AddContentProperty("w100Settings", innerSettings.Expected);
            }
        }

        public void StubTwoColumnWidthRow(BlockListDbModelBuilder outer, params BlockListDbModelBuilder[] inner)
        {
            outer.AddContentData("twoColumnSection");
            outer.AddContentProperty("l50", inner[0].Expected);
            outer.AddContentProperty("r50", inner[1].Expected);
        }

        public void StubContentBlock<V>(BlockListDbModelBuilder builder, Expression<Func<V, object>> property, object value)
        {
            builder.AddContentData<V>()
                .AddContentProperty<V>(property, value);
        }

        public void StubSettingsBlock<V>(BlockListDbModelBuilder builder, Expression<Func<V, object>> property, object value)
        {
            builder.AddSettingsData<V>()
                .AddSettingsProperty<V>(property, value);
        }

        public void ActAndAssert(object actual, BlockListDbModelBuilder expected)
        {
            var output = svc.GetImportValue(null, config, actual.ToJson(), "source", contentMapperFactory, contentImportPropertyService)
                .ToObject<BlockListDbModel>();

            Approvals.AssertEquals(expected.Expected.ToJson(), output.ToJson());
        }

        public void AssignSections(string section, List<BlockListGridRowConfigToContent.Property> sectionProperties, string colConfig = null)
        {
            umb.ContentType.ForUpdating(section);
            foreach (var p in sectionProperties)
            {
                if (!p.IsSettings)
                    umb.PropertyType.ForCreating(section, p.Name, alias: p.Alias);
                if (p.IsSettings && colConfig != null)
                    umb.PropertyType.ForCreating(section, p.Name, alias: p.Alias);
            }
        }

        public void SectionProperties(BlockListGridRowConfigToContent.Property[] properties, Action<string, List<BlockListGridRowConfigToContent.Property>, string> sectionAction, string rowConfig = null, string colConfig = null)
        {
            var sections = properties.Select(x => x.Type).Distinct();

            foreach (var section in sections)
            {
                var sectionProperties = properties.Where(x => x.Type == section).ToList();
                sectionAction(section, sectionProperties, colConfig);
            }
        }

        public void CreateData(bool defaultGridRows = true)
        {
            config.IsGrid = true;
            config.Grid.OfType = "Test";
            config.Grid.Sizes = umb.ImportConfig.GridRowConfigs.SelectMany(x => x.DefinedSizes).ToArray();


            if (defaultGridRows)
                umb.ImportConfig.GridRowConfigs.Add(new GridRowConfig(true, "fullWidthSection", "100", "100"));
            properties = gridRowConfigToContent.GetProperties(umb.ImportConfig.GridRowConfigs.ToArray());
        }

        public class vmBlock_ContentBlock
        {
            public vmBlock_ContentBlock() { _ref = "/parContentBlock"; }

            [JsonProperty("title")]
            public string Title { get; set; }

            public string _ref { get; set; }
        }

        public class vmBlock_SettingsBlock
        {
            public vmBlock_SettingsBlock() { _ref = "/parSettingsBlock"; }

            [JsonProperty("id")]
            public string Id { get; set; }
            public string _ref { get; set; }
        }

    }
}
