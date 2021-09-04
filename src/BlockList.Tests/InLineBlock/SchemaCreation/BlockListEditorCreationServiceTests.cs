using System.Text;
using System.Threading.Tasks;
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

namespace YuzuDelivery.Umbraco.BlockList.Tests.Inline
{
    [Category("BlockListEditor")]
    [UseReporter(typeof(DiffReporter))]
    public class BlockListEditorCreationServiceTests : BaseTestSetup
    {
        public BlockListDataTypeFactory dataTypeFactory;
        public BlockListEditorCreationService svc;

        public BlockListConfigurationBuilder builder;
        public VmToContentPropertyMapBuilder propertyMapBuilder;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            BaseFixtureSetup();
        }

        [SetUp]
        public void Setup()
        {
            BaseSetup(new BlockListTestBuilder());

            dataTypeFactory = container.Resolve<BlockListDataTypeFactory>();
            svc = container.Resolve<BlockListEditorCreationService>();

            builder = new BlockListConfigurationBuilder(umb);
            propertyMapBuilder = new VmToContentPropertyMapBuilder(umb, names);
            propertyMapBuilder.AddGroup("Content");

            umb.Config.AddViewModel<vmBlock_Test>();
            umb.ContentType.ForCreating<vmBlock_Test>();

            umb.Config.AddViewModel<vmBlock_ProductSummary>();
            umb.ContentType.ForCreating<vmBlock_ProductSummary>();

            umb.Config.AddViewModel<vmBlock_Test2>();
            umb.ContentType.ForCreating<vmBlock_Test2>();

        }

        [TestCase("Test", "Test", false, false, TestName = "Name - Can create one word child subblock name")]
        [TestCase("ProductSummary", "Product Summary", false, false, TestName = "Name - Can create camel case two word sublock name")]
        [TestCase("ProductSummary", "Product Summaries", true, false, TestName = "Name - Can create camel case two word list of subblocks name")]
        [TestCase("Test", "Test", false, true, TestName = "Name - Can create any of subBlocks name")]
        [TestCase("Test", "Tests", true, true, TestName = "Name - Can create any of list of subBlocks name")]
        [Test]
        public void datatype_names(string assignedName, string expectedName, bool isList, bool isAnyOf)
        {
            var assignedName2 = $"{assignedName}2";

            CreateData(isList, isAnyOf, assignedName, assignedName2);

            umb.DataType.AddAndStubCreate(1, expectedName, BlockListDataTypeFactory.DataEditorName);

            var output = svc.Create(propertyMapBuilder.CurrentProperty);

            Assert.AreEqual(expectedName, output.Name);
        }


        [TestCase("Test", "Test", "Test", false, false, TestName = "DataType - Can create subBlock block list")]
        [TestCase("ProductSummary", "Product Summary", "Product Summaries", true, false, TestName = "DataType - Can create sentence case two word sublock block list label")]
        [TestCase("ProductSummary", "Product Summary", "Product Summaries", true, false, TestName = "DataType - Can create list of subBlock block list")]
        [TestCase("Test", "Test", "Test", false, true, TestName = "DataType - Can create anyOf subBlock block list")]
        [TestCase("Test", "Test", "Tests", true, true, TestName = "DataType - Can create list of anyOf subBlock block list")]
        [Test]
        public void datatype(string assignedName, string expectedName, string dataTypeName, bool isList, bool isAnyOf)
        {
            var assignedName2 = $"{assignedName}2";

            CreateData(isList, isAnyOf, assignedName, assignedName2);

            umb.DataType.AddAndStubCreate(1, dataTypeName, BlockListDataTypeFactory.DataEditorName);

            builder.AddBlock(expectedName, assignedName, customView: string.Empty);
            if (isAnyOf) builder.AddBlock(assignedName2, assignedName2, customView: string.Empty);

            builder.Expected.UseInlineEditingAsDefault = true;
            builder.Expected.ValidationLimit.Min = 0;

            if (!isList)
                builder.Expected.ValidationLimit.Max = 1;

            var output = svc.Create(propertyMapBuilder.CurrentProperty).Configuration as BlockListConfiguration;

            Approvals.AssertEquals(builder.Expected.ToJson(), output.ToJson());

            umb.ContentType.WasCreated(assignedName);
            if (isAnyOf) umb.ContentType.WasCreated(assignedName2);
        }

        public void CreateData(bool isList, bool isAnyOf, params string[] blocks)
        {
            var map = new VmToContentPropertyMap();
            map.Config.IsList = isList;

            if (isAnyOf)
            {
                map.VmPropertyName = blocks.FirstOrDefault();
                map.Config.AllowedTypes = blocks;
            }
            else
                map.Config.TypeName = $"vmBlock_{blocks.FirstOrDefault()}";

            propertyMapBuilder.AddProperty(map);
        }

        public class vmBlock_Test
        { }

        public class vmBlock_ProductSummary
        { }

        public class vmBlock_Test2
        { }

    }
}
