using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.BlockList
{
    public interface IGridItem
    {

        Type ElementType { get; }
        bool IsValid(IPublishedElement control);
        object Apply(IPublishedElement model, IDictionary<string, object> contextItems);

    }
}
