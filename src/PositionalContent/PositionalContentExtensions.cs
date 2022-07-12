using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public static class PositionalContentExtensions
    {

        public static IPosConContentItem PosConPartialForBlock(this HtmlHelper helper, IPublishedElement publishedElement)
        {
            var posConItems = DependencyResolver.Current.GetServices<IPosConContentItem>();

            var item = posConItems
                .Cast<IPosConContentItem>()
                .Where(x => x.ElementType == publishedElement.GetType())
                .FirstOrDefault();

            if(item != null)
            {
                return item;
            }
            else
            {
                posConItems = DependencyResolver.Current.GetService<IPosConContentItemInternal[]>();

                return posConItems
                    .Where(x => x.ElementType == publishedElement.GetType())
                    .Cast<IPosConContentItem>()
                    .FirstOrDefault();
            }
        }

        public static IPosConImageItem PosConPartialForImage(this HtmlHelper helper, IPublishedElement publishedElement)
        {
            var posConItems = DependencyResolver.Current.GetServices<IPosConImageItem>();

            var item = posConItems
                .Cast<IPosConImageItem>()
                .Where(x => x.ModelType == publishedElement.GetType())
                .FirstOrDefault();

            if (item != null)
            {
                return item;
            }
            else
            {
                posConItems = DependencyResolver.Current.GetService<IPosConImageItemInternal[]>();

                return posConItems
                    .Where(x => x.ModelType == publishedElement.GetType())
                    .Cast<IPosConImageItem>()
                    .FirstOrDefault();
            }
        }

        public static R PositionalContentPartial<R>(this UmbracoHelper helper)
            where R : IPosConContentItem
        {
            var partial = DependencyResolver.Current.GetServices<IPosConContentItem>();
            return partial.Where(x => x is R).Cast<R>().FirstOrDefault();
        }
    }
}
