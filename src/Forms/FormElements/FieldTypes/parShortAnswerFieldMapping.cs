using System.Linq;

#if NETCOREAPP
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

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
                Value = model.Values != null ?  model.Values.Select(x => x.ToString()).FirstOrDefault() : null,
                Placeholder = model.PlaceholderText,
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                Pattern = model.Regex,
                PatternMessage = model.InvalidErrorMessage,
                Conditions = model.Condition != null && model.Condition.Rules.Any() ? model.Condition : null,
                _ref = "parFormTextInput"
            };
        }
    }
}
