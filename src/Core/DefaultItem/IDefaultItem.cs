using System.Web.Mvc;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IDefaultItem
    {
        object Apply(IPublishedElement element, UmbracoMappingContext context);
        bool IsValid(IPublishedElement element);
    }
}