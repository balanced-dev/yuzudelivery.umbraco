using System;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;

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
            where E : PublishedElementModel
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
