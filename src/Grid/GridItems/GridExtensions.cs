using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Grid
{
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
                var gridItems = DependencyResolver.Current.GetService<IGridItem[]>();

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
                var gridItems = DependencyResolver.Current.GetService<IGridItem[]>();

                return gridItems
                    .Where(x => x.ElementType == typeof(R))
                    .Cast<R>()
                    .FirstOrDefault();
            }
            else
                return ((R)gridItem);
        }

    }
}
