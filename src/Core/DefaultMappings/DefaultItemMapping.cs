using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{

    public class DefaultItemMappings : YuzuMappingConfig
    {
        public DefaultItemMappings()
        {
            ManualMaps.AddTypeReplace<DefaultItemsConvertor>(false);
            ManualMaps.AddTypeReplace<DefaultItemConvertor>(false);
        }
    }

    public class DefaultItemsConvertor : IYuzuTypeConvertor<IEnumerable<IPublishedElement>, IEnumerable<object>>
    {
        private readonly IMapper mapper;
        private readonly IDefaultItem[] defaultItems;

        public DefaultItemsConvertor(IMapper mapper, IDefaultItem[] defaultItems)
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

    public class DefaultItemConvertor : IYuzuTypeConvertor<IEnumerable<IPublishedElement>, object>
    {
        private readonly IMapper mapper;
        private readonly IDefaultItem[] defaultItems;

        public DefaultItemConvertor(IMapper mapper, IDefaultItem[] defaultItems)
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
