using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;
using Newtonsoft.Json;
using ApprovalTests;
using ApprovalTests.Reporters;
using Umb = Umbraco.Core.Services;
using Umbraco.Core.Logging;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Grid.Test
{
    [UseReporter(typeof(DiffReporter))]
    public class GridSchemaCreationServiceTests
    {
        public IDataTypeService dataTypeService;
        public ILogger logger;
        public IDTGEService dgteService;
        public IYuzuDeliveryImportConfiguration importConfig;

        public GridSchemaCreationService svc;

        public VmToContentPropertyMap data;
        public ContentPropertyConfig config;
        public IDataType dataType;

        public string propertyName;

        [SetUp]
        public void Setup()
        {
            dataTypeService = MockRepository.GenerateStub<IDataTypeService>();
            logger = MockRepository.GenerateStub<ILogger>();
            dgteService = MockRepository.GenerateStub<IDTGEService>();
            importConfig = new YuzuDeliveryImportConfiguration(new List<IUpdateableImportConfiguration>());

            Func<IDataType, IDataType> dataTypeAction = (dataType) => this.dataType = dataType;
            dataTypeService.Stub(x => x.Save(dataType))
                .IgnoreArguments()
                .Do(dataTypeAction);

            config = new ContentPropertyConfig();

            data = new VmToContentPropertyMap();
            data.Config = config;

            dataType = MockRepository.GenerateStub<IDataType>();

            svc = MockRepository.GeneratePartialMock<GridSchemaCreationService>(new object[] { dataTypeService, importConfig, dgteService, logger });

            Func<IDataType, IDataType> action = (d) => dataType = d;
            dataTypeService.Stub(x => x.Save(dataType)).IgnoreArguments().Do(action);
            dataTypeService.Stub(x => x.CreateDataType("Property Name", GridSchemaCreationService.DataEditorName)).Return(dataType);


        }

        [Test]
        public void Create_adds_default_single_column()
        { 
            StubGetDataTypeName();

            svc.Create(data);

            var output = GetConfigAsJson();

            var expected = new
            {
                styles = new object[] { },
                config = new object[] { },
                columns = 12,
                templates = DefaultTemplates(),
                layouts = DefaultLayouts()
            };

            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);

        }

        [Test]
        public void Create_add_multiple_columns()
        {
            StubGetDataTypeName();

            config.Grid.Sizes = new string[] { "6", "6" };

            svc.Create(data);

            var output = GetConfigAsJson();

            var expected = new
            {
                styles = new object[] { },
                config = new object[] { },
                columns = 12,
                templates = DefaultTemplates(),
                layouts = new object[] {
                    new {
                        name = "50/50",
                        areas = new object[]
                        {
                            new {
                                grid = "6",
                                allowAll = false,
                                allowed = new string[] { "propertyName" }
                            },
                            new {
                                grid = "6",
                                allowAll = false,
                                allowed = new string[] { "propertyName" }
                            }
                        }
                    }
                }
            };

            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);

        }

        [Test]
        public void Create_add_grid_config()
        {
            StubGetDataTypeName();

            config.Grid.Config = new Dictionary<string, ContentPropertyConfigGrid.ConfigValueItem>();
            config.Grid.Config.Add("someProperty", new ContentPropertyConfigGrid.ConfigValueItem() {
                Editor = "textstring",
                Row = true,
                Column = true,
                Prevalues = new string[] { "preValue" }
            });

            svc.Create(data);

            var output = GetConfigAsJson();

            var expected = new
            {
                styles = new object[] { },
                config = new object[] {
                    new
                    {
                        label = "someProperty",
                        key = "someProperty",
                        view = "textstring",
                        applyTo = "row|cell",
                        prevalues = new string[] { "preValue" }
                    }
                },
                columns = 12,
                templates = DefaultTemplates(),
                layouts = DefaultLayouts()
            };

            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);

        }

        [Test]
        public void GetDataTypeName_convert_of_type_name_removing_vm_prefix_and_adding_sentence_case()
        {
            config.Grid.OfType = "courseGrid";

            var output = svc.GetDataTypeName(data);

            Assert.AreEqual("Course Grid", output);
        }

        [Test]
        public void GetDataTypeAlias_use_of_type()
        {
            config.Grid.OfType = "courseGrid";

            var output = svc.GetDataTypeAlias(data);

            Assert.AreEqual(config.Grid.OfType, output);
        }

        public object DefaultTemplates()
        {
            return new object[] {
                new {
                    name = "1 column layout",
                    sections = new object[]
                    {
                        new {
                            grid = 12,
                            allowAll = true
                        }
                    },
                }
            };
        }

        public object DefaultLayouts()
        {
            return new object[] {
                new {
                    name = "100",
                    areas = new object[]
                    {
                        new {
                            grid = "12",
                            allowAll = false,
                            allowed = new string[] { "propertyName" }
                        }
                    }
                }
            };
        }

        public void StubGetDataTypeName()
        {
            svc.Stub(x => x.GetDataTypeName(data)).Return("Property Name");
            svc.Stub(x => x.GetDataTypeAlias(null)).IgnoreArguments().Return("propertyName");
        }

        public string GetConfigAsJson()
        {
            var dataTypeConfig = ((GridConfiguration)dataType.Configuration).Items;
            return JsonConvert.SerializeObject(dataTypeConfig, Formatting.Indented);
        }
    }
}
