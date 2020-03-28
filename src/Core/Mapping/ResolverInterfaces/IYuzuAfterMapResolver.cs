using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuAfterMapResolver<Source, Dest> : IYuzuAfterMapResolver, IYuzuMappingResolver
    {
        void Process(Source source, Dest dest, UmbracoMappingContext context);
    }

    public interface IYuzuAfterMapResolver { }
}
