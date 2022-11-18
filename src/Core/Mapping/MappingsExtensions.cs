using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YuzuDelivery.Core;

using YuzuCore = YuzuDelivery.Core.Mapping.MappingExtensions;

namespace YuzuDelivery.Umbraco.Core
{
    public static class MappingsExtensions
    {
        public static void AddTypeAfterMap<TConverter>(this List<YuzuMapperSettings> resolvers)
            where TConverter : IYuzuTypeAfterConvertor
        {
            resolvers.AddTypeAfterMap(typeof(TConverter));
        }

        public static void AddTypeAfterMap(this List<YuzuMapperSettings> resolvers, Type afterMapType)
        {
            YuzuCore.AddTypeAfterMap<UmbracoMappingContext>(resolvers, afterMapType);
        }

        public static void AddTypeReplace<ConvertorType>(this List<YuzuMapperSettings> resolvers, bool ignoreReturnType = true)
            where ConvertorType : IYuzuTypeConvertor
        {
            resolvers.AddTypeReplace(typeof(ConvertorType));
        }

        public static void AddTypeReplace(this List<YuzuMapperSettings> resolvers, Type convertorType, bool ignoreReturnType = true)
        {
            YuzuCore.AddTypeReplace<UmbracoMappingContext>(resolvers, convertorType, ignoreReturnType);
        }

        public static void AddTypeFactory<ResolverType, Dest>(this List<YuzuMapperSettings> resolvers)
            where ResolverType : IYuzuTypeFactory
        {
            resolvers.AddTypeFactory(typeof(ResolverType), typeof(Dest));
        }

        public static void AddTypeFactory(this List<YuzuMapperSettings> resolvers, Type factoryType, Type destType)
        {
            resolvers.Add(new YuzuTypeFactoryMapperSettings()
            {
                Mapper = typeof(IYuzuTypeFactoryMapper),
                Factory = factoryType,
                Dest = destType,
            });
        }

        public static void AddPropertyFactory<ResolverType, Source, Dest>(this List<YuzuMapperSettings> resolvers, Expression<Func<Dest, object>> destMember, string groupName = "", bool ignoreProperty = true, bool ignoreReturnType = true)
            where ResolverType : IYuzuTypeFactory
        {
            resolvers.AddPropertyFactory(typeof(ResolverType), typeof(Source), typeof(Dest), destMember.GetMemberName(), groupName);
        }

        public static void AddPropertyFactory(this List<YuzuMapperSettings> resolvers, Type factoryType, Type sourceType, Type destType, string destMemberName, string groupName = "")
        {
            resolvers.Add(new YuzuPropertyFactoryMapperSettings()
            {
                Mapper = typeof(IYuzuPropertyFactoryMapper),
                Factory = factoryType,
                Source = sourceType,
                Dest = destType,
                DestPropertyName = destMemberName
            });
        }

        public static void AddPropertyAfter<ResolverType, Dest, DestMember>(this List<YuzuMapperSettings> resolvers, Expression<Func<Dest, DestMember>> destMember, string groupName = "")
            where ResolverType : IYuzuPropertyAfterResolver
        {
            resolvers.Add(new YuzuPropertyAfterMapperSettings()
            {
                Mapper = typeof(IYuzuPropertyAfterMapper),
                Resolver = typeof(ResolverType),
                Dest = typeof(Dest),
                DestProperty = destMember,
                GroupName = groupName
            });
        }

        public static void AddPropertyReplace<ResolverType, Dest>(this List<YuzuMapperSettings> resolvers, Expression<Func<Dest, object>> destMember, string groupName = "", bool ignoreProperty = true, bool ignoreReturnType = true)
            where ResolverType : IYuzuPropertyReplaceResolver
        {
            resolvers.AddPropertyReplace(typeof(ResolverType), typeof(Dest), destMember.GetMemberName(), groupName, ignoreProperty, ignoreReturnType);
        }

        public static void AddPropertyReplace(this List<YuzuMapperSettings> resolvers, Type resolverType, Type destType, string destMemberName, string groupName = "", bool ignoreProperty = true, bool ignoreReturnType = true)
        {
            resolvers.Add(new YuzuPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuPropertyReplaceMapper),
                Resolver = resolverType,
                Dest = destType,
                DestPropertyName = destMemberName,
                GroupName = groupName,
                IgnoreProperty = ignoreProperty,
                IgnoreReturnType = ignoreReturnType
            });
        }

        public static void AddFullProperty<ResolverType, Source, Dest>(this List<YuzuMapperSettings> resolvers, Expression<Func<Source, object>> sourceMember, Expression<Func<Dest, object>> destMember, string groupName = "", bool ignoreProperty = true, bool ignoreReturnType = true)
            where ResolverType : IYuzuPropertyReplaceResolver
        {
            resolvers.AddFullProperty(typeof(ResolverType), sourceMember.GetMemberName(), destMember.GetMemberName(), groupName, ignoreProperty, ignoreReturnType);
        }

        public static void AddFullProperty(this List<YuzuMapperSettings> resolvers, Type resolverType, string sourceMemberName, string destMemberName, string groupName = "", bool ignoreProperty = true, bool ignoreReturnType = true)
        {
            resolvers.Add(new YuzuFullPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuFullPropertyMapper),
                Resolver = resolverType,
                SourcePropertyName = sourceMemberName,
                DestPropertyName = destMemberName,
                GroupName = groupName,
                IgnoreProperty = ignoreProperty,
                IgnoreReturnType = ignoreReturnType
            });
        }

        public static void AddGroup<Source, DestParent, DestChild>(this List<YuzuMapperSettings> resolvers, Expression<Func<DestParent, object>> destParentMember, string groupName)
        {
            resolvers.AddGroup(typeof(Source), typeof(DestParent), typeof(DestChild), destParentMember.GetMemberName(), groupName);
        }

        public static void AddGroup(this List<YuzuMapperSettings> resolvers, Type source, Type destParent, Type destChild, string destParentPropertyName, string groupName)
        {
            resolvers.Add(new YuzuGroupMapperSettings()
            {
                Mapper = typeof(IYuzuGroupMapper),
                Source = source,
                DestParent = destParent,
                DestChild = destChild,
                PropertyName = destParentPropertyName,
                GroupName = groupName
            });
        }

        public static void AddGlobal<Source, Dest>(this List<YuzuMapperSettings> resolvers, string groupName)
        {
            resolvers.AddGlobal(typeof(Source), typeof(Dest), groupName);
        }

        public static void AddGlobal(this List<YuzuMapperSettings> resolvers, Type source, Type dest, string groupName)
        {
            resolvers.Add(new YuzuGlobalMapperSettings()
            {
                Mapper = typeof(IYuzuGlobalMapper),
                Source = source,
                Dest = dest,
                GroupName = groupName
            });
        }
    }
}
