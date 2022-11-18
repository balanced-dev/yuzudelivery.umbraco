using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public interface IYuzuPropertyReplaceResolver<in TSource, out TDest>
        : IYuzuPropertyReplaceResolver<TSource, TDest, UmbracoMappingContext>, IYuzuMappingResolver
    {
    }
}
