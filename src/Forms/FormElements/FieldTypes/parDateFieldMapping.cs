using System.Linq;
using System;

#if NETCOREAPP 
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public class parDateFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Date";
        }

        public object Apply(FieldViewModel model)
        {
            string val = string.Empty;
            if (model.ValueAsObject != null && model.ValueAsObject.ToString() != "")
            {
                try
                {
                    DateTime d;
                    d = (DateTime)model.ValueAsObject;
                    val = d.ToShortDateString();
                }
                catch
                {
                    val = model.ValueAsObject.ToString();
                }
            }

            return new vmBlock_FormTextInput()
            {
                Type = "date",
                Id = model.Id,
                Name = model.Id,
                Label = model.Caption,
                Value = val,
                Placeholder = model.PlaceholderText,
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                Pattern = "[0-9]{4}-[0-9]{2}-[0-9]{2}",
                Conditions = model.Condition != null && model.Condition.Rules.Any() ? model.Condition : null,
                _ref = "parFormTextInput"
            };
        }

    }
}
