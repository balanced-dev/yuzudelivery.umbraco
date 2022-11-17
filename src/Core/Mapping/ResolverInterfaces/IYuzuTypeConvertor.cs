using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuTypeConvertor<in TSource, out TDest>
        : IYuzuTypeConvertor<TSource, TDest, UmbracoMappingContext>, IYuzuMappingResolver
    { }
}
