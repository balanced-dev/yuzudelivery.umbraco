using System.Linq;
using Umbraco.Forms.Mvc.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parSingleChoiceFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Single choice";
        }

        public object Apply(FieldViewModel model)
        {
            var radioButtons = model.PreValues.ToDictionary(pre => pre.Value, pre2 => pre2.Value);
            var value = model.Values.Select(x => x.ToString()).FirstOrDefault();

            return new vmBlock_FormCheckboxRadioList()
            {
                Legend = model.Caption,
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                Elements = radioButtons.Select(x => new vmBlock_FormCheckboxRadio()
                {
                    Type = "radio",
                    Id = model.Id,
                    Name = model.Id,
                    Label = x.Value,
                    Value = x.Key,
                    IsSelected = x.Value == value,
                    IsOneOfSet = true,
                    IsRequired = model.Mandatory,
                    _ref = "parFormCheckboxRadio"
                }).ToList(),
                _ref = "parFormCheckboxRadioList"
            };
        }
    }
}
