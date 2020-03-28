using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultItem<M, V> : IDefaultItem
    where M : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;

        public DefaultItem(string docTypeAlias, IMapper mapper)
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
        }

        public virtual bool IsValid(IPublishedElement element)
        {
            return element.ContentType.Alias == docTypeAlias;
        }

        public virtual object Apply(IPublishedElement element, UmbracoMappingContext context)
        {
            var item = element.ToElement<M>();

            var output = mapper.Map<V>(item, context.Items);
            return output;
        }
    }

}
