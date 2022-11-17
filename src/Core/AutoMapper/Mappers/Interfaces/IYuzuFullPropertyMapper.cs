using System;
using AutoMapper;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuFullPropertyMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Source, Destination, SourceMember, DestMember, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuFullPropertyResolver<Source, Destination, SourceMember, DestMember>;
    }
}
