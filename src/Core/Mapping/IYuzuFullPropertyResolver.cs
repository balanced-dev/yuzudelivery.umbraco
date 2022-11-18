using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public interface IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember>
        : IYuzuFullPropertyResolver<TSource, TDest, TSourceMember, TDestMember, UmbracoMappingContext>, IYuzuMappingResolver
    {
        TDestMember Resolve(TSource source, TDest destination, TSourceMember sourceMember, string destPropertyName, UmbracoMappingContext context);
    }
}
