using System.Linq;
using Umbraco.Forms.Core;
using Umbraco.Forms.Mvc.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parRecaptcha2FieldMapping : IFormFieldMappingsInternal
    {
        public bool IsValid(string name)
        {
            return name == "Recaptcha2";
        }

        public object Apply(FieldViewModel model)
        {
            var siteKey = Configuration.GetSetting("RecaptchaPublicKey");

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
                SiteKey = siteKey,
                Theme = theme,
                Size = siteKey,
                _ref = "parRecaptcha"
            };
        }

    }
}
