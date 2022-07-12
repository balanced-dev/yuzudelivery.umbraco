using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public interface IPosConContentItem
    {
        Type ElementType { get; }
        bool IsValid(IPublishedElement content);
        object Apply(IPublishedElement content, IPublishedElement settings, IDictionary<string, object> items, string modifierClass = null);
    }

    public interface IPosConContentItemInternal : IPosConContentItem
    {   }
}
