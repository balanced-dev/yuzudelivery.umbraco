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
    public class DefaultPropertyReplaceMapper : IYuzuPropertyReplaceMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;
        private readonly IMappingContextFactory contextFactory;

        public DefaultPropertyReplaceMapper(IYuzuDeliveryImportConfiguration importConfig, IMappingContextFactory contextFatcory)
        {
            this.importConfig = importConfig;
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

        public AddedMapContext CreateMap<Source, DestMember, Dest, Resolver>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where Resolver : class, IYuzuPropertyReplaceResolver<Source, DestMember>
        {
            var settings = baseSettings as YuzuPropertyMapperSettings;

            if (settings != null)
            {
                config.AddActiveManualMap<Resolver, Dest>(settings.DestPropertyName);

                if (settings.IgnoreProperty)
                    importConfig.IgnorePropertiesInViewModels.Add(new KeyValuePair<string, string>(typeof(Dest).Name, settings.DestPropertyName));

                if (settings.IgnoreReturnType)
                {
                    if(typeof(DestMember).IsGenericType)
                        importConfig.IgnoreViewmodels.Add(typeof(DestMember).GetGenericArguments().Select(x => x.Name).FirstOrDefault());
                    else
                        importConfig.IgnoreViewmodels.Add(typeof(DestMember).Name);
                }


                if (!string.IsNullOrEmpty(settings.GroupName))
                    cfg.RecognizePrefixes(settings.GroupName);

                Func<Source, Dest, object, ResolutionContext, DestMember> mappingFunction = (Source m, Dest v, object o, ResolutionContext context) =>
                {
                    var propertyResolver = factory.GetInstance(typeof(Resolver)) as Resolver;
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context.Items);
                    return propertyResolver.Resolve(m, yuzuContext);
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
