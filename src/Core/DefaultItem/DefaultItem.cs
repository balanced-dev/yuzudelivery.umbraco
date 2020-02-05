using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultItem<M, V> : IDefaultItem
    where M : PublishedElementModel
    {
        private string docTypeAlias;

        public DefaultItem(string docTypeAlias)
        {
            this.docTypeAlias = docTypeAlias;
        }

        public virtual bool IsValid(IPublishedElement element)
        {
            return element.ContentType.Alias == docTypeAlias;
        }

        public virtual object Apply(IPublishedElement element, HtmlHelper html)
        {
            var item = element.ToElement<M>();

            var mapper = DependencyResolver.Current.GetService<IMapper>();
            var output = mapper.Map<V>(item, opts => opts.Items.Add("HtmlHelper", html));
            return output;
        }
    }

}
