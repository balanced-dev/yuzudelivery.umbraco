using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models.Blocks;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListItemToTypeConvertor<M,V> : IYuzuTypeConvertor<BlockListItem<M>, V>
        where M : PublishedElementModel
    {

        private readonly IMapper _mapper;

        public BlockListItemToTypeConvertor(IMapper mapper)
        {
            _mapper = mapper;
        }

        public V Convert(BlockListItem<M> model, UmbracoMappingContext context)
        {
            return _mapper.Map<V>(model.Content);
        }
    }

}
