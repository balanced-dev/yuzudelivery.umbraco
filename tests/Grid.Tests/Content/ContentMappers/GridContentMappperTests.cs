using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mod = Umbraco.Cms.Core.Models;
using System.Reflection;
using ApprovalTests;
using ApprovalTests.Reporters;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;
using Microsoft.Extensions.Hosting;

namespace YuzuDelivery.Umbraco.Grid.Test
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class GridContentMappperTests
    {
        public IVmHelperService vmHelperService;
        public IDataTypeService dataTypeService;
        public IDTGEService dgteService;
        public IContentImportMergedService contentImportMergedService;
        public IYuzuDeliveryImportConfiguration importConfig;

        public IContentMapperFactory contentMapperFactory;
        public IContentImportPropertyService contentImportPropertyService;

        public GridContentMapper svc;

        public VmContentMapping vmContentMapping;
        public ContentPropertyConfig config;

        public VmToContentPropertyLink mapping;
        public List<VmToContentPropertyLink> mappings;

        public IDataType dataType;

        public IContentType contentType;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            YuzuConstants.Reset();
            YuzuConstants.Initialize(new YuzuConstantsConfig());
        }

        [SetUp]
        public void Setup()
        {
            vmHelperService = Substitute.For<IVmHelperService>();
            dataTypeService = Substitute.For<IDataTypeService>();
            dgteService = Substitute.For<IDTGEService>();
            contentImportMergedService = Substitute.For<IContentImportMergedService>();
            importConfig = new YuzuDeliveryImportConfiguration(Substitute.For<IHostEnvironment>(), new List<IUpdateableImportConfiguration>());

            contentImportPropertyService = Substitute.For<IContentImportPropertyService>();
            contentMapperFactory = Substitute.For<IContentMapperFactory>();

            config = new ContentPropertyConfig();

            contentType = Substitute.For<IContentType>();

            mapping = new VmToContentPropertyLink();
            mapping.CmsPropertyType = new InternalPropertyType();
            mapping.CmsPropertyType.Id = 144;
            mappings = new List<VmToContentPropertyLink>();

            vmContentMapping = new VmContentMapping();
            vmContentMapping.ContentType = contentType;
            vmContentMapping.Mappings = mappings;

            var umbDataType = Substitute.For<Mod.IDataType>();
            umbDataType.Name = "gridDataType";

            dataType = new DataType(umbDataType);

            dgteService.GetByName(dataType.Name).Returns(JObject.FromObject(new
            {
                name = mapping.CmsPropertyType.Name
            }));


            importConfig.GridRowConfigs.Clear();
            importConfig.GridRowConfigs.Add(new GridRowConfig("100", "12", "12", isDefault: true));
            importConfig.GridRowConfigs.Add(new GridRowConfig("50/50", "6", "6,6"));

            svc = Substitute.ForPartsOf<GridContentMapper>(vmHelperService, dataTypeService, dgteService, contentImportMergedService, importConfig);           
            svc.GetGuid().Returns("testGuid");

            dataTypeService.Get(mapping.CmsPropertyType.DataTypeId).Returns(dataType);

            Action<string, string, VmToContentPropertyLink, string, Action<string, string>> importProperty = (vmName, source, mapping, value, setValue) =>
            {
                if (mapping != null && value != null)
                    setValue(mapping.CmsPropertyType.Alias, value);
                else
                    setValue(mapping.CmsPropertyType.Alias, null);
            };
            contentImportPropertyService.WhenForAnyArgs(x => x.ImportProperty(null, null, null, null, null))
                .Do(x => importProperty(x.ArgAt<string>(0), x.ArgAt<string>(1), x.ArgAt<VmToContentPropertyLink>(2), x.ArgAt<string>(3), x.ArgAt<Action<string, string>>(4)));
        }

        [Test]
        public void ImportValue_empty_grid_with_rows()
        {
            var content = new
            {
                rows = new object[]
                {
                    new
                    {
                        items = new object[] { }
                    }
                }
            };

            var output = svc.GetImportValue(mapping, config, JsonConvert.SerializeObject(content), "", contentMapperFactory, contentImportPropertyService);

            var rows = new object[]
                {
                    new
                    {
                        name = "100",
                        areas = new object[]
                        {
                            new
                            {
                                grid = 12,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = false,
                                config = new object[] { },
                                controls = new object[] { }
                            }
                        },
                        style = new { },
                        hasConfig = false,
                        config = new { },
                        id = "testGuid"
                    }
                };

            var expected = AddDefaultSection(rows);
            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);
        }

        [Test]
        public void ImportValue_empty_grid_with_columns()
        {
            var content = new
            {
                rows = new object[]
                {
                    new
                    {
                        columns = new object[]
                        {
                            new
                            {
                                gridSize = 12,
                                items = new object[] { }
                            }
                        }
                    }
                }
            };

            var output = svc.GetImportValue(mapping, config, JsonConvert.SerializeObject(content), "", contentMapperFactory, contentImportPropertyService);

            var rows = new object[]
                {
                    new
                    {
                        name = "100",
                        areas = new object[]
                        {
                            new
                            {
                                grid = 12,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = false,
                                config = new { },
                                controls = new object[] { }
                            }
                        },
                        style = new { },
                        hasConfig = false,
                        config = new { },
                        id = "testGuid"
                    }
                };

            var expected = AddDefaultSection(rows);
            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);
        }

        [Test]
        public void ImportValue_row_config()
        {
            var content = new
            {
                rows = new object[]
                {
                    new
                    {
                        config = new {
                           backgroundTheme = "",
                           removePaddingTop = false
                        },
                        items = new object[] { }
                    }
                }
            };

            var output = svc.GetImportValue(mapping, config, JsonConvert.SerializeObject(content), "", contentMapperFactory, contentImportPropertyService);

            var rows = new object[]
                {
                    new
                    {
                        name = "100",
                        areas = new object[]
                        {
                            new
                            {
                                grid = 12,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = false,
                                config = new object[] { },
                                controls = new object[] { }
                            }
                        },
                        style = new { },
                        hasConfig = true,
                        config = new {
                           backgroundTheme = "",
                           removePaddingTop = false
                        },
                        id = "testGuid"
                    }
                };

            var expected = AddDefaultSection(rows);
            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);
        }

        [Test]
        public void ImportValue_column_config()
        {
            var content = new
            {
                rows = new object[]
                {
                    new
                    {
                        columns = new object[]
                        {
                            new
                            {
                                gridSize = 12,
                                config = new {
                                   backgroundTheme = "",
                                   removePaddingTop = false
                                },
                                items = new object[] { }
                            }
                        }
                    }
                }
            };

            var output = svc.GetImportValue(mapping, config, JsonConvert.SerializeObject(content), "", contentMapperFactory, contentImportPropertyService);

            var rows = new object[]
                {
                    new
                    {
                        name = "100",
                        areas = new object[]
                        {
                            new
                            {
                                grid = 12,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = true,
                                config = new {
                                   backgroundTheme = "",
                                   removePaddingTop = false
                                },
                                controls = new object[] { }
                            }
                        },
                        style = new { },
                        hasConfig = false,
                        config = new { },
                        id = "testGuid"
                    }
                };

            var expected = AddDefaultSection(rows);
            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);
        }

        [Test]
        public void ImportValue_multiple_columns()
        {
            var content = new
            {
                rows = new object[]
                {
                    new
                    {
                        columns = new object[]
                        {
                            new
                            {
                                gridSize = 6,
                                config = new {},
                                items = new object[] { }
                            },
                            new
                            {
                                gridSize = 6,
                                config = new {},
                                items = new object[] { }
                            }
                        }
                    }
                }
            };

            var output = svc.GetImportValue(mapping, config, JsonConvert.SerializeObject(content), "", contentMapperFactory, contentImportPropertyService);

            var rows = new object[]
                {
                    new
                    {
                        name = "50/50",
                        areas = new object[]
                        {
                            new
                            {
                                grid = 6,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = true,
                                config = new {},
                                controls = new object[] { }
                            },
                            new
                            {
                                grid = 6,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = true,
                                config = new {},
                                controls = new object[] { }
                            }
                        },
                        style = new { },
                        hasConfig = false,
                        config = new { },
                        id = "testGuid"
                    }
                };

            var expected = AddDefaultSection(rows);
            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);
        }

        [Test]
        public void ImportValue_parse_item_to_control_value()
        {
            StubVmLink("vmBlock_Test", "test");
            CreateImportData(new string[] { "title", "text" });

            var content = new
            {
                rows = new object[]
                {
                    new
                    {
                        columns = new object[]
                        {
                            new
                            {
                                gridSize = 12,
                                config = new {},
                                items = new object[] {
                                    new {
                                        title = "Learn From The Expert",
                                        text = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Maxime ipsa nulla sed quis rerum amet natus quas necessitatibus.",
                                        _ref = "/parTest"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var output = svc.GetImportValue(mapping, config, JsonConvert.SerializeObject(content), "", contentMapperFactory, contentImportPropertyService);

            var rows = new object[]
                {
                    new
                    {
                        name = "100",
                        areas = new object[]
                        {
                            new
                            {
                                grid = 12,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = true,
                                config = new {},
                                controls = new object[] {
                                    new {
                                        value = new
                                        {
                                            dtgeContentTypeAlias = "test",
                                            value = new {
                                                title = "Learn From The Expert",
                                                text = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Maxime ipsa nulla sed quis rerum amet natus quas necessitatibus.",
                                            },
                                            id = "testGuid"
                                        },
                                        editor = new {
                                            name = mapping.CmsPropertyType.Alias
                                        }
                                    }
                                }
                            }
                        },
                        style = new { },
                        hasConfig = false,
                        config = new { },
                        id = "testGuid"
                    }
                };

            var expected = AddDefaultSection(rows);
            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);
        }

        [Test]
        public void ImportValue_parse_jsonin_child_property_to_control_value()
        {
            var dgteConfig = new
            {
                name = mapping.CmsPropertyType.Name
            };

            StubVmLink("vmBlock_Test", "test");
            CreateImportData(new string[] { "title", "obj", "objSubItem", "arraySubItem", "grandChildSubItem" });
            dgteService.GetByName(dataType.Name).Returns(JObject.FromObject(dgteConfig));

            var jsonObject = JsonConvert.SerializeObject(new { title = "subObject" });
            var childJsonObj = JsonConvert.SerializeObject(new { obj = jsonObject });
            var childJsonArray = JsonConvert.SerializeObject(new object[] { new { array = jsonObject } });
            var grandChildJsonObj = JsonConvert.SerializeObject(new { obj = childJsonObj });

            var content = new
            {
                rows = new object[]
                {
                    new
                    {
                        columns = new object[]
                        {
                            new
                            {
                                gridSize = 12,
                                config = new {},
                                items = new object[] {
                                    new {
                                        title = "Learn From The Expert",
                                        obj = jsonObject,
                                        objSubItem = childJsonObj,
                                        arraySubItem = childJsonArray,
                                        grandChildSubItem = grandChildJsonObj,
                                        _ref = "/parTest"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var output = svc.GetImportValue(mapping, config, JsonConvert.SerializeObject(content), "", contentMapperFactory, contentImportPropertyService);

            var rows = new object[]
                {
                    new
                    {
                        name = "100",
                        areas = new object[]
                        {
                            new
                            {
                                grid = 12,
                                allowAll = false,
                                allowed = new string[] { "docType" },
                                hasConfig = true,
                                config = new {},
                                controls = new object[] {
                                    new {
                                        value = new
                                        {
                                            dtgeContentTypeAlias = "test",
                                            value = new {
                                                title = "Learn From The Expert",
                                                obj = new
                                                {
                                                    title = "subObject"
                                                },
                                                objSubItem = new {
                                                    obj = new
                                                    {
                                                        title = "subObject"
                                                    }
                                                },
                                                arraySubItem = new object[]
                                                {
                                                    new {
                                                        array = new
                                                        {
                                                            title = "subObject"
                                                        }
                                                    }
                                                },
                                                grandChildSubItem = new {
                                                    obj = new
                                                    {
                                                        obj = new
                                                        {
                                                            title = "subObject"
                                                        }
                                                    }
                                                },
                                            },
                                            id = "testGuid"
                                        },
                                        editor = new {
                                            name = mapping.CmsPropertyType.Alias
                                        }
                                    }
                                }
                            }
                        },
                        style = new { },
                        hasConfig = false,
                        config = new { },
                        id = "testGuid"
                    }
                };

            var expected = AddDefaultSection(rows);
            Approvals.AssertEquals(JsonConvert.SerializeObject(expected, Formatting.Indented), output);
        }

        public object AddDefaultSection(object[] rows)
        {
            return new
            {
                name = "1 column layout",
                sections = new object[]
                {
                    new
                    {
                        grid = 12,
                        allowAll = true,
                        rows = rows
                    }
                }
            };
        }

        public void StubVmLink(string vmName, string documentTypeAlias)
        {
            svc.GetDocumentTypeAlias(vmContentMapping).Returns(documentTypeAlias);
            vmHelperService.GetWithMapping(vmName).Returns(vmContentMapping);
        }

        public void CreateImportData(string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                mappings.Add(new VmToContentPropertyLink()
                {
                    VmProperty = CreatePropertyInfo(propertyName),
                    CmsPropertyType = new InternalPropertyType()
                    {
                        Alias = propertyName
                    }
                });
            }
        }

        public PropertyInfo CreatePropertyInfo(string name, bool stubJsonPropertyName = true)
        {
            var p = Substitute.For<PropertyInfo>();
            p.Name.Returns(name);
            if (stubJsonPropertyName)
                svc.Configure().GetJsonPropertyName(p).Returns(name);
            return p;
        }

    }
}
