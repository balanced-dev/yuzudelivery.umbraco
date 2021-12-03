using System.Linq;
using Umbraco.Forms.Core;

#if NETCOREAPP
using Microsoft.Extensions.Options;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public class parRecaptcha3FieldMapping : IFormFieldMappingsInternal
    {
        private string SiteKey;

#if NETCOREAPP
        public parRecaptcha3FieldMapping(IOptions<Recaptcha3Settings> configuration)
        {
            this.SiteKey = configuration.Value.SiteKey;
        }
#else
        private readonly IFacadeConfiguration _facadeConfiguration;

        public parRecaptcha3FieldMapping(IFacadeConfiguration facadeConfiguration)
        {
            this.SiteKey = _facadeConfiguration.GetSetting("RecaptchaV3SiteKey");
        }
#endif

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
