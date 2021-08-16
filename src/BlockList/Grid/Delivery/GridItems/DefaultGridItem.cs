using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Umbraco.Import;
using System.Web.Mvc;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class DefaultGridItem<M, V> : IGridItem, IGridItemInternal
        where M : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;
        private readonly IYuzuTypeFactoryRunner typeFactoryRunner;

        public DefaultGridItem(string docTypeAlias, IMapper mapper)
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
            this.typeFactoryRunner = DependencyResolver.Current.GetService<IYuzuTypeFactoryRunner>();
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
            var item = model.ToElement<M>();

            var output = typeFactoryRunner.Run<V>(contextItems);
            if (output == null)
                output = mapper.Map<V>(model, contextItems);

            return output;
        }

    }

}