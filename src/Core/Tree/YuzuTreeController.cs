using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.Common.ModelBinders;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Trees;

namespace YuzuDelivery.Umbraco.Core
{
    [Tree(Constants.Applications.Settings, "YuzuDeliveryViewModelsBuilder", TreeTitle = "Yuzu Viewmodels Builder", TreeGroup = "ui", SortOrder = 15)]
    [PluginController("YuzuDeliveryViewModelsBuilder")]
    public class YuzuTreeController : TreeController
    {
        public YuzuTreeController(ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IEventAggregator eventAggregator)
            : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        { }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Value.RoutePath = $"{Constants.Applications.Settings}/YuzuDeliveryViewModelsBuilder/dashboard";
            root.Value.Icon = "icon-code";
            root.Value.HasChildren = false;
            root.Value.MenuUrl = null;

            return root;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            return null;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            return new TreeNodeCollection();
        }
    }
}
