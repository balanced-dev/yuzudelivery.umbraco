using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuPropertyReplaceResolver<in TSource, out TDest>
        : IYuzuPropertyReplaceResolver<TSource, TDest, UmbracoMappingContext>, IYuzuMappingResolver
    {
    }
}
