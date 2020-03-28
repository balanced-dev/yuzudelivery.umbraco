using AutoMapper;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class UmbracoMappingContextFactory : MappingContextFactory
    {
        public override T From<T>(ResolutionContext context)
        {
            var output = new UmbracoMappingContext();

            AddDefaults(output, context);

            if (context.Items.ContainsKey("Model"))
            {
                output.Model = context.Items["Model"] as IPublishedContent;
            }

            return output as T;
        }
    }
}
