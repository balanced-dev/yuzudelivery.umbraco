using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;

namespace YuzuDelivery.Umbraco.Forms
{
    public static class FormMappingsExtension
    {
        public static void AddForm<TSource, TDest>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, object>> sourceMember, Expression<Func<TDest, vmBlock_DataForm>> destMember)
        {
            resolvers.Add(new YuzuFullPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuFullPropertyMapper<UmbracoMappingContext>),
                Resolver = typeof(FormValueResolver<TSource, TDest>),
                SourcePropertyName = sourceMember.GetMemberName(),
                DestPropertyName = destMember.GetMemberName()
            });
        }
    }
}
