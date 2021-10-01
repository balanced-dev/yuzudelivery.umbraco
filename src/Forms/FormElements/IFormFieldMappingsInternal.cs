#if NETCOREAPP 
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IFormFieldMappingsInternal
    {

        bool IsValid(string name);
        object Apply(FieldViewModel model);

    }
}
