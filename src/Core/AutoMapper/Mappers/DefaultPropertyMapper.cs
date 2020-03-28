using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Umbraco.Core.Composing;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultPropertyMapper : IYuzuPropertyMapper
    {
        private readonly IYuzuDeliveryImportConfiguration config;
        private readonly IMappingContextFactory contextFactory;

        public DefaultPropertyMapper(IYuzuDeliveryImportConfiguration config, IMappingContextFactory contextFatcory)
        {
            this.config = config;
            this.contextFactory = contextFatcory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuPropertyMapperSettings;

            if (settings != null)
            {
                var genericArguments = settings.Resolver.GetInterfaces().FirstOrDefault().GetGenericArguments().ToList();
                genericArguments.Add(settings.Dest);
                genericArguments.Add(settings.Resolver);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyMappingSettings");
        }

        public AddedMapContext CreateMap<Source, DestMember, Dest, Resolver>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext)
            where Resolver : class, IYuzuPropertyResolver<Source, DestMember>
        {
            var settings = baseSettings as YuzuPropertyMapperSettings;

            if (settings != null)
            {
                if (settings.IgnoreProperty)
                    config.IgnorePropertiesInViewModels.Add(new KeyValuePair<string, string>(typeof(Dest).Name, settings.DestPropertyName));

                if (settings.IgnoreReturnType)
                    config.IgnoreViewmodels.Add(typeof(DestMember).Name);

                if (!string.IsNullOrEmpty(settings.GroupName))
                    cfg.RecognizePrefixes(settings.GroupName);

                Func<Source, Dest, object, ResolutionContext, DestMember> mappingFunction = (Source m, Dest v, object o, ResolutionContext context) =>
                {
                    var propertyResolver = factory.GetInstance(typeof(Resolver)) as Resolver;
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context);
                    return propertyResolver.Resolve(m, yuzuContext);
                };

                var map = mapContext.Get<Source, Dest>(cfg);

                map.ForMember(settings.DestPropertyName, opt => opt.MapFrom(mappingFunction));

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyMappingSettings");
        }

    }

}
