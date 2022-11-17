using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuTypeAfterConvertor<in TSource, in TDest>
        : IYuzuTypeAfterConvertor<TSource, TDest, UmbracoMappingContext>, IYuzuMappingResolver
    { }
}
