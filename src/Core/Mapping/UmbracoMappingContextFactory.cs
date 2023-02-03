using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public class UmbracoMappingContextFactory : IMappingContextFactory<UmbracoMappingContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UmbracoMappingContextFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UmbracoMappingContext Create(IDictionary<string, object> items)
        {
            var output = new UmbracoMappingContext(items);

            AddDefaults(output, items);

            if (items.ContainsKey("Model"))
            {
                output.Model = items["Model"] as IPublishedContent;
            }

            return output;
        }

        private void AddDefaults(UmbracoMappingContext output, IDictionary<string, object> items)
        {
            output.HttpContext = _httpContextAccessor.HttpContext;
        }
    }
}
