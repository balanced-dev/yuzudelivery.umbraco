using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Web;
using Umbraco.Extensions;

namespace YuzuDelivery.Umbraco.Core
{
    public class LinkIPublishedContentConvertor : IYuzuTypeConvertor<IPublishedContent, vmBlock_DataLink>
    {
        public vmBlock_DataLink Convert(IPublishedContent link, UmbracoMappingContext context)
        {
            if (link != null)
            {
                return new vmBlock_DataLink()
                {
                    Title = link.Name,
                    Label = link.Name,
                    Href = link.Url(),
                    IsActive = link == context.Model
                };
            }
            else
                return null;
        }
    }

    public class LinkConvertor : IYuzuTypeConvertor<Link, vmBlock_DataLink>
    {
        public ILinkFactory _linkFactory;

        public LinkConvertor(ILinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public vmBlock_DataLink Convert(Link link, UmbracoMappingContext context)
        {
            return _linkFactory.Create(link, context);
        }
    }
}
