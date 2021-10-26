#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Web.Services;
using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.ViewComponents;

namespace YuzuDelivery.Umbraco.Forms
{
    public class RenderYuzuUmbracoFormsViewComponent : RenderFormViewComponentBase
      {
        public RenderYuzuUmbracoFormsViewComponent(
          IFormRenderingService formRenderingService,
          IFormThemeResolver formThemeResolver)
          : base(formRenderingService, formThemeResolver)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(
          Guid formId,
          string partial,
          string template,
          Guid? recordId = null)
        {
            FormViewModel formModelAsync = await FormRenderingService.GetFormModelAsync(HttpContext, formId, recordId, null);
          
            if (formModelAsync.FormId == Guid.Empty)
            return (IViewComponentResult) null;

            if (TempData != null && TempData["UmbracoFormSubmitted"] != null)
            {
                Guid guid = (Guid) TempData["UmbracoFormSubmitted"];
                formModelAsync.SubmitHandled = guid == formId;
            }

            return (IViewComponentResult) View<FormViewModel>(partial, formModelAsync);
        }
    }
}
#endif
