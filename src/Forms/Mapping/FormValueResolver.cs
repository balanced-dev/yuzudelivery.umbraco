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
    public class FormMemberValueResolver<Source, Destination> : IMemberValueResolver<Source, Destination, object, vmBlock_DataForm>
    {
        private ISchemaMetaService schemaMetaService;

        public FormMemberValueResolver(ISchemaMetaService schemaMetaService)
        {
            this.schemaMetaService = schemaMetaService;
        }

        public vmBlock_DataForm Resolve(Source source, Destination destination, object formValue, vmBlock_DataForm destMember, ResolutionContext context)
        {
            if (source != null)
            {
                if (!context.Options.Items.ContainsKey("HtmlHelper"))
                    throw new Exception("Form Type Convertor requires HtmlHelper in mapper options items. Using a property resolver? Make sure it's passed in the chain");

                var html = context.Options.Items["HtmlHelper"] as HtmlHelper;

                var property = destination.GetType().GetProperties().Where(x => x.PropertyType == typeof(vmBlock_DataForm)).FirstOrDefault();

                var ofType = schemaMetaService.GetOfType(property, "refs");

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
                            template = ofType
                        }).ToHtmlString()
                    };
                }

            }
            return null;
        }
    }
}
