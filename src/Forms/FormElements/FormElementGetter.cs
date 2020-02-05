using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Forms.Mvc.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormElementMapGetter : IFormElementMapGetter
    {
        private IFormFieldMappings[] formfieldMappings;
        private IFormFieldPostProcessor[] formFieldPostProcessors;
        private IFormFieldMappingsInternal[] formfieldMappingsInternal;

        public FormElementMapGetter(IFormFieldMappings[] formfieldMappings, IFormFieldPostProcessor[] formFieldPostProcessors, IFormFieldMappingsInternal[] formfieldMappingsInternal)
        {
            this.formfieldMappings = formfieldMappings;
            this.formFieldPostProcessors = formFieldPostProcessors;
            this.formfieldMappingsInternal = formfieldMappingsInternal;
        }

        public List<object> UmbracoFormParseFieldMappings(IList<FieldsetContainerViewModel> fieldsets)
        {
            return fieldsets.SelectMany(y => y.Fields.Select(z =>
            {
                foreach (var f in formfieldMappings)
                {
                    if (f.IsValid(z.FieldType.Name))
                    {
                        var viewmodel = f.Apply(z);
                        RunPostProcessor(z, ref viewmodel);
                        return viewmodel;
                    }
                }

                foreach (var f in formfieldMappingsInternal)
                {
                    if (f.IsValid(z.FieldType.Name))
                    {
                        var viewmodel = f.Apply(z);
                        RunPostProcessor(z, ref viewmodel);
                        return viewmodel;
                    }
                }

                return null;
            })).Where(x => x != null).ToList();
        }

        private void RunPostProcessor(FieldViewModel z, ref object viewmodel)
        {
            if (viewmodel is IFormFieldElementConfig)
            {
                foreach (var p in formFieldPostProcessors)
                {
                    if (p.IsValid(z.FieldTypeName))
                    {
                        viewmodel = p.Apply(z, viewmodel as IFormFieldElementConfig);
                    }
                }
            }
        }
    }
}
