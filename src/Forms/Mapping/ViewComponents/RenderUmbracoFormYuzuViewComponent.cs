using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Forms.Web.Services;
using Umbraco.Forms.Web.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Threading.Tasks;


namespace YuzuDelivery.Umbraco.Forms
{
    public class RenderYuzuUmbracoFormsViewComponent : RenderFormViewComponentBase
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RenderYuzuUmbracoFormsViewComponent(
            IFormRenderingService formRenderingService,
            IFormThemeResolver formThemeResolver,
            IServiceScopeFactory scopeFactory)
            : base(formRenderingService, formThemeResolver)
        {
            _scopeFactory = scopeFactory;
        }

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
                using var scope = _scopeFactory.CreateScope();

                Guid guid = (Guid) TempData["UmbracoFormSubmitted"];
                formModelAsync.SubmitHandled = guid == formId;
                TempData.Remove("UmbracoFormSubmitted");
                TempData.Clear();

                var tdp = scope.ServiceProvider.GetRequiredService<ITempDataProvider>();
                tdp.SaveTempData(HttpContext, TempData);
            }

            ViewData["template"] = template;
            ViewData["mappingItems"] = mappingItems;

            return View(partial, formModelAsync);
        }
    }
}
