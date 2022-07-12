using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.Extensions.DependencyInjection;
#else
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
#endif


namespace YuzuDelivery.Umbraco.PositionalContent
{
    public static class PositionalContentRenderExtensions
    {
        public static IPosConContentItem PositionalContentPartial(this HtmlHelper helper, IPublishedElement publishedElement)
        {
            var overridePositionalContentItems = DependencyResolver.Current.GetServices<IPosConContentItem>();

            var gridItem = overridePositionalContentItems
                .Where(x => x.ElementType == publishedElement.GetType())
                .Cast<IPosConContentItem>()
                .FirstOrDefault();

            if (gridItem == null)
            {
                var gridItems = DependencyResolver.Current.GetService<IPosConContentItemInternal[]>();

                return gridItems
                    .Where(x => x.ElementType == publishedElement.GetType())
                    .Cast<IPosConContentItem>()
                    .FirstOrDefault();
            }
            else
                return gridItem;
        }

        public static R GridPartial<R>(this HtmlHelper helper)
            where R : IPosConContentItem
        {
            var overrideGridItems = DependencyResolver.Current.GetServices<IPosConContentItem>();

            var gridItem = overrideGridItems
                .Where(x => x.ElementType == typeof(R))
                .Cast<IPosConContentItem>()
                .FirstOrDefault();

            if (gridItem == null)
            {
                var gridItems = DependencyResolver.Current.GetServices<IPosConContentItemInternal[]>();

                return gridItems
                    .Cast<IPosConContentItem>()
                    .Where(x => x.ElementType == typeof(R))
                    .Cast<R>()
                    .FirstOrDefault();
            }
            else
                return ((R)gridItem);
        }

    }
}
