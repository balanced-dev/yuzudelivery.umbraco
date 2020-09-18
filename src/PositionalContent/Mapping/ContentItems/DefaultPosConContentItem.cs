using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public class DefaultPosConContentItem<M, V> : IPosConContentItem, IPosConContentItemInternal
        where M : PublishedElementModel
    {
        public IMapper mapper;

        public Type ModelType => typeof(M);

        public DefaultPosConContentItem(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public bool IsValid(IPublishedElement content)
        {
            return content is M;
        }

        public object Apply(IPublishedElement content, IPublishedElement settings, string modifierClass = null)
        {
            var contentModel = content.ToElement<M>();

            if (contentModel != null)
            {
                var output = mapper.Map<V>(contentModel);
                return output;
            }
            else
                return null;
        }
    }
}
