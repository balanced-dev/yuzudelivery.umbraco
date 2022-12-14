using System;
using System.Linq;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.DependencyInjection;

namespace YuzuDelivery.Umbraco.Core
{
    public static class PublishedModelExtensions
    {
        public static E ToElement<E>(this IPublishedElement x, IPublishedValueFallback publishedValueFallback)
            where E : PublishedElementModel
        {
            if (x != null && x is E)
            {
                var type = typeof(E);
                return Activator.CreateInstance(type, new object[] { x, publishedValueFallback }) as E;
            }
            return null;
        }

        public static object ToElement(this IPublishedElement x, Type type, IPublishedValueFallback publishedValueFallback)
        {
            if (x != null && x.GetType() == type)
            {
                return Activator.CreateInstance(type, new object[] { x, publishedValueFallback });
            }
            return null;
        }

        public static E ToModel<E>(this IPublishedContent x, IPublishedValueFallback publishedValueFallback)
            where E : PublishedContentModel
        {
            if (x != null)
            {
                var type = typeof(E);
                return Activator.CreateInstance(type, new object[] { x, publishedValueFallback }) as E;
            }
            return null;
        }
    }
}
