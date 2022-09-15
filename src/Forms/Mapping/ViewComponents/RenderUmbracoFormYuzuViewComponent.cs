#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Web.Services;
using Umbraco.Forms.Web.ViewComponents;

namespace YuzuDelivery.Umbraco.Forms
{
    public class RenderYuzuUmbracoFormsViewComponent : RenderFormViewComponentBase
    {
        public RenderYuzuUmbracoFormsViewComponent(
            IFormRenderingService formRenderingService,
            IFormThemeResolver formThemeResolver)
            : base(formRenderingService, formThemeResolver)
        { }

        public async Task<IViewComponentResult> InvokeAsync(
            Guid formId,
            string partial,
            string template,
            IDictionary<string, object> mappingItems,
            Guid? recordId = null,
            string theme = "")
        {
            var formModelAsync = await FormRenderingService.GetFormModelAsync(HttpContext, formId, recordId, theme);

            if (formModelAsync.FormId == Guid.Empty)
                return null;

            if (TempData["UmbracoFormSubmitted"] != null)
            {
                Guid guid = (Guid) TempData["UmbracoFormSubmitted"];
                formModelAsync.SubmitHandled = guid == formId;
            }

            ViewData["template"] = template;
            ViewData["mappingItems"] = mappingItems;

            return View(partial, formModelAsync);
        }
    }
}
#endif