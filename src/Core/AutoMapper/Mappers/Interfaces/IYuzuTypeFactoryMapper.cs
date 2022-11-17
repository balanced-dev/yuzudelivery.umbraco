using AutoMapper;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuTypeFactoryMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<Dest>;
    }
}
