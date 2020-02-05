using System;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Grid
{
    internal static class PartialAndModelExtensions
    {
        internal static E ToElement<E>(this IPublishedElement x)
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
