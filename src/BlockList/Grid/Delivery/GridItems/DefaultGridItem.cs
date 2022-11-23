using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class DefaultGridItem<TSource, TDest> : IGridItemInternal
        where TSource : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;
        private readonly IYuzuTypeFactoryRunner typeFactoryRunner;

        public DefaultGridItem(
            string docTypeAlias,
            IMapper mapper,
            IYuzuTypeFactoryRunner typeFactoryRunner)
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
            this.typeFactoryRunner = typeFactoryRunner;
        }

        public Type ElementType => typeof(TSource);

        public virtual bool IsValid(IPublishedElement content)
        {
            if (content != null)
            {
                return content.ContentType.Alias == docTypeAlias;
            }

            return false;
        }

        public virtual object Apply(IPublishedElement model, IDictionary<string, object> contextItems)
        {
            var output = typeFactoryRunner.Run<TDest>(contextItems);
            if (output != null)
            {
                return output;
            }

            if (model is TSource source)
            {
                output = mapper.Map<TSource, TDest>(source, contextItems);
            }

            return output;
        }
    }
}
