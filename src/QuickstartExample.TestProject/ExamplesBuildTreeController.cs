using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.Common.ModelBinders;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Trees;
#else
using System.Net.Http.Formatting;
using System.Web.Http.ModelBinding;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi.Filters;
using Umbraco.Web.Trees;
#endif

namespace YuzuDelivery.Umbraco.TestProject
{
    [Tree(Constants.Applications.Settings, "YuzuDeliveryExamples", TreeTitle = "Yuzu Viewmodels Test Project", TreeGroup = "ui", SortOrder = 17)]
    [PluginController("YuzuDeliveryExamples")]
#if NETCOREAPP
    public class YuzuExamplesTreeController : TreeController
    {

        public YuzuExamplesTreeController(ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IEventAggregator eventAggregator)
    :       base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        { }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Value.RoutePath = $"{Constants.Applications.Settings}/YuzuDeliveryExamples/dashboard";
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
#else
    public class YuzuExamplesTreeController : TreeController
    {
        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.RoutePath = $"{Constants.Applications.Settings}/YuzuDeliveryExamples/dashboard";
            root.Icon = "icon-code";
            root.HasChildren = false;
            root.MenuUrl = null;

            return root;
        }

        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return null;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return new TreeNodeCollection();
        }
    }
#endif
}
