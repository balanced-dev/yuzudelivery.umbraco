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
using FluentAssertions;
using Umbraco.Cms.Core;
using VerifyNUnit;

namespace YuzuDelivery.Umbraco.BlockList.Tests.Grid
{
    [Category("BlockGrid")]
    [UseReporter(typeof(DiffReporter))]
    public class BlockGridCreationServiceTests : BaseTestSetup
    {

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            BaseFixtureSetup();
        }

        [SetUp]
        public void Setup()
        {
            BaseSetup(new BlockListTestBuilder());
        }

        [Test]
        public void Create_WithMissingGridConfig_Throws()
        {
            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = null
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();
            var act = () => sut.Create(map);

            act.Should().Throw<ArgumentException>().WithMessage("Grid configuration missing *");
        }

        [Test]
        public void Update_WithMissingGridConfig_Throws()
        {
            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = null
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();
            var act = () => sut.Update(map, Substitute.For<IDataType>());

            act.Should().Throw<ArgumentException>().WithMessage("Grid configuration missing *");
        }

        [Test]
        public void Create_WithGridConfig_ExtractsAppropriateNameFromGridConfigOfType()
        {
            umb.ContentType.ForCreating("Grid Column");
            umb.ContentType.ForCreating("Grid Row");
            umb.DataType.AddAndStubCreate(1, "Foo Bar Baz", Constants.PropertyEditors.Aliases.BlockGrid);

            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = new ContentPropertyConfigGrid
                    {
                        OfType = "fooBarBaz"
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            result.Name.Should().Be("Foo Bar Baz");
        }

        [Test]
        public Task Create_Always_CreatesDefaultContainers()
        {
            umb.ContentType.ForCreating("Grid Column");
            umb.ContentType.ForCreating("Grid Row");
            umb.DataType.AddAndStubCreate(1, "Grid Builder", Constants.PropertyEditors.Aliases.BlockGrid);

            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = new ContentPropertyConfigGrid
                    {
                        OfType = "gridBuilder"
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            return Verifier.Verify(result.Umb().Configuration);
        }

        [Test]
        public Task Create_WithConfiguredRowConfigOfType_AssociatesSettingsElementWithRow()
        {
            umb.ContentType.ForCreating("Grid Column");
            umb.ContentType.ForCreating("Grid Row");
            umb.ContentType.ForCreating("My Row Settings");

            umb.DataType.AddAndStubCreate(1, "Grid Builder", Constants.PropertyEditors.Aliases.BlockGrid);

            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = new ContentPropertyConfigGrid
                    {
                        OfType = "gridBuilder",
                        RowConfigOfType = "myRowSettings"
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            return Verifier.Verify(result.Umb().Configuration);
        }

        [Test]
        public Task Create_WithConfiguredColumnConfigOfType_AssociatesSettingsElementWithColumn()
        {
            umb.ContentType.ForCreating("Grid Column");
            umb.ContentType.ForCreating("Grid Row");
            umb.ContentType.ForCreating("My Column Settings");

            umb.DataType.AddAndStubCreate(1, "Grid Builder", Constants.PropertyEditors.Aliases.BlockGrid);

            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = new ContentPropertyConfigGrid
                    {
                        OfType = "gridBuilder",
                        ColumnConfigOfType = "myColumnSettings"
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            return Verifier.Verify(result.Umb().Configuration);
        }

        [Test]
        public Task Create_WithConfiguredAllowedBlocks_AddsBlocksGridDataType()
        {
            umb.ContentType.ForCreating("Grid Column");
            umb.ContentType.ForCreating("Grid Row");
            umb.ContentType.ForCreating("My Block Type");
            umb.ContentType.ForCreating("My Alternate Block Type");

            umb.DataType.AddAndStubCreate(1, "Grid Builder", Constants.PropertyEditors.Aliases.BlockGrid);

            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = new ContentPropertyConfigGrid
                    {
                        OfType = "gridBuilder",

                    },
                    AllowedTypes = new []
                    {
                        "myBlockType",
                        "myAlternateBlockType"
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            return Verifier.Verify(result.Umb().Configuration);
        }

        [Test]
        public Task Update_WithAdditionalBlocks_UpdatesBlocksGridDataType()
        {
            umb.ContentType.ForCreating("Grid Column");
            umb.ContentType.ForCreating("Grid Row");
            umb.ContentType.ForCreating("My Block Type");
            umb.ContentType.ForCreating("My Added Block Type");


            umb.DataType.AddAndStubCreate(1, "Grid Builder", Constants.PropertyEditors.Aliases.BlockGrid);

            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = new ContentPropertyConfigGrid
                    {
                        OfType = "gridBuilder",

                    },
                    AllowedTypes = new []
                    {
                        "myBlockType"
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var original = sut.Create(map);

            map.Config.AllowedTypes = new[]
            {
                "myBlockType",
                "myAddedBlockType"
            };

            var updated = sut.Update(map, original);

            return Verifier.Verify(updated.Umb().Configuration);
        }
    }
}
