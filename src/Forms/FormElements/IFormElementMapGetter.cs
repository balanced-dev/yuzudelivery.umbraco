using System.Collections.Generic;
using Umbraco.Forms.Mvc.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public interface IFormElementMapGetter
    {
        List<object> UmbracoFormParseFieldMappings(IList<FieldsetContainerViewModel> fieldsets);

        List<vmSub_DataFormBuilderRow> UmbracoFormParseGridMappings(IList<FieldsetContainerViewModel> fieldsets);
    }
}