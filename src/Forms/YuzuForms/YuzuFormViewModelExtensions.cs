using Newtonsoft.Json;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using YuzuDelivery.Umbraco.Core;
using Umbraco.Web;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Forms
{
    public static class YuzuFormViewModelExtensions
    {
        public static void AddDataAppSettings(this YuzuFormViewModel model, string key, object value)
        {
            var keyName = $"data-{key}";
            var strValue = JsonConvert.SerializeObject(value);

            if (model.HtmlFormAttributes.ContainsKey(keyName))
            {
                model.HtmlFormAttributes[keyName] = strValue;
            }
            else
            {
                model.HtmlFormAttributes.Add(keyName, strValue);
            }
        }

        public static void AddFormField(this vmBlock_DataFormBuilder formBuilder, object field, int fieldsetIndex = 0)
        {
            //if (formBuilder.Fieldsets.ElementAt(fieldsetIndex) != null)
                //((List<Object>)formBuilder.Fieldsets[fieldsetIndex].Fields).Add(field);
        }

        public static void AddHandler<C>(this YuzuFormViewModel formBuilder, Expression<Func<C, ActionResult>> actionLambda)
        {
            Type type = typeof(C);

            var methodName = actionLambda.GetMemberName();

            formBuilder.Controller = type;
            formBuilder.Action = methodName;
        }

        public static bool HasTempData(this Controller controller, string key)
        {
            return controller.TempData.ContainsKey(key) && controller.TempData[key].ToString() == "True";
        }

        public static void SetTempData(this Controller controller, string key)
        {
            controller.TempData[key] = "True";
        }

        public static (string Controller, string Action) ToControllerAndAction(this string endpoint)
        {
            var arrEndpoint = endpoint.Split(',');

            if (arrEndpoint.Length != 2)
                throw new Exception($"{endpoint} endpoint not valid");
            else
                return (arrEndpoint[0], arrEndpoint[1]);
        }

        public static IPublishedElement GetGridFormForEndpoint<C, M>(this UmbracoContext context, Expression<Func<C, ActionResult>> actionLambda, Expression<Func<M, GridDataModel>> gridProperty, string endpointPropertyName = "endpoint")
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
