using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormTypeConvertor : IYuzuTypeConvertor<string, vmBlock_DataForm>
    {
#if NETCOREAPP 
        private readonly IViewRenderService _viewRenderService;

        public FormTypeConvertor(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }
#endif

        public vmBlock_DataForm Convert(string formValue, UmbracoMappingContext context)
        {
            if (formValue != null)
            {
                if (!context.Items.ContainsKey("FormBuilderTemplate"))
                    throw new Exception("Form Type Convertor requires FormBuilderTemplate in mapper options items to define which Yuzu template is used.");

                var formFieldTemplate = context.Items["FormBuilderTemplate"].ToString();

                if (formValue != null && formValue.ToString() != string.Empty)
                {
                    return new vmBlock_DataForm()
                    {
                        TestForm = null,
#if NETCOREAPP 
                        LiveForm = _viewRenderService.RenderToStringAsync("Render",
                        new
                        {
                            formId = formValue,
                            view = "YuzuUmbracoForms.cshtml",
                            template = formFieldTemplate,
                            items = context.Items
                        }).Result
#else
                        LiveForm = context.Html?.Action("Render", "UmbracoForms",
                        new
                        {
                            formId = formValue,
                            view = "YuzuUmbracoForms.cshtml",
                            template = formFieldTemplate,
                            items = context.Items
                        }).ToHtmlString()
#endif
                    };
                }

            }
            return null;
        }
    }
}
