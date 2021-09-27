using YuzuDelivery.Core;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public class UmbracoMappingContext : YuzuMappingContext
    {
        public IPublishedContent Model { get; set; }
    }
}
