using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class UmbracoMappingContext : YuzuMappingContext
    {
        public IPublishedContent Model { get; set; }
    }
}
