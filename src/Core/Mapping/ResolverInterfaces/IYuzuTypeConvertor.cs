using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuTypeConvertor<Source, Dest> : IYuzuTypeConvertor, IYuzuMappingResolver
    {
        Dest Convert(Source source, UmbracoMappingContext context);
    }

    public interface IYuzuTypeConvertor { }
}
