using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{

    public class DefaultElementMapping : YuzuMappingConfig
    {
        public DefaultElementMapping()
        {
            ManualMaps.AddTypeReplace<DefaultPublishedElementCollectionConvertor>(false);
            ManualMaps.AddTypeReplace<DefaultPublishedElementCollectionToSingleConvertor>(false);
        }
    }

    public class DefaultPublishedElementCollectionConvertor : IYuzuTypeConvertor<IEnumerable<IPublishedElement>, IEnumerable<object>>
    {
        private readonly IMapper mapper;
        private readonly IDefaultPublishedElement[] defaultItems;

        public DefaultPublishedElementCollectionConvertor(IMapper mapper, IDefaultPublishedElement[] defaultItems)
        {
            this.mapper = mapper;
            this.defaultItems = defaultItems;
        }

        public IEnumerable<object> Convert(IEnumerable<IPublishedElement> elements, UmbracoMappingContext context)
        {
            var output = new List<object>();

            if (elements.Any())
            {
                var element = elements.FirstOrDefault();

                foreach (var i in defaultItems)
                {
                    if (i.IsValid(element))
                        output.Add(i.Apply(element, context));
                }
            }

            return output;
        }
    }

    public class DefaultPublishedElementCollectionToSingleConvertor : IYuzuTypeConvertor<IEnumerable<IPublishedElement>, object>
    {
        private readonly IMapper mapper;
        private readonly IDefaultPublishedElement[] defaultItems;

        public DefaultPublishedElementCollectionToSingleConvertor(IMapper mapper, IDefaultPublishedElement[] defaultItems)
        {
            this.mapper = mapper;
            this.defaultItems = defaultItems;
        }

        public object Convert(IEnumerable<IPublishedElement> elements, UmbracoMappingContext context)
        {
            if (elements.Any())
            {
                var element = elements.FirstOrDefault();

                foreach (var i in defaultItems)
                {
                    if (i.IsValid(element))
                        return i.Apply(element, context);
                }
                return null;
            }
            else
                return null;
        }
    }

}
