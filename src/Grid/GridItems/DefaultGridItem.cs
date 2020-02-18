using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using Umbraco.Core.Models.PublishedContent;
using AutoMapper;
using System.Dynamic;
using System.Web.Mvc;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.Grid
{
    public class DefaultGridItem<M, V> : IGridItem, IGridItemInternal
        where M : PublishedElementModel
    {
        private string docTypeAlias;
        private readonly IMapper mapper;

        public DefaultGridItem(string docTypeAlias, IMapper mapper)
        {
            this.docTypeAlias = docTypeAlias;
            this.mapper = mapper;
        }

        public Type ElementType { get { return typeof(M); } }

        public virtual bool IsValid(string name, GridControl control)
        {
            var content = control.GetValue<GridControlDtgeValue>();
            if(content != null)
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
            var item = model.ToElement<M>();

            var output = mapper.Map<V>(item, opts => AddItemContext(opts.Items, contextItems));

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
