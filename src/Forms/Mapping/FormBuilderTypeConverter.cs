using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Forms.Web.Models;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormBuilderTypeConverter : IYuzuTypeConvertor<FormViewModel, vmBlock_DataFormBuilder>
    {
        private readonly IFormElementMapGetter formElementMapGetter;

        public FormBuilderTypeConverter(IFormElementMapGetter formElementMapGetter)
        {
            this.formElementMapGetter = formElementMapGetter;
        }

        public vmBlock_DataFormBuilder Convert(FormViewModel source, UmbracoMappingContext context)
        {
            if (source != null)
            {
                return new vmBlock_DataFormBuilder()
                {
                    Title = source.CurrentPage.Caption,
                    Fieldsets = source.CurrentPage.Fieldsets.Select(x => new vmSub_DataFormBuilderFieldset()
                    {
                        Legend = x.Caption,
                        Fields = x.Containers.Count == 1 ? formElementMapGetter.UmbracoFormParseFieldMappings(x.Containers) : null,
                        Rows = x.Containers.Count > 1 ? formElementMapGetter.UmbracoFormParseGridMappings(x.Containers) : null
                    }).ToList(),
                    SuccessMessage = source.IsLastPage ? source.SubmitCaption : string.Empty
                };
            }
            return new vmBlock_DataFormBuilder();
        }
    }
}
