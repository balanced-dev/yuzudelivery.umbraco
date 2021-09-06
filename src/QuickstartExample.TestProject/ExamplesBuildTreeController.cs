using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Trees;
using System.Net.Http.Formatting;
using System.Web.Http.ModelBinding;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.WebApi.Filters;
using Umbraco.Web.Mvc;

using Umbraco.Core;

namespace YuzuDelivery.Umbraco.TestProject
{
    [Tree(Constants.Applications.Settings, "YuzuDeliveryExamples", TreeTitle = "Yuzu Viewmodels Test Project", TreeGroup = "ui", SortOrder = 17)]
    [PluginController("YuzuDeliveryExamples")]
    public class YuzuTreeController : TreeController
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
}
