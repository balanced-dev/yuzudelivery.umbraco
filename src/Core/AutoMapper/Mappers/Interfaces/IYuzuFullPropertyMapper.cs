using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper.Configuration;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuFullPropertyMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Source, Destination, SourceMember, DestMember, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuFullPropertyResolver<Source, Destination, SourceMember, DestMember>;
    }
}
