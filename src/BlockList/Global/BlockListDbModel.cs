using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListDbModel
    {
        public BlockListDbModel()
        {
            Layout = new LayoutObj();
            ContentData = new List<JObject>();
            SettingsData = new List<JObject>();
        }

        [JsonProperty("layout")]
        public LayoutObj Layout { get; set; }

        [JsonProperty("contentData")]
        public List<JObject> ContentData { get; set; }

        [JsonProperty("settingsData")]
        public List<JObject> SettingsData { get; set; }


        public class LayoutObj
        {
            public LayoutObj()
            {
                UmbracoBlockList = new List<LayoutItem>();
            }

            [JsonProperty("Umbraco.BlockList")]
            public List<LayoutItem> UmbracoBlockList { get; set; }
        }

        public class LayoutItem
        {
            [JsonProperty("contentUdi", NullValueHandling = NullValueHandling.Ignore)]
            public string ContentUdi { get; set; }

            [JsonProperty("settingsUdi", NullValueHandling = NullValueHandling.Ignore)]
            public string SettingsUdi { get; set; }
        }
    }
}
