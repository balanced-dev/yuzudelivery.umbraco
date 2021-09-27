using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper.Configuration;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuPropertyAfterMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<M, PropertyType, V, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings settings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuPropertyAfterResolver<M, PropertyType>;
    }
}
