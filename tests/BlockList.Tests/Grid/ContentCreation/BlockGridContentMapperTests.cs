using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using VerifyNUnit;
using YuzuDelivery.Umbraco.BlockList.Grid.ContentCreation;
using YuzuDelivery.Umbraco.BlockList.Tests.Extensions;
using YuzuDelivery.Umbraco.BlockList.Tests.Stubs;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;

namespace YuzuDelivery.Umbraco.BlockList.Tests.Grid.ContentCreation;

public class BlockGridContentMapperTests : BaseTestSetup
{
    private const int SimpleGridDataTypeId = 42;

    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        BaseFixtureSetup();
    }

    [SetUp]
    public void Setup()
    {
        BaseSetup(new BlockListTestBuilder());

        var guidFactory = container.Resolve<ConsistentGuidFactoryStub>();

        var rowKey = guidFactory.CreateNew();
        var rowSettingsKey = guidFactory.CreateNew();

        var columnKey = guidFactory.CreateNew();
        var columnSettingsKey = guidFactory.CreateNew();

        umb.ContentType.ForUpdating("gridRow", key: rowKey);
        umb.ContentType.ForUpdating("gridColumn", key: columnKey);

        umb.Config.AddViewModel<vmBlock_ContentBlock>();
        umb.ContentType.ForUpdating<vmBlock_ContentBlock>(key: guidFactory.CreateNew())
            .PropertyType.ForUpdating<vmBlock_ContentBlock>(x => x.Title, groupName: "Content", key: guidFactory.CreateNew());

        umb.Config.AddViewModel<vmBlock_SettingsBlock>();
        umb.ContentType.ForUpdating<vmBlock_SettingsBlock>(key: guidFactory.CreateNew())
            .PropertyType.ForUpdating<vmBlock_SettingsBlock>(x => x.Id, groupName: "Content", key: guidFactory.CreateNew());

        umb.DataType.AddAndStubGet(SimpleGridDataTypeId, "simpleGrid", Constants.PropertyEditors.Aliases.BlockGrid);
        umb.DataType.Added["simpleGrid"].Configuration.Returns(new BlockGridConfiguration
        {
            Blocks = new []
            {
                new BlockGridConfiguration.BlockGridBlockConfiguration
                {
                    ContentElementTypeKey = rowKey,
                    SettingsElementTypeKey = rowSettingsKey,
                    Areas = new []
                    {
                        new BlockGridConfiguration.BlockGridAreaConfiguration
                        {
                            Key = guidFactory.CreateNew()
                        }
                    }
                },
                new BlockGridConfiguration.BlockGridBlockConfiguration
                {
                    ContentElementTypeKey = columnKey,
                    SettingsElementTypeKey = columnSettingsKey,
                    Areas = new []
                    {
                        new BlockGridConfiguration.BlockGridAreaConfiguration
                        {
                            Key = guidFactory.CreateNew()
                        }
                    }
                }
            }
        });
    }

    [Test]
    [TestCase(Constants.PropertyEditors.Aliases.BlockGrid, true, true)]
    [TestCase(Constants.PropertyEditors.Aliases.BlockGrid, false, false)]
    [TestCase(Constants.PropertyEditors.Aliases.Label, true, false)]
    public void IsValid_VariousInputs_ReturnsExpectedValue(string editor, bool isGrid, bool expected)
    {
        var sut = container.Resolve<BlockGridContentMapper>();
        var config = new ContentPropertyConfig { IsGrid = isGrid };

        sut.IsValid(editor, config).Should().Be(expected);
    }

    [Test]
    public Task GetImportValue_RowWithSettings_RendersCorrectly()
    {
        var config = new ContentPropertyConfig
        {
            Grid =
            {
                RowConfigOfType = "vmBlock_SettingsBlock"
            }
        };
        var sut = container.Resolve<BlockGridContentMapper>();
        var contentMapperFactory = container.Resolve<IContentMapperFactory>();
        var contentImportPropertyService = container.Resolve<IContentImportPropertyService>();

        var model = new vmBlock_DataGrid();

        model.AddRow().WithSettings(219);

        var mapping = GridBuildingExtensions.PropertyLinkForEditor(SimpleGridDataTypeId);

        var result = sut.GetImportValue(mapping, config, model.ToJson(), "source", contentMapperFactory, contentImportPropertyService);

        return Verifier.Verify(result);
    }

    [Test]
    public Task GetImportValue_ColWithSettings_RendersCorrectly()
    {
        var config = new ContentPropertyConfig
        {
            Grid =
            {
                ColumnConfigOfType = "vmBlock_SettingsBlock"
            }
        };
        var sut = container.Resolve<BlockGridContentMapper>();
        var contentMapperFactory = container.Resolve<IContentMapperFactory>();
        var contentImportPropertyService = container.Resolve<IContentImportPropertyService>();

        var model = new vmBlock_DataGrid();

        model.AddRow().AddCol().WithSettings(219);

        var mapping = GridBuildingExtensions.PropertyLinkForEditor(SimpleGridDataTypeId);

        var result = sut.GetImportValue(mapping, config, model.ToJson(), "source", contentMapperFactory, contentImportPropertyService);

        return Verifier.Verify(result);
    }

    [Test]
    public Task GetImportValue_SingleItem_RendersCorrectly()
    {
        var config = new ContentPropertyConfig();
        var sut = container.Resolve<BlockGridContentMapper>();
        var contentMapperFactory = container.Resolve<IContentMapperFactory>();
        var contentImportPropertyService = container.Resolve<IContentImportPropertyService>();

        var model = new vmBlock_DataGrid();

        model.AddRow()
            .AddCol()
            .AddItem("a");

        var mapping = GridBuildingExtensions.PropertyLinkForEditor(SimpleGridDataTypeId);

        var result = sut.GetImportValue(mapping, config, model.ToJson(), "source", contentMapperFactory, contentImportPropertyService);

        return Verifier.Verify(result);
    }

    [Test]
    public Task GetImportValue_SingleColWithMultipleItems_RendersCorrectly()
    {
        var config = new ContentPropertyConfig();
        var sut = container.Resolve<BlockGridContentMapper>();
        var contentMapperFactory = container.Resolve<IContentMapperFactory>();
        var contentImportPropertyService = container.Resolve<IContentImportPropertyService>();

        var model = new vmBlock_DataGrid();

        model.AddRow()
            .AddCol()
            .AddItem("a")
            .AddItem("b");

        var mapping = GridBuildingExtensions.PropertyLinkForEditor(SimpleGridDataTypeId);

        var result = sut.GetImportValue(mapping, config, model.ToJson(), "source", contentMapperFactory, contentImportPropertyService);

        return Verifier.Verify(result);
    }

    [Test]
    public Task GetImportValue_MultipleRowsAndColumns_RendersCorrectly()
    {
        var config = new ContentPropertyConfig();
        var sut = container.Resolve<BlockGridContentMapper>();
        var contentMapperFactory = container.Resolve<IContentMapperFactory>();
        var contentImportPropertyService = container.Resolve<IContentImportPropertyService>();

        var model = new vmBlock_DataGrid();

        var row1 = model.AddRow();
        row1.AddCol().WithSize(6)
            .AddItem("a");
        row1.AddCol().WithSize(6)
            .AddItem("b");

        var row2 = model.AddRow();
        row2.AddCol().WithSize(4)
            .AddItem("a");
        row2.AddCol().WithSize(4)
            .AddItem("b");
        row2.AddCol().WithSize(4)
            .AddItem("c");

        var mapping = GridBuildingExtensions.PropertyLinkForEditor(SimpleGridDataTypeId);

        var result = sut.GetImportValue(mapping, config, model.ToJson(), "source", contentMapperFactory, contentImportPropertyService);

        return Verifier.Verify(result);
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
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
