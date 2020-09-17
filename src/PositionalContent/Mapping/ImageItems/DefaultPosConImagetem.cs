using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using Hifi.PositionalContent;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public class DefaultPosConImagetem<M, V> : IPosConImageItem, IPosConImageItemInternal
        where M : PublishedElementModel
    {
        public IMapper mapper;

        public DefaultPosConImagetem(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public bool IsValid(IPublishedElement content)
        {
            return content is M;
        }

        public object Apply(PositionalContentModel model, IPublishedElement content, IPublishedElement settings)
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
