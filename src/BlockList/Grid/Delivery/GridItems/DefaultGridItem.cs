using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models.Blocks;
#else
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models.Blocks;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    public class DefaultGridItem<M, V> : IGridItem, IGridItemInternal
        where M : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;
        private readonly IYuzuTypeFactoryRunner typeFactoryRunner;
#if NETCOREAPP
        private readonly IPublishedValueFallback publishedValueFallback;
#endif

        public DefaultGridItem(string docTypeAlias, IMapper mapper, IYuzuTypeFactoryRunner typeFactoryRunner
#if NETCOREAPP
            , IPublishedValueFallback publishedValueFallback
#endif
            )
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
            this.typeFactoryRunner = typeFactoryRunner;
#if NETCOREAPP
            this.publishedValueFallback = publishedValueFallback;
#endif
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
#if NETCOREAPP
            var item = model.ToElement<M>(publishedValueFallback);
#else
            var item = model.ToElement<M>();
#endif

            var output = typeFactoryRunner.Run<V>(contextItems);
            if (output == null)
                output = mapper.Map<V>(model, contextItems);

            return output;
        }

    }

}