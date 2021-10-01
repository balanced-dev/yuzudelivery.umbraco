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


namespace YuzuDelivery.Umbraco.Grid
{
#if NETCOREAPP
    public static class GridExtensions
    {
        public static IGridItem GridPartial(this IHtmlHelper helper, IPublishedElement publishedElement)
        {
            var overrideGridItems = helper.ViewContext.HttpContext.RequestServices.GetServices<IGridItem>();

            var gridItem = overrideGridItems
                .Where(x => x.ElementType == publishedElement.GetType())
                .Cast<IGridItem>()
                .FirstOrDefault();

            if (gridItem == null)
            {
                var gridItems = helper.ViewContext.HttpContext.RequestServices.GetServices<IGridItemInternal>();

                return gridItems
                    .Where(x => x.ElementType == publishedElement.GetType())
                    .Cast<IGridItem>()
                    .FirstOrDefault();
            }
            else
                return gridItem;
        }

        public static R GridPartial<R>(this IHtmlHelper helper)
            where R : IGridItem
        {
            var overrideGridItems = helper.ViewContext.HttpContext.RequestServices.GetServices<IGridItem>();

            var gridItem = overrideGridItems
                .Where(x => x.ElementType == typeof(R))
                .Cast<IGridItem>()
                .FirstOrDefault();

            if (gridItem == null)
            {
                var gridItems = helper.ViewContext.HttpContext.RequestServices.GetServices<IGridItemInternal>();

                return gridItems
                    .Cast<IGridItem>()
                    .Where(x => x.ElementType == typeof(R))
                    .Cast<R>()
                    .FirstOrDefault();
            }
            else
                return ((R)gridItem);
        }

    }
#else
public static class GridExtensions
    {
        public static IGridItem GridPartial(this HtmlHelper helper, IPublishedElement publishedElement)
        {
            var overrideGridItems = DependencyResolver.Current.GetServices<IGridItem>();

            var gridItem = overrideGridItems
                .Where(x => x.ElementType == publishedElement.GetType())
                .Cast<IGridItem>()
                .FirstOrDefault();

            if (gridItem == null)
            {
                var gridItems = DependencyResolver.Current.GetService<IGridItemInternal[]>();

                return gridItems
                    .Where(x => x.ElementType == publishedElement.GetType())
                    .Cast<IGridItem>()
                    .FirstOrDefault();
            }
            else
                return gridItem;
        }

        public static R GridPartial<R>(this HtmlHelper helper)
            where R : IGridItem
        {
            var overrideGridItems = DependencyResolver.Current.GetServices<IGridItem>();

            var gridItem = overrideGridItems
                .Where(x => x.ElementType == typeof(R))
                .Cast<IGridItem>()
                .FirstOrDefault();

            if (gridItem == null)
            {
                var gridItems = DependencyResolver.Current.GetServices<IGridItemInternal[]>();

                return gridItems
                    .Cast<IGridItem>()
                    .Where(x => x.ElementType == typeof(R))
                    .Cast<R>()
                    .FirstOrDefault();
            }
            else
                return ((R)gridItem);
        }

    }
#endif
}
