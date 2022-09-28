using System;
using System.Linq;
using System.Text;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormValueResolver<Source, Destination> : IYuzuFullPropertyResolver<Source, Destination, object, vmBlock_DataForm>
    {
        private readonly ISchemaMetaService schemaMetaService;
        private readonly ViewComponentHelper viewComponentHelper;

        public FormValueResolver(ISchemaMetaService schemaMetaService, ViewComponentHelper viewComponentHelper)
        {
            this.schemaMetaService = schemaMetaService;
            this.viewComponentHelper = viewComponentHelper;
        }

        public vmBlock_DataForm Resolve(Source source, Destination destination, object formValue, string propertyName, UmbracoMappingContext context)
        {
            if (source != null)
            {
                var property = destination.GetType().GetProperties().FirstOrDefault(x => x.PropertyType == typeof(vmBlock_DataForm));

                var formBuilderTemplate = schemaMetaService.GetOfType(property, "refs");

                if (string.IsNullOrEmpty(formBuilderTemplate))
                    throw new Exception("Form Type not set in definition e.g \"anyOfType\": \"parFormBuilder\" below the ref");

                if (formValue != null && formValue.ToString() != string.Empty)
                {
                    return new vmBlock_DataForm()
                    {
                        TestForm = null,
                        LiveForm = viewComponentHelper.RenderToString("RenderYuzuUmbracoForms", new
                        {
                            formId = formValue,
                            partial = "/Views/Partials/Forms/YuzuUmbracoFormsV9.cshtml",
                            template = formBuilderTemplate,
                            mappingItems = context.Items
                        }, context.Html.ViewContext, context.HttpContext).Result
                    };
                }
            }
            return null;
        }
    }
}
