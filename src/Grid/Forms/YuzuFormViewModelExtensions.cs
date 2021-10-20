using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YuzuDelivery.Umbraco.Core;
using Skybrud.Umbraco.GridData.Dtge;

#if NETCOREAPP
using Umbraco.Extensions;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Skybrud.Umbraco.GridData.Models;
#else
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Skybrud.Umbraco.GridData;
#endif

namespace YuzuDelivery.Umbraco.Grid
{
    public static class YuzuFormViewModelExtensions
    {

#if NETCOREAPP
        public static IPublishedElement GetGridFormForEndpoint<C, M>(this UmbracoContext context, Expression<Func<C, Task<IActionResult>>> actionLambda, Expression<Func<M, GridDataModel>> gridProperty, string endpointPropertyName = "endpoint")
#else
        public static IPublishedElement GetGridFormForEndpoint<C, M>(this UmbracoContext context, Expression<Func<C, ActionResult>> actionLambda, Expression<Func<M, GridDataModel>> gridProperty, string endpointPropertyName = "endpoint")
#endif
            where C : Controller
        {
            Type type = typeof(C);
            var controllerName = type.Name.RemoveFromEnd("Controller");

            var methodName = actionLambda.GetMemberName();
            var propertyName = gridProperty.GetMemberName();

            var pageContent = context.PublishedRequest.PublishedContent;
            if (pageContent != null)
            {
                var endpoint = $"{controllerName},{methodName}";
                var output = pageContent.Value<GridDataModel>(propertyName).GetAllControls()
                    .Select(x => x.GetValue<GridControlDtgeValue>().Content)
                    .Where(x => x != null && x.Value<string>(endpointPropertyName) == endpoint)
                    .FirstOrDefault();
                if (output == null)
                    throw new ArgumentException($"{endpoint} endpoint not found");
                else
                    return output;
            }
            return null;
        }

        public static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }

    }
}
