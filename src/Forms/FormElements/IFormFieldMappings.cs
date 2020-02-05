using Umbraco.Forms.Mvc.Models;
using System.Web.Mvc;

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IFormFieldMappings
    {

        bool IsValid(string name);
        object Apply(FieldViewModel model);

    }
}
