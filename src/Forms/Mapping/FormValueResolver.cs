using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormValueResolver<Source, Destination> : IYuzuFullPropertyResolver<Source, Destination, object, vmBlock_DataForm>
    {
        private ISchemaMetaService schemaMetaService;

        public FormValueResolver(ISchemaMetaService schemaMetaService)
        {
            this.schemaMetaService = schemaMetaService;
        }

        public vmBlock_DataForm Resolve(Source source, Destination destination, object formValue, string propertyName, UmbracoMappingContext context)
        {
            if (source != null)
            {
                var property = destination.GetType().GetProperties().Where(x => x.PropertyType == typeof(vmBlock_DataForm)).FirstOrDefault();

                var ofType = schemaMetaService.GetOfType(property, "refs");

                if (formValue != null && formValue.ToString() != string.Empty)
                {
                    return new vmBlock_DataForm()
                    {
                        TestForm = null,
                        LiveForm = context.Html.Action("Render", "UmbracoForms",
                        new
                        {
                            formId = formValue,
                            view = "YuzuUmbracoForms.cshtml",
                            template = ofType
                        }).ToHtmlString()
                    };
                }

            }
            return null;
        }
    }
}
