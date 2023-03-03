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

        [TestCase(false)]
        [TestCase(true)]
        public Task Create_Always_CreatesDefaultContainers(bool hasColumns)
        {
            if(hasColumns)
            {
                umb.ContentType.ForCreating("Grid Column");
            }
            umb.ContentType.ForCreating("Grid Row");
            umb.DataType.AddAndStubCreate(1, "Grid Builder", Constants.PropertyEditors.Aliases.BlockGrid);

            var map = new VmToContentPropertyMap
            {
                Config = new ContentPropertyConfig
                {
                    Grid = new ContentPropertyConfigGrid
                    {
                        OfType = "gridBuilder",
                        HasColumns = hasColumns
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            if(hasColumns)
            {
                umb.ContentType.WasCreated("Grid Column");
                result.GridConfig().Blocks[1].ContentElementTypeKey.Should().Be(umb.ContentType.Current.TypesByAlias["gridColumn"].Key);
            }
            umb.ContentType.WasCreated("Grid Row");
            result.GridConfig().Blocks[0].ContentElementTypeKey.Should().Be(umb.ContentType.Current.TypesByAlias["gridRow"].Key);

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
                        RowConfigOfType = "myRowSettings",
                        HasColumns = true
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            umb.ContentType.WasCreated("My Row Settings");

            result.GridConfig().Blocks[0].SettingsElementTypeKey.Should().Be(umb.ContentType.Current.TypesByAlias["myRowSettings"].Key);

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
                        ColumnConfigOfType = "myColumnSettings",
                        HasColumns = true
                    }
                }
            };

            var sut = container.Resolve<BlockGridCreationService>();

            var result = sut.Create(map);

            umb.ContentType.WasCreated("My Column Settings");

            result.GridConfig().Blocks[1].SettingsElementTypeKey.Should().Be(umb.ContentType.Current.TypesByAlias["myColumnSettings"].Key);

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
                        HasColumns = true,
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
                        HasColumns= true,
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

    public static class Extensions
    {
        public static BlockGridConfiguration GridConfig(this IDataType dataType)
        {
            return dataType.Umb().ConfigurationAs<BlockGridConfiguration>();
        }
    }
}
