using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IYuzuFullPropertyResolver<Source, Destination, SourceMember, DestinationMember> : IYuzuPropertyResolver, IYuzuMappingResolver
    {
        DestinationMember Resolve(Source source, Destination destination, SourceMember sourceMember, string destPropertyName, UmbracoMappingContext context);
    }

    public interface IYuzuFullPropertyResolver { }
}
