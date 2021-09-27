using System.Collections.Generic;
using YuzuDelivery.Core;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.AspNetCore.Http;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public class UmbracoMappingContextFactory : MappingContextFactory
    {

#if NETCOREAPP
        public UmbracoMappingContextFactory(IHttpContextAccessor httpContextAccessor)
            :base(httpContextAccessor)
        {  }
#endif

        public override T From<T>(IDictionary<string, object> items)
        {
            var output = new UmbracoMappingContext();

            AddDefaults(output, items);

            if (items.ContainsKey("Model"))
            {
                output.Model = items["Model"] as IPublishedContent;
            }

            return output as T;
        }
    }
}
