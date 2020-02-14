using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Web.Mvc;
using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormBuilderTypeConverter : ITypeConverter<FormViewModel, vmBlock_DataFormBuilder>
    {
        private readonly IFormElementMapGetter formElementMapGetter;

        public FormBuilderTypeConverter(IFormElementMapGetter formElementMapGetter)
        {
            this.formElementMapGetter = formElementMapGetter;
        }

        public vmBlock_DataFormBuilder Convert(FormViewModel source, vmBlock_DataFormBuilder destination, ResolutionContext context)
        {
            if (source != null)
            {
                return new vmBlock_DataFormBuilder()
                {
                    Title = source.CurrentPage.Caption,
                    Fieldsets = source.CurrentPage.Fieldsets.Select(x => new vmSub_DataFormBuilderFieldset()
                    {
                        Legend = x.Caption,
                        Fields = formElementMapGetter.UmbracoFormParseFieldMappings(x.Containers)
                    }).ToList(),
                    SuccessMessage = source.IsLastPage ? source.SubmitCaption : string.Empty
                };
            }
            return new vmBlock_DataFormBuilder();
        }
    }
}
