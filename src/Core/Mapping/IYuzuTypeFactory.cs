using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public interface IYuzuTypeFactory<out TDest>
        : IYuzuTypeFactory<TDest, UmbracoMappingContext>, IYuzuMappingResolver
    { }
}
