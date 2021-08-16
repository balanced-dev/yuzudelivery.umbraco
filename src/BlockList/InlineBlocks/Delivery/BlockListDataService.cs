using System;
using System.Linq;
using System.Collections.Generic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.BlockList
{
    public class BlockListDataService
    {
        private readonly IMapper mapper;
        private readonly IYuzuConfiguration config;
        private readonly IBlockListItem[] blockListItems;

        private IEnumerable<Type> viewmodelTypes;

        public BlockListDataService(IMapper mapper, IYuzuConfiguration config, IBlockListItem[] blockListItems)
        {
            this.mapper = mapper;
            this.config = config;
            this.blockListItems = blockListItems;

            this.viewmodelTypes = config.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix) || x.Name.StartsWith(YuzuConstants.Configuration.SubPrefix));
        }

        public V CreateItem<V>(BlockListModel model, UmbracoMappingContext context)
        {
            if (model != null && model.Any())
            {
                return ConvertToVm<V>(model.FirstOrDefault(), context);
            }
            else
                return default(V);
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

        private V ConvertToVm<V>(BlockListItem i, UmbracoMappingContext context)
        {
            var alias = i.Content.ContentType.Alias.FirstCharacterToUpper();
            var modelType = config.CMSModels.Where(x => x.Name == alias).FirstOrDefault();
            var vmType = viewmodelTypes.Where(x => x.Name.EndsWith(alias)).FirstOrDefault();

            var o = i.Content.ToElement(modelType);
            if (o != null)
            {
                var custom = blockListItems.Where(x => x.IsValid(i)).FirstOrDefault();

                if (custom != null)
                    return custom.CreateVm<V>(i, context);
                else
                    return mapper.Map<V>(o, context.Items);
            }
            return default(V);
        }
    }

    public static class PublishedModelExtensions
    {
        public static object ToElement(this IPublishedElement x, Type type)
        {
            if (x != null && x.GetType() == type)
            {
                return Activator.CreateInstance(type, new object[] { x });
            }
            return null;
        }

    }
}