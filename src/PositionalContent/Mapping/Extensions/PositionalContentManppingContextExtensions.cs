using Hifi.PositionalContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public static class PositionalContentManppingContextExtensions
    {
        public static PositionalContentItemViewModel PositionalContentItemViewModel(this UmbracoMappingContext context)
        {
            if (context.Items.ContainsKey("PositionalContentItemViewModel"))
                return context.Items["PositionalContentItemViewModel"] as PositionalContentItemViewModel;
            else
                return null;
        }

        public static E PositionalContentSettings<E>(this UmbracoMappingContext context)
            where E : PublishedElementModel
        {
            if (context.Items.ContainsKey("PositionalContentSettings"))
            {
                var e = context.Items["PositionalContentSettings"] as IPublishedElement;
                if (e != null) return e.ToElement<E>();
            }
            return default(E);
        }
    }
}
