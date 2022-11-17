using AutoMapper;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuGlobalMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Model, V>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config);
    }
}
