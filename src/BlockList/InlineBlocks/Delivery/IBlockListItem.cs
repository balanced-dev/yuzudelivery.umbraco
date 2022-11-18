using System;
using System.Linq;
using System.Collections.Generic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.Blocks;
#else
using Umbraco.Core.Models.Blocks;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    public interface IBlockListItem
    {
        bool IsValid(BlockListItem i);
        V CreateVm<V>(BlockListItem i, UmbracoMappingContext context);

        object CreateVm(BlockListItem i, Type destinationType, UmbracoMappingContext context);
    }
}