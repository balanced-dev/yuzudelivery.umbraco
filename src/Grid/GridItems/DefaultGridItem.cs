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

namespace YuzuDelivery.Umbraco.Grid
{
    public class DefaultGridItem<M, V> : IGridItem
        where M : PublishedElementModel
    {
        private string docTypeAlias;

        public DefaultGridItem(string docTypeAlias)
        {
            this.docTypeAlias = docTypeAlias;
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

            return CreateVm(model.Content, data.Html, config);
        }

        public virtual dynamic AddToConfig(GridItemData data,  dynamic config)
        {
            return config;
        }

        public virtual object CreateVm(IPublishedElement model, HtmlHelper html, dynamic config = null)
        {
            var item = model.ToElement<M>();

            var mapper = DependencyResolver.Current.GetService<IMapper>();
            var output = mapper.Map<V>(item, opts => opts.Items.Add("HtmlHelper", html));
            if(config != null && typeof(V).GetProperty("Config") != null)
                typeof(V).GetProperty("Config").SetValue(output, config);
            return output;
        }
    }
}
