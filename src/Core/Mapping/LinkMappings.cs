using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using AutoMapper;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;
using Umbraco.Web.Composing;
using Umbraco.Core;

namespace YuzuDelivery.Umbraco.Core
{    

    public class LinkMappings : Profile
    {
        public LinkMappings()
        {
            CreateMap<Link, vmBlock_DataLink>()
                .ConvertUsing<LinkConvertor>();

            CreateMap<IPublishedContent, vmBlock_DataLink>()
                    .ConvertUsing<LinkIPublishedContentConvertor>();
        }
    }

    public class LinkIPublishedContentConvertor : ITypeConverter<IPublishedContent, vmBlock_DataLink>
    {
        public vmBlock_DataLink Convert(IPublishedContent link, vmBlock_DataLink destination, ResolutionContext context)
        {
            if (link != null)
            {
                return new vmBlock_DataLink()
                {
                    Title = link.Name,
                    Label = link.Name,
                    Href = link.Url,
                };
            }
            else
                return null;
        }
    }

    public class LinkConvertor : ITypeConverter<Link, vmBlock_DataLink>
    {
        public vmBlock_DataLink Convert(Link link, vmBlock_DataLink destination, ResolutionContext context)
        {
            if (link != null)
            {
                if (link.Type == LinkType.External)
                {
                    return new vmBlock_DataLink()
                    {
                        Title = link.Name,
                        Label = link.Name,
                        Href = link.Url
                    };
                }
                else
                {
                    var id = link.Udi as GuidUdi;
                    if(id != null)
                    {
                        var content = Current.UmbracoHelper.Content(id.Guid);

                        if(content != null)
                        {
                            var d = new vmBlock_DataLink()
                            {
                                Title = link.Name,
                                Label = link.Name,
                                Href = content.Url
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
