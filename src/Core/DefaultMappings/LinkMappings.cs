using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Web;
using Umbraco.Extensions;
#else
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Models;
#endif

namespace YuzuDelivery.Umbraco.Core
{

    public class LinkMappings : YuzuMappingConfig
    {
        public LinkMappings()
        {
            ManualMaps.AddTypeReplace<LinkIPublishedContentConvertor>(false);
            ManualMaps.AddTypeReplace<LinkConvertor>(false);
        }
    }

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

        public IPublishedContentQueryAccessorYuzu _contentQueryAccessor;

        public LinkConvertor(IPublishedContentQueryAccessorYuzu contentQueryAccessor)
        {
            _contentQueryAccessor = contentQueryAccessor;
        }

        public vmBlock_DataLink Convert(Link link, UmbracoMappingContext context)
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

}