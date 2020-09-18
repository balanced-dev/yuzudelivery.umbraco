using System;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public interface IPosConContentItem
    {
        Type ModelType { get; }
        bool IsValid(IPublishedElement content);
        object Apply(IPublishedElement content, IPublishedElement settings, string modifierClass = null);
    }

    public interface IPosConContentItemInternal : IPosConContentItem
    {   }
}
