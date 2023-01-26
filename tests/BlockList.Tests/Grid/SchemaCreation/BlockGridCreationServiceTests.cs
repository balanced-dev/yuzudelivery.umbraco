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
    public class BlockGridCreationServiceTests : BaseTestSetup
    {
       // TODO: Cover BlockGridCreationService
    }
}
