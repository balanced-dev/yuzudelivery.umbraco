using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public interface IYuzuTypeConvertor<in TSource, out TDest>
        : IYuzuTypeConvertor<TSource, TDest, UmbracoMappingContext>, IYuzuMappingResolver
    { }
}
