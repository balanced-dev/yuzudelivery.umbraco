using System.Collections.Generic;
using YuzuDelivery.Umbraco.Core.Mapping;
using Umbraco.Cms.Core.Models.Blocks;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListToListOfObjectsTypeConvertor : IYuzuTypeConvertor<BlockListModel, List<object>>
    {
        private readonly BlockListDataService service;

        public BlockListToListOfObjectsTypeConvertor(BlockListDataService service)
        {
            this.service = service;
        }

        public List<object> Convert(BlockListModel model, UmbracoMappingContext context)
        {
            return service.CreateList<object>(model, context);
        }
    }

    public class BlockListToObjectTypeConvertor : IYuzuTypeConvertor<BlockListModel, object>
    {
        private readonly BlockListDataService service;

        public BlockListToObjectTypeConvertor(BlockListDataService service)
        {
            this.service = service;
        }

        public object Convert(BlockListModel model, UmbracoMappingContext context)
        {
            return service.CreateItem<object>(model, context);
        }
    }

    public class BlockListToTypeConvertor<V> : IYuzuTypeConvertor<BlockListModel, V>
    {

        private readonly BlockListDataService service;

        public BlockListToTypeConvertor(BlockListDataService service)
        {
            this.service = service;
        }

        public V Convert(BlockListModel model, UmbracoMappingContext context)
        {
            return service.CreateItem<V>(model, context);
        }
    }

    public class BlockListToListOfTypesConvertor<V> : IYuzuTypeConvertor<BlockListModel, List<V>>
    {
        private readonly BlockListDataService service;

        public BlockListToListOfTypesConvertor(BlockListDataService service)
        {
            this.service = service;
        }

        public List<V> Convert(BlockListModel model, UmbracoMappingContext context)
        {
            return service.CreateList<V>(model, context);
        }
    }

}
