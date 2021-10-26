using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormTypeConvertor : IYuzuTypeConvertor<string, vmBlock_DataForm>
    {
#if NETCOREAPP 
        private readonly ViewComponentHelper viewComponentHelper;

        public FormTypeConvertor(ViewComponentHelper viewComponentHelper)
        {
            this.viewComponentHelper = viewComponentHelper;
        }
#endif

        public vmBlock_DataForm Convert(string formValue, UmbracoMappingContext context)
        {
            if (formValue != null)
            {
                if (!context.Items.ContainsKey("FormBuilderTemplate"))
                    throw new Exception("Form Type Convertor requires FormBuilderTemplate in mapper options items to define which Yuzu template is used.");

                var formBuilderTemplate = context.Items["FormBuilderTemplate"].ToString();

                if (formValue != null && formValue.ToString() != string.Empty)
                {
#if NETCOREAPP 
                    context.Html.ViewContext.RouteData.AddProperty("template", formBuilderTemplate);
                    context.Html.ViewContext.RouteData.AddProperty("mappingItems", context.Items);

                    return new vmBlock_DataForm()
                    {
                        TestForm = null,
                        LiveForm = viewComponentHelper.RenderToString("RenderYuzuUmbracoForms", new
                        {
                            formId = formValue,
                            partial = "/Views/Partials/Forms/YuzuUmbracoFormsV9.cshtml",
                            template = formBuilderTemplate,
                            items = context.Items
                        }, context.Html.ViewContext, context.HttpContext).Result
                    };
#else
                    return new vmBlock_DataForm()
                    {
                        TestForm = null,
                        LiveForm = context.Html?.Action("Render", "UmbracoForms",
                        new
                        {
                            formId = formValue,
                            view = "YuzuUmbracoFormsV8.cshtml",
                            template = formBuilderTemplate,
                            items = context.Items
                        }).ToHtmlString()
                    };
#endif
                }
            }
            return null;
        }
    }
}
