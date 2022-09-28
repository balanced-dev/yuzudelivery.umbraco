using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models.Blocks;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class DefaultGridItem<M, V> : IGridItem, IGridItemInternal
        where M : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;
        private readonly IYuzuTypeFactoryRunner typeFactoryRunner;
        private readonly IPublishedValueFallback publishedValueFallback;

        public DefaultGridItem(string docTypeAlias, IMapper mapper, IYuzuTypeFactoryRunner typeFactoryRunner, IPublishedValueFallback publishedValueFallback
            )
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
            this.typeFactoryRunner = typeFactoryRunner;
            this.publishedValueFallback = publishedValueFallback;
        }

        public Type ElementType { get { return typeof(M); } }

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
            var item = model.ToElement<M>(publishedValueFallback);

            var output = typeFactoryRunner.Run<V>(contextItems);
            if (output == null)
                output = mapper.Map<V>(model, contextItems);

            return output;
        }

    }

}