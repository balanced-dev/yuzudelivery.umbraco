using System.Linq;

#if NETCOREAPP 
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public class parTitleAndDescriptionFieldMapping : IFormFieldMappingsInternal
    {

        public bool IsValid(string name)
        {
            return name == "Title and description";
        }

        public object Apply(FieldViewModel model)
        {
            var title = string.Empty;
            model.AdditionalSettings.TryGetValue("Caption", out title);

            var description = string.Empty;
            model.AdditionalSettings.TryGetValue("BodyText", out description);

            return new vmBlock_TitleAndDescription()
            {
                Title = title,
                Description = description,
                _ref = "parTitleAndDescription"
            };
        }

    }
}
