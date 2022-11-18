using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public interface IYuzuPropertyAfterResolver<M, Type> : IYuzuPropertyAfterResolver, IYuzuMappingResolver
    {
        Type Apply(Type value);
    }
}
