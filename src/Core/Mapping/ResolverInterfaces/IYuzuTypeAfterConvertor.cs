using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuTypeAfterConvertor<Source, Dest> : IYuzuTypeAfterConvertor, IYuzuMappingResolver
    {
        void Apply(Source source, Dest dest, UmbracoMappingContext context);
    }
}
