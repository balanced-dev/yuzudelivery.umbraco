using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using YuzuDelivery.Core;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core
{
    public class UmbracoMappingContext : YuzuMappingContext
    {
        public IPublishedContent Model { get; set; }
        public IHtmlHelper Html { get; set; }
        public HttpContext HttpContext { get; set; }

        public UmbracoMappingContext(IDictionary<string, object> items)
            : base(items)
        { }
    }
}
