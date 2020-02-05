using System.Linq;
using Umbraco.Forms.Mvc.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parShortAnswerFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Short answer";
        }

        public object Apply(FieldViewModel model)
        {
            var type = "text";
            if (model.Regex == "[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+")
                type = "email";
            else if (model.Regex == "^[0-9]*$")
                type = "number";
            else if (model.Regex == "https?://[a-zA-Z0-9-.]+.[a-zA-Z]{2,}")
                type = "url";

            return new vmBlock_FormTextInput()
            {
                Type = type,
                Name = model.Id,
                Id = model.Id,
                Label = model.Caption,
                Value = model.Values.Select(x => x.ToString()).FirstOrDefault(),
                Placeholder = model.PlaceholderText,
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                Pattern = model.Regex,
                PatternMessage = model.InvalidErrorMessage,
                _ref = "parFormTextInput"
            };
        }
    }
}
