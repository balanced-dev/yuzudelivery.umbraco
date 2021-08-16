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
    public class BlockListContentMapperTests : BaseTestSetup
    {
        public IContentMapperFactory contentMapperFactory;
        public IContentImportPropertyService contentImportPropertyService;

        public BlockListContentMapper svc;

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

            svc = container.Resolve<BlockListContentMapper>();
            contentMapperFactory = container.Resolve<IContentMapperFactory>();
            contentImportPropertyService = container.Resolve<IContentImportPropertyService>();

            propertyMapBuilder = new VmToContentPropertyMapBuilder(umb, names);
            propertyMapBuilder.AddGroup("Content");

            /*umb.Config.AddViewModel<vmBlock_Test>();
            umb.ContentType.ForCreating<vmBlock_Test>();

            umb.Config.AddViewModel<vmBlock_ProductSummary>();
            umb.ContentType.ForCreating<vmBlock_ProductSummary>();

            umb.Config.AddViewModel<vmBlock_Test2>();
            umb.ContentType.ForCreating<vmBlock_Test2>();*/

        }


        public class vmBlock_Test
        { }

        public class vmBlock_ProductSummary
        { }

        public class vmBlock_Test2
        { }

    }
}
