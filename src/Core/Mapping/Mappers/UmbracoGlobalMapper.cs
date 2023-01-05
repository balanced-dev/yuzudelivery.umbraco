using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Umbraco.Core.Mapping.Mappers;

public class UmbracoGlobalMapper : DefaultGlobalMapper
{
    public override void CreateMap<TSource, TDest>(MapperConfigurationExpression cfg, YuzuGlobalMapperSettings settings,
        IServiceProvider factory, AddedMapContext mapContext, YuzuConfiguration config)
    {
        base.CreateMap<TSource, TDest>(cfg, settings, factory, mapContext, config);

        var ignored = new HashSet<Type>
        {
            typeof(IPublishedContent),
            typeof(IPublishedElement)
        };

        foreach (var t in settings.Source.GetInterfaces().Where(x => !ignored.Contains(x)))
        {
            AddListTypeConverter(cfg, t, settings.Dest);
        }
    }

    private void AddListTypeConverter(IMapperConfigurationExpression cfg, Type sourceType, Type destinationType)
    {
        var sourceCollectionType = typeof(IEnumerable<IPublishedContent>);
        var destinationCollectionType = typeof(List<>).MakeGenericType(destinationType); // TODO: Is always List<T>? never T[]?

        var typeConverter = typeof(GlobalMappingEnumerableTypeConverter<,>).MakeGenericType(sourceType, destinationType);

        cfg.CreateMap(sourceCollectionType, destinationCollectionType)
           .ConvertUsing(typeConverter);
    }
}
