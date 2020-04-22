using AutoMapper;
using System.Web.Mvc;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class UmbracoMappingContextFactory : MappingContextFactory
    {
        public override T From<T>(IDictionary<string, object> items)
        {
            var output = new UmbracoMappingContext();

            AddDefaults(output, items);

            if (items.ContainsKey("Model"))
            {
                output.Model = items["Model"] as IPublishedContent;
            }

            return output as T;
        }
    }
}
