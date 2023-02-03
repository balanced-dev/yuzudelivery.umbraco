using System;
using System.Linq;
using System.Text;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YuzuDelivery.Umbraco.Core.Mapping;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormValueResolver<Source, Destination> : IYuzuFullPropertyResolver<Source, Destination, object, vmBlock_DataForm>
    {
        private readonly ISchemaMetaService _schemaMetaService;
        private readonly ViewComponentHelper _viewComponentHelper;
        private readonly ViewContextFactory _viewContextFactory;

        private const string FormsComponent = "RenderYuzuUmbracoForms";

        public FormValueResolver(ISchemaMetaService schemaMetaService, ViewComponentHelper viewComponentHelper, ViewContextFactory viewContextFactory)
        {
            _schemaMetaService = schemaMetaService;
            _viewComponentHelper = viewComponentHelper;
            _viewContextFactory = viewContextFactory;
        }

        public vmBlock_DataForm Resolve(Source source, Destination destination, object formValue, string propertyName, UmbracoMappingContext context)
        {
            if (source != null)
            {
                var property = destination.GetType().GetProperties().FirstOrDefault(x => x.PropertyType == typeof(vmBlock_DataForm));

                var formBuilderTemplate = _schemaMetaService.GetOfType(property, "refs");

                if (string.IsNullOrEmpty(formBuilderTemplate))
                    throw new Exception("Form Type not set in definition e.g \"anyOfType\": \"parFormBuilder\" below the ref");

                if (formValue != null && formValue.ToString() != string.Empty)
                {
                    var httpContext = context.HttpContext;

                    var viewContext = _viewContextFactory.Create(httpContext);

                    var viewComponentArgs = new
                    {
                        formId = formValue,
                        partial = "/Views/Partials/Forms/YuzuUmbracoFormsV9.cshtml",
                        template = formBuilderTemplate,
                        mappingItems = context.Items
                    };

                    var renderTask = _viewComponentHelper.RenderToString(
                        FormsComponent,
                        viewComponentArgs,
                        viewContext,
                        httpContext
                    );

                    return new vmBlock_DataForm
                    {
                        TestForm = null,
                        LiveForm = renderTask.GetAwaiter().GetResult()
                    };
                }
            }
            return null;
        }
    }
}
