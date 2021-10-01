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
    public class parRecaptcha2FieldMapping : IFormFieldMappingsInternal
    {
        private string SiteKey;

#if NETCOREAPP
        public parRecaptcha2FieldMapping(IOptionsSnapshot<Recaptcha2Settings> configuration)
        {
            this.SiteKey = configuration.Value.PublicKey;
        }
#else
        public parRecaptcha2FieldMapping()
        {
            this.SiteKey = Configuration.GetSetting("RecaptchaPublicKey");
        }
#endif

        public bool IsValid(string name)
        {
            return name == "Recaptcha2";
        }

        public object Apply(FieldViewModel model)
        {
            var theme = "clean";
            var themeSetting = model.AdditionalSettings.FirstOrDefault(x => x.Key == "Theme");
            if (!string.IsNullOrEmpty(themeSetting.Value))
                theme = themeSetting.Value;

            var size = "clean";
            var sizeSetting = model.AdditionalSettings.FirstOrDefault(x => x.Key == "Size");
            if (!string.IsNullOrEmpty(sizeSetting.Value))
                size = themeSetting.Value;

            return new vmBlock_Recaptcha()
            {
                SiteKey = SiteKey,
                Theme = theme,
                Size = SiteKey,
                _ref = "parRecaptcha"
            };
        }

    }
}
