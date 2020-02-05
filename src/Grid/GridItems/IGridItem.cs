using Skybrud.Umbraco.GridData;
using Umbraco.Core.Models.PublishedContent;
using System.Web.Mvc;
using System;

namespace YuzuDelivery.Umbraco.Grid
{
    public interface IGridItem
    {
        Type ElementType { get; }
        bool IsValid(string name, GridControl control);
        object Apply(GridItemData data);
        object CreateVm(IPublishedElement model, HtmlHelper htmlHelper, dynamic config = null);
    }
}
