using AutoMapper;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuGroupMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Model, VParent, VChild>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config);
    }
}
