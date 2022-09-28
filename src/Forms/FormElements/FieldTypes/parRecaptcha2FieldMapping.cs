using System.Linq;
using Umbraco.Forms.Core;
using Microsoft.Extensions.Options;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class parRecaptcha2FieldMapping : IFormFieldMappingsInternal
    {
        private string SiteKey;

        public parRecaptcha2FieldMapping(IOptions<Recaptcha2Settings> configuration)
        {
            this.SiteKey = configuration.Value.PublicKey;
        }

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
