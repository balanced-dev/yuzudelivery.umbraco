using Skybrud.Umbraco.GridData;
using Umbraco.Core.Models.PublishedContent;
using System.Web.Mvc;
using System.Collections.Generic;
using System;

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IGridItemInternal
    {
        Type ElementType { get; }
        bool IsValid(string name, GridControl control);
        object Apply(GridItemData data);
        object CreateVm(IPublishedElement model, IDictionary<string, object> contextItems, dynamic config = null);
    }
}
