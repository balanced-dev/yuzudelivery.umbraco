using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using System;
using System.Collections.Generic;
using System.Dynamic;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

#if NETCOREAPP
using Skybrud.Umbraco.GridData.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Grid
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

        public virtual bool IsValid(string name, GridControl control)
        {
            var content = control.GetValue<GridControlDtgeValue>();
            if(content != null && content.Content != null)
            {
                return content.Content.ContentType.Alias == docTypeAlias;
            }
            return false;
        }

        public virtual object Apply(GridItemData data)
        {
            var model = data.Control.GetValue<GridControlDtgeValue>();

            dynamic config = new ExpandoObject();
            config.gridSize = data.Control.Area.Grid;

            return CreateVm(model.Content, data.ContextItems, config);
        }

        public virtual object CreateVm(IPublishedElement model, IDictionary<string, object> contextItems, dynamic config = null)
        {
#if NETCOREAPP
            var item = model.ToElement<M>(publishedValueFallback);
#else
            var item = model.ToElement<M>();
#endif

            var output = typeFactoryRunner.Run<V>(contextItems);
            if (output == null)
                output = mapper.Map<V>(model, contextItems);

            if (config != null && typeof(V).GetProperty("Config") != null)
                typeof(V).GetProperty("Config").SetValue(output, config);
            return output;
        }

        private void AddItemContext(IDictionary<string, object> items, IDictionary<string, object> contextItems)
        {
            if(contextItems != null)
            {
                foreach (var i in contextItems)
                {
                    items.Add(i.Key, i.Value);
                }
            }
        }

    }
}
