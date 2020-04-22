using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper.Configuration;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuGroupMapper : IYuzuBaseMapper
    {
        AddedMapContext CreateMap<Model, VParent, VChild>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config);
    }
}
