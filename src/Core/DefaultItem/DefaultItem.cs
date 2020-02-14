using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using AutoMapper;
using Umbraco.Core.Models.PublishedContent;

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

        public virtual object Apply(IPublishedElement element, IDictionary<string, object> contextItems)
        {
            var item = element.ToElement<M>();

            var output = mapper.Map<V>(item, opts => AddItemContext(opts.Items, contextItems));
            return output;
        }

        private void AddItemContext(IDictionary<string, object> items, IDictionary<string, object> contextItems)
        {
            if (contextItems != null)
            {
                foreach (var i in contextItems)
                {
                    items.Add(i.Key, i.Value);
                }
            }
        }
    }

}
