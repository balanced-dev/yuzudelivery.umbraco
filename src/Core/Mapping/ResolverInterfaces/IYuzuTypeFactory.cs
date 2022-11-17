using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuTypeFactory<out TDest>
        : IYuzuTypeFactory<TDest, UmbracoMappingContext>, IYuzuMappingResolver
    { }
}
