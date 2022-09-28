using System.Linq;
using Umbraco.Forms.Core;
using Microsoft.Extensions.Options;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parRecaptcha3FieldMapping : IFormFieldMappingsInternal
    {
        private string SiteKey;

        public parRecaptcha3FieldMapping(IOptions<Recaptcha3Settings> configuration)
        {
            this.SiteKey = configuration.Value.SiteKey;
        }

        public bool IsValid(string name)
        {
            return name == "reCAPTCHA v3 with score";
        }

        public object Apply(FieldViewModel model)
        {
            return new vmBlock_Recaptcha3()
            {
                SiteKey = this.SiteKey,
                Id = model.Id,
                _ref = "parRecaptcha3"
            };
        }

    }
}
