using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public interface IYuzuTypeAfterConvertor<in TSource, in TDest>
        : IYuzuTypeAfterConvertor<TSource, TDest, UmbracoMappingContext>, IYuzuMappingResolver
    { }
}
