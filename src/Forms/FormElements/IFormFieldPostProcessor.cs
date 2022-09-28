using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IFormFieldPostProcessor
    {

        bool IsValid(string name);
        object Apply(FieldViewModel model, IFormFieldElementConfig viewmodel);

    }
}
