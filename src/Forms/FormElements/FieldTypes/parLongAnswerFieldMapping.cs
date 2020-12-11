using System.Linq;
using Umbraco.Forms.Mvc.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parLongAnswerFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Long answer";
        }

        public object Apply(FieldViewModel model)
        {
            return new vmBlock_FormTextArea()
            {
                Name = model.Id,
                Label = model.Caption,
                Value = model.Values.Select(x => x.ToString()).FirstOrDefault(),
                Placeholder = model.PlaceholderText,
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                Conditions = model.Condition.Rules.Any() ? model.Condition : null,
                _ref = "parFormTextArea"
            };
        }

    }
}
