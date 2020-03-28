using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public static class MappingsExtensions
    {
        public static void AfterMap<AfterMapType>(this List<YuzuMapperSettings> resolvers)
            where AfterMapType : IYuzuAfterMapResolver
        {
            resolvers.Add(new YuzuAfterMapperSettings()
            {
                Mapper = typeof(IYuzuAfterMapper),
                Action = typeof(AfterMapType)
            });
        }

        public static void AddType<ConvertorType>(this List<YuzuMapperSettings> resolvers, bool ignoreReturnType = true)
            where ConvertorType : IYuzuTypeConvertor
        {
            resolvers.Add(new YuzuTypeMapperSettings()
            {
                Mapper = typeof(IYuzuTypeMapper),
                Convertor = typeof(ConvertorType),
                IgnoreReturnType = ignoreReturnType
            });
        }

        public static void AddProperty<ResolverType, Dest>(this List<YuzuMapperSettings> resolvers, Expression<Func<Dest, object>> destMember, string groupName = "", bool ignoreProperty = true, bool ignoreReturnType = true)
            where ResolverType : IYuzuPropertyResolver
        {
            resolvers.Add(new YuzuPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuPropertyMapper),
                Resolver = typeof(ResolverType),
                Dest = typeof(Dest),
                DestPropertyName = destMember.GetMemberName(),
                GroupName = groupName,
                IgnoreProperty = ignoreProperty,
                IgnoreReturnType = ignoreReturnType
            });
        }

        public static void AddFullProperty<ResolverType, Source, Dest>(this List<YuzuMapperSettings> resolvers, Expression<Func<Source, object>> sourceMember, Expression<Func<Dest, object>> destMember, string groupName = "", bool ignoreProperty = true, bool ignoreReturnType = true)
            where ResolverType : IYuzuPropertyResolver
        {
            resolvers.Add(new YuzuFullPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuFullPropertyMapper),
                Resolver = typeof(ResolverType),
                SourcePropertyName = sourceMember.GetMemberName(),
                DestPropertyName = destMember.GetMemberName(),
                GroupName = groupName,
                IgnoreProperty = ignoreProperty,
                IgnoreReturnType = ignoreReturnType
            });
        }

        public static void AddGroup<Source, DestParent, DestChild>(this List<YuzuMapperSettings> resolvers, Expression<Func<DestParent, object>> lambda, string groupName)
        {
            resolvers.Add(new YuzuGroupMapperSettings()
            {
                Mapper = typeof(IYuzuGroupMapper),
                Source = typeof(Source),
                DestParent = typeof(DestParent),
                DestChild = typeof(DestChild),
                PropertyName = lambda.GetMemberName(),
                GroupName = groupName
            });
        }
    }
}
