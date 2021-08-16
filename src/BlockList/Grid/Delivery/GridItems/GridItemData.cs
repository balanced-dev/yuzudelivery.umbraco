using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models.Blocks;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class GridItemData
    {
        public GridItemData(BlockListItem item, IDictionary<string, object> contextItems)
        {
            Content = item.Content;
            Config = item.Settings;
            ContextItems = contextItems;
        }

        public IPublishedElement Content { get; set; }

        public IPublishedElement Config { get; set; }

        public IDictionary<string, object> ContextItems { get; set; }
    }

}