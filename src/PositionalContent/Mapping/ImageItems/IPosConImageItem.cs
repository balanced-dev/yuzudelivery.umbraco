using System;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Hifi.PositionalContent;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public interface IPosConImageItem
    {
        Type ModelType { get; }
        bool IsValid(IPublishedElement content);
        object Apply(PositionalContentModel model, IPublishedElement content, IPublishedElement settings);
    }

    public interface IPosConImageItemInternal : IPosConImageItem
    { }
}
