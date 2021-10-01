using System;
using System.Linq;
using System.Collections.Generic;
using YuzuDelivery.Core;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultPublishedElement<M, V> : IDefaultPublishedElement
    where M : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;
#if NETCOREAPP
        private readonly IPublishedValueFallback publishedValueFallback;
#endif

        public DefaultPublishedElement(string docTypeAlias, IMapper mapper
#if NETCOREAPP
            , IPublishedValueFallback publishedValueFallback
#endif
            )
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
#if NETCOREAPP
            this.publishedValueFallback = publishedValueFallback;
#endif
        }

        public virtual bool IsValid(IPublishedElement element)
        {
            return element.ContentType.Alias == docTypeAlias;
        }

        public virtual object Apply(IPublishedElement element, UmbracoMappingContext context)
        {
#if NETCOREAPP
            var item = element.ToElement<M>(publishedValueFallback);
#else
            var item = element.ToElement<M>();
#endif

            var output = mapper.Map<V>(item, context.Items);
            return output;
        }
    }

    public interface IDefaultPublishedElement
    {
        object Apply(IPublishedElement element, UmbracoMappingContext context);
        bool IsValid(IPublishedElement element);
    }

}
