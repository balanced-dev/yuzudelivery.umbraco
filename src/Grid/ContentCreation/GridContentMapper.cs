using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridContentMapper : IContentMapper
    {
        protected IVmHelperService vmHelperService;
        protected IDataTypeService dataTypeService;
        protected IDTGEService dgteService;
        protected IContentImportMergedService contentImportMergedService;
        private readonly IYuzuDeliveryImportConfiguration importConfig;

        public GridContentMapper(IVmHelperService vmHelperService, IDataTypeService dataTypeService, IDTGEService dgteService, 
            IContentImportMergedService contentImportMergedService, IYuzuDeliveryImportConfiguration importConfig)
        {
            this.vmHelperService = vmHelperService;
            this.dataTypeService = dataTypeService;
            this.dgteService = dgteService;
            this.contentImportMergedService = contentImportMergedService;
            this.importConfig = importConfig;
        }

        public bool IsValid(string propertyEditorAlias, ContentPropertyConfig config)
        {
            return propertyEditorAlias == "Umbraco.Grid";
        }

        public virtual string GetImportValue(VmToContentPropertyLink mapping, ContentPropertyConfig config, string content, string source, IContentMapperFactory factory, IContentImportPropertyService import)
        {
            var contentObj = JObject.Parse(content);

            var d = dataTypeService.Get(mapping.CmsPropertyType.DataTypeId);
            var editor = dgteService.GetByName(d.Name);

            if (editor == null)
            {
                throw new Exception("Grid editor not found in Umbraco grid.config.js");
            }

            var output = new
            {
                name = "1 column layout",
                sections = AddSections(contentObj, editor, mapping, import)
            };

            return JsonConvert.SerializeObject(output, Formatting.Indented);
        }

        public object AddSections(JObject contentObj, JObject editor, VmToContentPropertyLink mapping, IContentImportPropertyService import)
        {
            return new object[]
            {
                new
                {
                    grid = 12,
                    allowAll = true,
                    rows = AddRow(contentObj["rows"] as JArray, editor, mapping, import)
                }
            };
        }

        public object AddRow(JArray rows, JObject editor, VmToContentPropertyLink mapping, IContentImportPropertyService import)
        {
            var output = new List<object>();

            if(rows != null)
            {
                foreach(var r in rows)
                {
                    var row = new
                    {
                        name = GetRowConfigName(r["columns"] as JArray),
                        areas = r["columns"] != null ? AddArea(r["columns"] as JArray, editor, mapping, import) : AddAreaAndControls(r["items"] as JArray, editor, mapping, import),
                        style = new { },
                        hasConfig = r["config"] != null,
                        config = r["config"] != null ? r["config"] : new { } as object,
                        id = GetGuid()
                    };
                    output.Add(row);
                }
            }

            return output;
        }

        public object AddAreaAndControls(JArray items, JObject editor, VmToContentPropertyLink mapping, IContentImportPropertyService import)
        {
            var output = new List<object>();

            if (items != null)
            {
                var area = new
                {
                    grid = 12,
                    allowAll = false,
                    allowed = new string[] { "docType" },
                    hasConfig = false,
                    config = new object[] { },
                    controls = AddControls(items, editor, mapping, import)
                };
                output.Add(area);
            }

            return output;
        }

        public object AddArea(JArray areas, JObject editor, VmToContentPropertyLink mapping, IContentImportPropertyService import)
        {
            var output = new List<object>();

            if (areas != null)
            {
                foreach (var a in areas)
                {
                    var area = new
                    {
                        grid = a["gridSize"] != null ? a["gridSize"] : 12,
                        allowAll = false,
                        allowed = new string[] { "docType" },
                        hasConfig = a["config"] != null,
                        config = a["config"] != null ? a["config"] : new { } as object,
                        controls = AddControls(a["items"] as JArray, editor, mapping, import)
                    };
                    output.Add(area);
                }
            }

            return output;
        }

        public object AddControls(JArray controls, JObject editor, VmToContentPropertyLink mapping, IContentImportPropertyService import)
        {
            var output = new List<object>();

            if (controls != null)
            {
                foreach (var data in controls)
                {
                    var vmName = data["_ref"].ToString().BlockRefToVmTypeName();
                    var vmLink = vmHelperService.GetWithMapping(vmName);

                    var controlOutput = new Dictionary<string, object>();

                    foreach (var m in vmLink.Mappings.Where(x => x.CmsPropertyType != null).OrderBy(x => x.CmsPropertyType.SortOrder))
                    {
                        if(m.VmProperty != null)
                        {
                            var proprtyName = GetJsonPropertyName(m.VmProperty);
                            if (data[proprtyName] != null)
                            {
                                var propertyValue = data[proprtyName].ToString();
                                import.ImportProperty(vmLink.ContentType.Name, "grid", m, propertyValue, (name, value) => {
                                    controlOutput[name] = value;
                                });
                            }
                        }
                    }

                    contentImportMergedService.GetForGrid(data, vmLink.Viewmodel, vmLink.ContentType.Alias, vmLink.Mappings, controlOutput, import);

                    foreach(var key in controlOutput.Keys.ToList())
                    {
                        var value = controlOutput[key];
                        if (value != null && IsValidJson(value.ToString()))
                        {
                            var parsedJson = JToken.Parse(value.ToString());
                            controlOutput[key] = RecursiveParseJsonProperties(parsedJson);
                        }
                    }

                    var control = new
                    {
                        value = new {
                            dtgeContentTypeAlias = GetDocumentTypeAlias(vmLink),
                            value = controlOutput,
                            id = GetGuid()
                        },
                        editor = editor,
                    };
                    output.Add(control);
                }
            }

            return output;
        }

        public string GetRowConfigName(JArray columns)
        {
            var configs = importConfig.GridRowConfigs;

            if (columns == null)
                return configs.Where(x => x.IsDefault).Select(x => x.Name).FirstOrDefault();
            else
            {
                if (columns.Any(x => x["gridSize"] == null))
                    return configs.Where(x => x.IsDefault).Select(x => x.Name).FirstOrDefault();
                else
                {
                    var gridSizes = columns.Select(x => x["gridSize"].ToString()).ToArray();
                    return configs.Where(x => Enumerable.SequenceEqual(x.ActualSizes, gridSizes)).Select(x => x.Name).FirstOrDefault();
                }
            }
        }

        public virtual string GetDocumentTypeAlias(VmContentMapping vmLink)
        {
            return vmLink.ContentType.Alias;
        }

        public virtual string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public virtual string GetJsonPropertyName(PropertyInfo property)
        {
            return property.GetCustomAttributes<JsonPropertyAttribute>().Select(x => x.PropertyName).FirstOrDefault();
        }

        private bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private JToken RecursiveParseJsonProperties(JToken d)
        {
            if (d is JArray)
            {
                var f = d as JArray;
                foreach (var g in f)
                {
                    if(g is JObject)
                    {
                        var h = g as JObject;
                        foreach (var l in h)
                        {
                            if (IsValidJson(l.Value.ToString()))
                            {
                                var parsedJson = JToken.Parse(l.Value.ToString());
                                h[l.Key] = RecursiveParseJsonProperties(parsedJson);
                            }
                        }
                    }
                }
            }
            if (d is JObject)
            {
                var h = d as JObject;
                foreach (var l in h)
                {
                    if (IsValidJson(l.Value.ToString()))
                    {
                        var parsedJson = JToken.Parse(l.Value.ToString());
                        h[l.Key] = RecursiveParseJsonProperties(parsedJson);
                    }
                }
            }
            return d;
        }

    }
}
