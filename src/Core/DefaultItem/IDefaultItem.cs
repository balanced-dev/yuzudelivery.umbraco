using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IDefaultItem
    {
        object Apply(IPublishedElement element, HtmlHelper html);
        bool IsValid(IPublishedElement element);
    }
}