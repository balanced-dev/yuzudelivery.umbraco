using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

#if NETCOREAPP 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormValueResolver<Source, Destination> : IYuzuFullPropertyResolver<Source, Destination, object, vmBlock_DataForm>
    {
        private ISchemaMetaService schemaMetaService;

#if NETCOREAPP 
        private readonly IViewRenderService _viewRenderService;

        public FormValueResolver(ISchemaMetaService schemaMetaService, IViewRenderService viewRenderService)
        {
            this.schemaMetaService = schemaMetaService;
            _viewRenderService = viewRenderService;
        }
#else
        public FormValueResolver(ISchemaMetaService schemaMetaService)
        {
            this.schemaMetaService = schemaMetaService;
        }
#endif

        public vmBlock_DataForm Resolve(Source source, Destination destination, object formValue, string propertyName, UmbracoMappingContext context)
        {
            if (source != null)
            {
                var property = destination.GetType().GetProperties().Where(x => x.PropertyType == typeof(vmBlock_DataForm)).FirstOrDefault();

                var ofType = schemaMetaService.GetOfType(property, "refs");

                if (string.IsNullOrEmpty(ofType))
                    throw new Exception("Form Type not set in definition e.g \"anyOfType\": \"parFormBuilder\" below the ref");

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
                            template = ofType,
                            items = context.Items
                        }).Result
#else
                        LiveForm = context.Html?.Action("Render", "UmbracoForms",
                        new
                        {
                            formId = formValue,
                            view = "YuzuUmbracoForms.cshtml",
                            template = ofType,
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
