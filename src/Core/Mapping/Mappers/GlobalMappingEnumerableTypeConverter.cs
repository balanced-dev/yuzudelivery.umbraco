using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core.Mapping.Mappers;

public class GlobalMappingEnumerableTypeConverter<TSource, TDest> : AutoMapper.ITypeConverter<IEnumerable<IPublishedContent>, List<TDest>>
{
    public List<TDest> Convert(IEnumerable<IPublishedContent> source, List<TDest> destination, ResolutionContext context)
    {
        if(source != null)
        {
            return source.OfType<TSource>().Select(x => context.Mapper.Map<TDest>(x)).ToList();
        }
        return new List<TDest>();
    }
}
