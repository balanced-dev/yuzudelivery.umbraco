using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;

namespace YuzuDelivery.Umbraco.BlockList.Global;

public class BlockGridDbModel
{
    public BlockGridDbModel()
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
            BlockGrid = new List<GridLayoutItem>();
        }

        [JsonProperty("Umbraco.BlockGrid")]
        public List<GridLayoutItem> BlockGrid { get; set; }
    }

    public class GridLayoutItem
    {
        public GridLayoutItem()
        {

        }

        public GridLayoutItem(Udi contentUdi)
        {
            ContentUdi = contentUdi.ToString();
        }

        public GridLayoutItem(BlockGridConfiguration.BlockGridBlockConfiguration block, Udi contentUdi, Udi settingsUdi)
        {
            ContentUdi = contentUdi.ToString();
            if (settingsUdi != null)
            {
                SettingsUdi = settingsUdi.ToString();
            }
            Areas.Add(new GridArea
            {
                Key = block.Areas.First().Key
            });
        }

        [JsonProperty("contentUdi", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentUdi { get; set; }

        [JsonProperty("settingsUdi", NullValueHandling = NullValueHandling.Ignore)]
        public string SettingsUdi { get; set; }

        [JsonProperty("rowSpan")]
        public int RowSpan { get; set; } = 1;

        [JsonProperty("columnSpan")]
        public int ColumnSpan { get; set; } = 12;

        [JsonProperty("areas")]
        public List<GridArea> Areas { get; set; } = new();

        public void AddChild(GridLayoutItem gridLayoutItem)
        {
            Areas.First().Items.Add(gridLayoutItem);
        }
    }

    public class GridArea
    {
        [JsonProperty("key")]
        public Guid Key { get; set; }

        [JsonProperty("items")]
        public List<GridLayoutItem> Items { get; set; } = new();
    }

    public class Container
    {
        public Container(BlockGridConfiguration.BlockGridBlockConfiguration config, GuidFactory guidFactory)
        {
            ContentTypeKey = config.ContentElementTypeKey;
            Udi = Udi.Create("element", guidFactory.CreateNew());
        }

        [JsonProperty("contentTypeKey")]
        public Guid ContentTypeKey { get; set; }

        [JsonProperty("udi")]
        public Udi Udi { get; set; }
    }
}
