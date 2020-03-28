using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Core;
using Umbraco.Web;

namespace YuzuDelivery.Umbraco.Core
{
    public class UmbracoMapperAddItems : IMapperAddItem
    {
        private readonly Func<UmbracoContext> umbracoContext;

        public UmbracoMapperAddItems(Func<UmbracoContext> umbracoContext)
        {
            this.umbracoContext = umbracoContext;
        }

        public void Add(IDictionary<string, object> items)
        {
            if(umbracoContext != null && umbracoContext().PublishedRequest != null && umbracoContext().PublishedRequest.PublishedContent != null)
            {
                items.Add("Model", umbracoContext().PublishedRequest.PublishedContent);
            }
        }
    }
}
