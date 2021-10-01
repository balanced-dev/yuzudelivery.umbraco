using System.Collections.Generic;

#if NETCOREAPP 
using Umbraco.Forms.Web.Models;
#else
using Umbraco.Forms.Mvc.Models;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IFormElementMapGetter
    {
        List<object> UmbracoFormParseFieldMappings(IList<FieldsetContainerViewModel> fieldsets);

        List<vmSub_DataFormBuilderRow> UmbracoFormParseGridMappings(IList<FieldsetContainerViewModel> fieldsets);
    }
}