﻿using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultFullPropertyMapper : IYuzuFullPropertyMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;
        private readonly IMappingContextFactory contextFactory;

        public DefaultFullPropertyMapper(IYuzuDeliveryImportConfiguration importConfig, IMappingContextFactory contextFactory)
        {
            this.importConfig = importConfig;
            this.contextFactory = contextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuFullPropertyMapperSettings;

            if (settings != null)
            {
                var genericArguments = settings.Resolver.GetInterfaces().FirstOrDefault().GetGenericArguments().ToList();
                genericArguments.Add(settings.Resolver);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyMappingSettings");
        }

        public AddedMapContext CreateMap<Source, Dest, SourceMember, DestMember, Resolver>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where Resolver : class, IYuzuFullPropertyResolver<Source, Dest, SourceMember, DestMember>
        {
            var settings = baseSettings as YuzuFullPropertyMapperSettings;

            if (settings != null)
            {
                if (settings.IgnoreProperty)
                    importConfig.IgnorePropertiesInViewModels.Add(new KeyValuePair<string, string>(typeof(Dest).Name, settings.DestPropertyName));

                if (settings.IgnoreReturnType)
                    importConfig.IgnoreViewmodels.Add(typeof(Type).Name);

                if (!string.IsNullOrEmpty(settings.GroupName))
                    cfg.RecognizePrefixes(settings.GroupName);

                Func<Source, Dest, object, ResolutionContext, DestMember> mappingFunction = (Source m, Dest v, object o, ResolutionContext context) =>
                {
                    var propertyResolver = factory.GetInstance(typeof(Resolver)) as Resolver;
                    var sourceValue = ((SourceMember)typeof(Source).GetProperty(settings.SourcePropertyName).GetValue(m));
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context.Items);

                    return propertyResolver.Resolve(m, v, sourceValue, settings.DestPropertyName, yuzuContext);
                };

                var map = mapContext.AddOrGet<Source, Dest>(cfg);

                map.ForMember(settings.DestPropertyName, opt => opt.MapFrom(mappingFunction));

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyMappingSettings");
        }

    }

}
