using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public class UmbracoMappingContext : YuzuMappingContext
    {
        public IPublishedContent Model { get; set; }
        public HttpContext HttpContext { get; set; }

        public UmbracoMappingContext(IDictionary<string, object> items)
            : base(items)
        { }
    }
}
