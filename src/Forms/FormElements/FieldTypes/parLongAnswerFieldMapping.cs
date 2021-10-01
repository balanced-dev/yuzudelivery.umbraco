using System.Linq;

#if NETCOREAPP 
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

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
                Conditions = model.Condition != null && model.Condition.Rules.Any() ? model.Condition : null,
                _ref = "parFormTextArea"
            };
        }

    }
}
