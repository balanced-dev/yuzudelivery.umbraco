using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Blocks;
using YuzuDelivery.Core;
using System.Reflection;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListDbModelFactory
    {
        private readonly IVmHelperService vmHelperService;
        private readonly GuidFactory guidFactory;

        public BlockListDbModelFactory(IVmHelperService vmHelperService, GuidFactory guidFactory)
        {
            this.vmHelperService = vmHelperService;
            this.guidFactory = guidFactory;
        }

        public virtual BlockListDbModel Create(JArray arrObjects, IContentMapperFactory contentMapperFactory, IContentImportPropertyService contentImportPropertyService, VmContentMapping vmLink = null)
        {
            var outputModel = new BlockListDbModel();

            foreach (var data in arrObjects)
            {
                var layout = new BlockListDbModel.LayoutItem();

                var contentRawData = data["content"] != null ? data["content"] : data;
                var content = GetObject(contentRawData, contentImportPropertyService, vmLink);

                outputModel.ContentData.Add(JObject.FromObject(content));
                layout.ContentUdi = content["udi"].ToString();

                if(data["config"] != null)
                {
                    var settings = GetObject(data["config"], contentImportPropertyService, vmLink);

                    outputModel.SettingsData.Add(JObject.FromObject(settings));
                    layout.SettingsUdi = settings["udi"].ToString();
                }


                outputModel.Layout.UmbracoBlockList.Add(layout);
            }

            return outputModel;
        }

        public Dictionary<string, object> GetObject(JToken data, IContentImportPropertyService contentImportPropertyService, VmContentMapping vmLink = null)
        {
            if (data["_ref"] != null)
            {
                var vmName = data["_ref"].ToString().BlockRefToVmTypeName();
                vmLink = vmHelperService.GetWithMapping(vmName);
            }

            if (vmLink == null)
            {
                throw new Exception("Block list content mapping link not found, a missing _ref on objects");
            }

            var output = new Dictionary<string, object>();
            output["contentTypeKey"] = vmLink.ContentType.Key;
            output["udi"] = Udi.Create("element", guidFactory.CreateNew(vmLink.ContentType.Key));

            foreach (var m in vmLink.Mappings.Where(x => x.CmsPropertyType != null).OrderBy(x => x.CmsPropertyType.SortOrder))
            {
                if (m.VmProperty != null)
                {
                    var proprtyName = GetJsonPropertyName(m.VmProperty);
                    if (data[proprtyName] != null)
                    {
                        var propertyValue = data[proprtyName].ToString();
                        contentImportPropertyService.ImportProperty(vmLink.ContentType.Name, "blockList", m, propertyValue, (name, value) =>
                        {
                            output[name] = value;
                        });
                    }
                }
            }

            return output;
        }

        public virtual string GetJsonPropertyName(PropertyInfo property)
        {
            return property.GetCustomAttributes<JsonPropertyAttribute>().Select(x => x.PropertyName).FirstOrDefault();
        }

    }

    

    public class GuidFactory
    {
        public virtual Guid CreateNew(Guid key)
        {
            return System.Guid.NewGuid();
        }
    } 
}