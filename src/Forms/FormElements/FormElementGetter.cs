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
            return fieldsets.SelectMany(x => x.Fields.Select(z => GetFieldVm(z)).Where(z => z != null).ToList()).ToList();
        }

        public List<vmSub_DataFormBuilderRow> UmbracoFormParseGridMappings(IList<FieldsetContainerViewModel> fieldsets)
        {
            var firstColumn = fieldsets.FirstOrDefault();

            return firstColumn.Fields.Select((x, index) =>
            {
                var columns = new List<object>();

                columns.Add(GetFieldVm(x));

                foreach(var f in fieldsets)
                {
                    if(f != firstColumn)
                    {
                        var otherFieldsetField = f.Fields.ElementAtOrDefault(index);

                        if(otherFieldsetField != null)
                        {
                            if (otherFieldsetField.FieldTypeName == _FormsConstant.ColumnBlank_Name)
                            {
                                var fieldName = _FormsConstant.ColumnBlank_Blank_Type;
                                if (otherFieldsetField.AdditionalSettings.ContainsKey(fieldName) && otherFieldsetField.AdditionalSettings[fieldName] == _FormsConstant.ColumnBlank_BlankSpace)
                                    columns.Add(new vmBlock_FormBlank());
                            }
                            else
                            {
                                columns.Add(GetFieldVm(otherFieldsetField));
                            }
                        }
                    }
                }

                return new vmSub_DataFormBuilderRow()
                {
                    Columns = columns.Where(y => y != null).ToList()
                };

            }).ToList();

        }

        private object GetFieldVm(FieldViewModel z)
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
