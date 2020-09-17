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

            return posConItems
                .Where(x => x.GetType().GetInterfaces().Any(y => y.GetGenericArguments().Any(z => z == publishedElement.GetType())))
                .Cast<IPosConContentItem>()
                .FirstOrDefault();
        }

        public static R PositionalContentPartial<R>(this UmbracoHelper helper)
            where R : IPosConContentItem
        {
            var partial = DependencyResolver.Current.GetServices<IPosConContentItem>();
            return partial.Where(x => x is R).Cast<R>().FirstOrDefault();
        }
    }
}
