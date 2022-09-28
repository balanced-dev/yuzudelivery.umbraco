using System.Linq;
using System;
using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parDropdownFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Dropdown";
        }

        public object Apply(FieldViewModel model)
        {
            var options = model.PreValues.Select((a, index) => new SelectedItem() { option = a.Value, value = a.Value }).ToList();

            var selectedOption = options.FirstOrDefault(x => model.Values?.Any(y => y.ToString() == x.value) ?? false);
            if (selectedOption != null) selectedOption.selected = true;


            return new vmBlock_FormSelect()
            {
                Id = model.Id,
                Name = model.Id,
                Label = model.Caption,
                Options = options.Select(x => new vmSub_FormSelectOption() {
                    Value = x.value,
                    Label = x.option,
                    IsSelected = x.selected
                }).ToList(),
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                Conditions = model.Condition != null && model.Condition.Rules.Any() ? model.Condition : null,
                _ref = "parFormSelect"
            };
        }

    }

    public class SelectedItem
    {
        public string option { get; set; }
        public string value { get; set; }
        public bool selected { get; set; }
    }
}
