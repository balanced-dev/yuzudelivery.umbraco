using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormTypeConvertor : IYuzuTypeConvertor<string, vmBlock_DataForm>
    {
        private ISchemaMetaService schemaMetaService;

        public FormTypeConvertor(ISchemaMetaService schemaMetaService)
        {
            this.schemaMetaService = schemaMetaService;
        }

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
                        LiveForm = context.Html.Action("Render", "UmbracoForms",
                        new
                        {
                            formId = formValue,
                            view = "YuzuUmbracoForms.cshtml",
                            template = formFieldTemplate
                        }).ToHtmlString()
                    };
                }

            }
            return null;
        }
    }
}
