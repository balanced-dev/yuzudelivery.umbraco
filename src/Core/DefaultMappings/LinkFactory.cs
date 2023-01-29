using YuzuDelivery.Umbraco.Core.Mapping;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace YuzuDelivery.Umbraco.Core
{
    public class LinkFactory : ILinkFactory
    {
        public IPublishedContentQueryAccessor _contentQueryAccessor;

        public LinkFactory(IPublishedContentQueryAccessor contentQueryAccessor)
        {
            _contentQueryAccessor = contentQueryAccessor;
        }

        public vmBlock_DataLink Create(Link link, UmbracoMappingContext context)
        {
            _contentQueryAccessor.TryGetValue(out IPublishedContentQuery contentQuery);

            if (link != null)
            {
                if (link.Type == LinkType.External)
                {
                    return new vmBlock_DataLink()
                    {
                        Title = link.Name,
                        Label = link.Name,
                        Href = link.Url,
                        IsExternalLink = true,
                        IsNewTab = link.Target == "_blank"
                    };
                }
                else
                {
                    var id = link.Udi as GuidUdi;
                    if (id != null)
                    {
                        var content = contentQuery.Content(id);

                        if (content != null)
                        {
                            var name = string.IsNullOrEmpty(link.Name) ? content.Name : link.Name;
                            var udi = context.Model != null ? new GuidUdi("document", context.Model.Key) : null;

                            var d = new vmBlock_DataLink()
                            {
                                Title = name,
                                Label = name,
                                Href = link.Url,
                                IsActive = link.Udi == udi,
                                IsNewTab = link.Target == "_blank"
                            };
                            return d;
                        }

                        var media = contentQuery.Media(id);

                        if (media != null)
                        {
                            var name = string.IsNullOrEmpty(link.Name) ? media.Name : link.Name;

                            var d = new vmBlock_DataLink()
                            {
                                Title = name,
                                Label = name,
                                Href = media.Url(),
                                IsNewTab = link.Target == "_blank"
                            };
                            return d;
                        }
                    }

                }
            }
            return null;
        }
    }

    public interface ILinkFactory
    {
        vmBlock_DataLink Create(Link link, UmbracoMappingContext context);
    }
}
