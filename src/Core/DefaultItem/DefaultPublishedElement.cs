using System;
using System.Linq;
using System.Collections.Generic;
using YuzuDelivery.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultPublishedElement<M, V> : IDefaultPublishedElement
    where M : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;
        private readonly IPublishedValueFallback publishedValueFallback;

        public DefaultPublishedElement(string docTypeAlias, IMapper mapper, IPublishedValueFallback publishedValueFallback
            )
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
            this.publishedValueFallback = publishedValueFallback;
        }

        public virtual bool IsValid(IPublishedElement element)
        {
            return element.ContentType.Alias == docTypeAlias;
        }

        public virtual object Apply(IPublishedElement element, UmbracoMappingContext context)
        {
            var item = element.ToElement<M>(publishedValueFallback);

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
