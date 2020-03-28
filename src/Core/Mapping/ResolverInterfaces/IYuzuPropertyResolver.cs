using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuPropertyResolver<M, Type> : IYuzuPropertyResolver, IYuzuMappingResolver
    {
        Type Resolve(M source, UmbracoMappingContext context);
    }

    public interface IYuzuPropertyResolver { }
}
