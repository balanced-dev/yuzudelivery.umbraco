using System.Linq;
using Umbraco.Forms.Mvc.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parCheckboxFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Checkbox";
        }

        public object Apply(FieldViewModel model)
        {
            return new vmBlock_FormCheckboxRadio()
            {
                Type = "checkbox",
                Id = model.Id,
                Name = model.Id,
                Label = model.Caption,
                Value = "true",
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                IsSelected = (model.ContainsValue(true) || model.ContainsValue("true") || model.ContainsValue("on")),
                _ref = "parFormCheckboxRadio"
            };
        }

    }
}
