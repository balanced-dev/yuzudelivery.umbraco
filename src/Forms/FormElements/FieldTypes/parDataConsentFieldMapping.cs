using System.Linq;

#if NETCOREAPP 
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public class parDataConsentFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Data Consent";
        }

        public object Apply(FieldViewModel model)
        {
            var acceptCopy = string.Empty;
            model.AdditionalSettings.TryGetValue("AcceptCopy", out acceptCopy);

            return new vmBlock_FormCheckboxRadio()
            {
                Type = "checkbox",
                Id = model.Id,
                Name = model.Id,
                Label = acceptCopy,
                Value = "true",
                IsRequired = model.Mandatory,
                RequiredMessage = model.RequiredErrorMessage,
                IsSelected = (model.ContainsValue(true) || model.ContainsValue("true") || model.ContainsValue("on")),
                _ref = "parFormCheckboxRadio"
            };
        }

    }
}
