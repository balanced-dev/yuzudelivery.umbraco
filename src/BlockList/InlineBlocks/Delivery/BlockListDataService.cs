using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListDataService
    {
        private readonly IMapper mapper;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly IEnumerable<IBlockListItem> blockListItems;

        private readonly IPublishedValueFallback publishedValueFallback;


        private IEnumerable<Type> viewmodelTypes;

        public BlockListDataService(IMapper mapper, IOptions<YuzuConfiguration> config, IEnumerable<IBlockListItem> blockListItems, IPublishedValueFallback publishedValueFallback)
        {
            this.mapper = mapper;
            this.config = config;
            this.blockListItems = blockListItems;
            this.publishedValueFallback = publishedValueFallback;

            this.viewmodelTypes = config.Value.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix) || x.Name.StartsWith(YuzuConstants.Configuration.SubPrefix));
        }

        public bool IsItem<V>(BlockListModel model)
        {
            return IsItem(typeof(V), model);
        }

        public bool IsItem(Type type, BlockListModel model)
        {
            var alias = model.FirstOrDefault().Content.ContentType.Alias.FirstCharacterToUpper();
            var modelType = config.Value.CMSModels.Where(x => x.Name == alias).FirstOrDefault();

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
            var modelType = config.Value.CMSModels.Where(x => x.Name == alias).FirstOrDefault();
            var vmType = viewmodelTypes.Where(x => x.Name.EndsWith(alias)).FirstOrDefault();

            var o = i.Content.ToElement(modelType, publishedValueFallback);

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
