using System.Collections.Generic;
using System;

#if NETCOREAPP
using Skybrud.Umbraco.GridData.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Skybrud.Umbraco.GridData;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IGridItem
    {
        Type ElementType { get; }
        bool IsValid(string name, GridControl control);
        object Apply(GridItemData data);
        object CreateVm(IPublishedElement model, IDictionary<string, object> contextItems, dynamic config = null);
    }
}
