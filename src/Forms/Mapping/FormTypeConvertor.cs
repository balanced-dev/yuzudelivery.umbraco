using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormTypeConvertor : ITypeConverter<string, vmBlock_DataForm>
    {
        private ISchemaMetaService schemaMetaService;

        public FormTypeConvertor(ISchemaMetaService schemaMetaService)
        {
            this.schemaMetaService = schemaMetaService;
        }

        public vmBlock_DataForm Convert(string formValue, vmBlock_DataForm destination, ResolutionContext context)
        {
            if (formValue != null)
            {
                if (!context.Options.Items.ContainsKey("HtmlHelper"))
                    throw new Exception("Form Type Convertor requires HtmlHelper in mapper options items. Using a property resolver? Make sure it's passed in the chain");

                if (!context.Options.Items.ContainsKey("FormBuilderTemplate"))
                    throw new Exception("Form Type Convertor requires FormBuilderTemplate in mapper options items to define which Yuzu template is used.");

                var html = context.Options.Items["HtmlHelper"] as HtmlHelper;
                var formFieldTemplate = context.Options.Items["FormBuilderTemplate"].ToString();

                if (formValue != null && formValue.ToString() != string.Empty)
                {
                    return new vmBlock_DataForm()
                    {
                        TestForm = null,
                        LiveForm = html.Action("Render", "UmbracoForms",
                        new
                        {
                            formId = formValue,
                            view = "YuzuForms.cshtml",
                            template = formFieldTemplate
                        }).ToHtmlString()
                    };
                }

            }
            return null;
        }
    }
}
