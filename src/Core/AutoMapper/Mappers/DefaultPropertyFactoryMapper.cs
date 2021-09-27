using AutoMapper;
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
    public class DefaultPropertyFactoryMapper : IYuzuPropertyFactoryMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;
        private readonly IMappingContextFactory contextFactory;

        public DefaultPropertyFactoryMapper(IYuzuDeliveryImportConfiguration importConfig, IMappingContextFactory contextFactory)
        {
            this.importConfig = importConfig;
            this.contextFactory = contextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuPropertyFactoryMapperSettings;

            if (settings != null)
            {
                var genericArguments = settings.Factory.GetInterfaces().FirstOrDefault().GetGenericArguments().ToList();
                genericArguments.Add(settings.Source);
                genericArguments.Add(settings.Dest);
                genericArguments.Add(settings.Factory);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyFactoryMapperSettings");
        }

        public AddedMapContext CreateMap<DestMember, Source, Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<DestMember>
        {
            var settings = baseSettings as YuzuPropertyFactoryMapperSettings;

            if (settings != null)
            {
                config.AddActiveManualMap<TService, Dest>(settings.DestPropertyName);

                Func<Source, Dest, object, ResolutionContext, DestMember> mappingFunction = (Source m, Dest v, object o, ResolutionContext context) =>
                {
                    var propertyResolver = factory.GetInstance(typeof(TService)) as TService;
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context.Items);
                    return propertyResolver.Create(yuzuContext);
                };

                var map = mapContext.AddOrGet<Source, Dest>(cfg);

                map.ForMember(settings.DestPropertyName, opt => opt.MapFrom(mappingFunction));

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyFactoryMapperSettings");
        }

    }

}
