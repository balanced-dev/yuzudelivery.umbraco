using System.Linq;
using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parMultipleChoiceFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Multiple choice";
        }

        public object Apply(FieldViewModel model)
        {
            var checkBoxes = model.PreValues.ToDictionary(pre => pre.Value, pre2 => pre2.Value);
            var selectedValues = model.Values.Select(d => d.ToString()).ToArray();

            return new vmBlock_FormCheckboxRadioList()
            {
                Legend = model.Caption,
                Id = model.Id,
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                Elements = checkBoxes.Select((x, index) => new vmBlock_FormCheckboxRadio()
                {
                    Type = "checkbox",
                    Id = string.Format("{0}-{1}", model.Id, index),
                    CheckboxRadioListId = model.Id,
                    Name = model.Id,
                    Label = x.Value,
                    Value = x.Key,
                    IsSelected = selectedValues.Contains(x.Value),
                    IsOneOfSet = true,
                    _ref = "parFormCheckboxRadio"
                }).ToList(),
                Conditions = model.Condition != null && model.Condition.Rules.Any() ? model.Condition : null,
                _ref = "parFormCheckboxRadioList"
            };
        }
    }
}
