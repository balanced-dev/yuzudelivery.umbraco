using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormTypeConvertor : IYuzuTypeConvertor<Guid, vmBlock_DataForm>
    {
        private readonly ViewComponentHelper _viewComponentHelper;
        private readonly ViewContextFactory _viewContextFactory;

        public FormTypeConvertor(ViewComponentHelper viewComponentHelper, ViewContextFactory viewContextFactory)
        {
            _viewComponentHelper = viewComponentHelper;
            _viewContextFactory = viewContextFactory;
        }

        public vmBlock_DataForm Convert(Guid formValue, UmbracoMappingContext context)
        {
            if (formValue != Guid.Empty)
            {
                if (!context.Items.ContainsKey("FormBuilderTemplate"))
                    throw new Exception("Form Type Convertor requires FormBuilderTemplate in mapper options items to define which Yuzu template is used.");

                if (formValue != Guid.Empty && formValue.ToString() != string.Empty)
                {
                    return new vmBlock_DataForm()
                    {
                        TestForm = null,
                        LiveForm = _viewComponentHelper.RenderToString("RenderYuzuUmbracoForms", new
                        {
                            formId = formValue,
                            partial = "/Views/Partials/Forms/YuzuUmbracoFormsV9.cshtml",
                            template = context.Items["FormBuilderTemplate"].ToString(),
                            mappingItems = context.Items
                        }, _viewContextFactory.Create(context.HttpContext), context.HttpContext).Result
                    };
                }
            }
            return null;
        }
    }
}
