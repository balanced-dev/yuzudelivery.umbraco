using System;
using System.Linq;
using System.Collections.Generic;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListDataService
    {
        private readonly IMapper mapper;
        private readonly IYuzuConfiguration config;
        private readonly IEnumerable<IBlockListItem> blockListItems;
#if NETCOREAPP
        private readonly IPublishedValueFallback publishedValueFallback;
#endif

        private IEnumerable<Type> viewmodelTypes;

        public BlockListDataService(IMapper mapper, IYuzuConfiguration config, IEnumerable<IBlockListItem> blockListItems
#if NETCOREAPP
            , IPublishedValueFallback publishedValueFallback
#endif
            )
        {
            this.mapper = mapper;
            this.config = config;
            this.blockListItems = blockListItems;
#if NETCOREAPP
            this.publishedValueFallback = publishedValueFallback;
#endif

            this.viewmodelTypes = config.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix) || x.Name.StartsWith(YuzuConstants.Configuration.SubPrefix));
        }

        public bool IsItem<V>(BlockListModel model)
        {
            return IsItem(typeof(V), model);
        }

        public bool IsItem(Type type, BlockListModel model)
        {
            var alias = model.FirstOrDefault().Content.ContentType.Alias.FirstCharacterToUpper();
            var modelType = config.CMSModels.Where(x => x.Name == alias).FirstOrDefault();

            return modelType == type;
        }

        public V CreateItem<V>(BlockListModel model, UmbracoMappingContext context)
        {
            return ((V)CreateItem(model, context));
        }

        public object CreateItem(BlockListModel model, UmbracoMappingContext context)
        {
            if (model != null && model.Any())
            {
                return ConvertToVm(model.FirstOrDefault(), context);
            }
            else
                return null;
        }

        public List<V> CreateList<V>(BlockListModel model, UmbracoMappingContext context)
        {
            var output = new List<V>();

            foreach (var i in model)
            {
                var vm = ConvertToVm<V>(i, context);
                if (vm != null)
                    output.Add(vm);
            }

            return output;
        }

        public List<object> CreateList(BlockListModel model, UmbracoMappingContext context)
        {
            var output = new List<object>();

            foreach (var i in model)
            {
                var vm = ConvertToVm(i, context);
                if (vm != null)
                    output.Add(vm);
            }

            return output;
        }

        private V ConvertToVm<V>(BlockListItem i, UmbracoMappingContext context)
        {
            return ((V)ConvertToVm(i, context));
        }

        private object ConvertToVm(BlockListItem i, UmbracoMappingContext context)
        {
            var alias = i.Content.ContentType.Alias.FirstCharacterToUpper();
            var modelType = config.CMSModels.Where(x => x.Name == alias).FirstOrDefault();
            var vmType = viewmodelTypes.Where(x => x.Name.EndsWith(alias)).FirstOrDefault();

#if NETCOREAPP
            var o = i.Content.ToElement(modelType, publishedValueFallback);
#else
            var o = i.Content.ToElement(modelType);
#endif
            if (o != null)
            {
                var custom = blockListItems.Where(x => x.IsValid(i)).FirstOrDefault();

                if (custom != null)
                    return custom.CreateVm(i, vmType, context);
                else
                    return mapper.Map(o, modelType, vmType, context.Items);
            }
            return null;
        }
    }
}