﻿using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;
using Umbraco.Web.Composing;
using Umbraco.Core;
using YuzuDelivery.Core;

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
                    Href = link.Url,
                    IsActive = link == context.Model
                };
            }
            else
                return null;
        }
    }

    public class LinkConvertor : IYuzuTypeConvertor<Link, vmBlock_DataLink>
    {
        public vmBlock_DataLink Convert(Link link, UmbracoMappingContext context)
        {
            if (link != null)
            {
                if (link.Type == LinkType.External)
                {
                    return new vmBlock_DataLink()
                    {
                        Title = link.Name,
                        Label = link.Name,
                        Href = link.Url,
                        IsExternalLink = true
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
                                Href = content.Url,
                                IsActive = link == context.Model
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
