using System;
using System.Linq;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public static class PublishedModelExtensions
    {
        public static E ToElement<E>(this IPublishedElement x)
            where E : PublishedElementModel
        {
            if (x != null && x is E)
            {
                var type = typeof(E);
                return Activator.CreateInstance(type, new object[] { x }) as E;
            }
            return null;
        }

        public static E ToModel<E>(this IPublishedContent x)
            where E : PublishedContentModel
        {
            if (x != null)
            {
                var type = typeof(E);
                return Activator.CreateInstance(type, new object[] { x }) as E;
            }
            return null;
        }
    }

}
