using System;
using System.Linq;
using System.Collections.Generic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.BlockList
{
    public interface IBlockListItem
    {
        bool IsValid(BlockListItem i);
        V CreateVm<V>(BlockListItem i, UmbracoMappingContext context);
    }
}