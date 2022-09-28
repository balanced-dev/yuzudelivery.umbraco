using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IFormFieldMappings
    {

        bool IsValid(string name);
        object Apply(FieldViewModel model);

    }
}
