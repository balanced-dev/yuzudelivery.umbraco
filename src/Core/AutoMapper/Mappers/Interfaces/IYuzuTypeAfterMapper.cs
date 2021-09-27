using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper.Configuration;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuTypeAfterMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Source, Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeAfterConvertor<Source, Dest>;
    }
}
