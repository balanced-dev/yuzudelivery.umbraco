using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core
{

    public class DefaultItemMappings : Profile
    {
        public DefaultItemMappings()
        {

            CreateMap<IEnumerable<IPublishedElement>, object>()
                .ConvertUsing<DefaultItemConvertor>();

            CreateMap<IEnumerable<IPublishedElement>, IEnumerable<object>>()
                .ConvertUsing<DefaultItemsConvertor>();

        }
    }

    public class DefaultItemsConvertor : ITypeConverter<IEnumerable<IPublishedElement>, IEnumerable<object>>
    {
        private readonly IMapper mapper;
        private readonly IDefaultItem[] defaultItems;

        public DefaultItemsConvertor(IMapper mapper, IDefaultItem[] defaultItems)
        {
            this.mapper = mapper;
            this.defaultItems = defaultItems;
        }

        public IEnumerable<object> Convert(IEnumerable<IPublishedElement> elements, IEnumerable<object> destination, ResolutionContext context)
        {
            var output = new List<object>();

            if (elements.Any())
            {
                var element = elements.FirstOrDefault();
                var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

                foreach (var i in defaultItems)
                {
                    if (i.IsValid(element))
                        output.Add(i.Apply(element, context.Items));
                }
            }

            return output;
        }
    }

    public class DefaultItemConvertor : ITypeConverter<IEnumerable<IPublishedElement>, object>
    {
        private readonly IMapper mapper;
        private readonly IDefaultItem[] defaultItems;

        public DefaultItemConvertor(IMapper mapper, IDefaultItem[] defaultItems)
        {
            this.mapper = mapper;
            this.defaultItems = defaultItems;
        }

        public object Convert(IEnumerable<IPublishedElement> elements, object destination, ResolutionContext context)
        {
            if (elements.Any())
            {
                var element = elements.FirstOrDefault();
                var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

                foreach (var i in defaultItems)
                {
                    if (i.IsValid(element))
                        return i.Apply(element, context.Items);
                }
                return null;
            }
            else
                return null;
        }
    }

}
