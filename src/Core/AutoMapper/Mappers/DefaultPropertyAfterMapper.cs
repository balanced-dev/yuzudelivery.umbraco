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
using System.Linq.Expressions;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultPropertyAfterMapper : IYuzuPropertyAfterMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;
        private readonly IMappingContextFactory contextFactory;

        public DefaultPropertyAfterMapper(IYuzuDeliveryImportConfiguration importConfig, IMappingContextFactory contextFatcory)
        {
            this.importConfig = importConfig;
            this.contextFactory = contextFatcory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuPropertyAfterMapperSettings;

            if (settings != null)
            {
                var genericArguments = settings.Resolver.GetInterfaces().FirstOrDefault().GetGenericArguments().ToList();
                genericArguments.Add(settings.Dest);
                genericArguments.Add(settings.Resolver);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyAfterMapperSettings");
        }

        public AddedMapContext CreateMap<Source, DestMember, Dest, Resolver>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where Resolver : class, IYuzuPropertyAfterResolver<Source, DestMember>
        {
            var settings = baseSettings as YuzuPropertyAfterMapperSettings;

            if (settings != null)
            {
                //need a fix here
                //config.AddActiveManualMap<Resolver, Dest>(settings.DestProperty);

                if (!string.IsNullOrEmpty(settings.GroupName))
                    cfg.RecognizePrefixes(settings.GroupName);

                Func<DestMember, DestMember> mappingFunction = (DestMember input) =>
                {
                    var propertyResolver = factory.GetInstance(typeof(Resolver)) as Resolver;
                    return propertyResolver.Apply(input);
                };

                var map = mapContext.AddOrGet<Source, Dest>(cfg);

                map.ForMember<DestMember>(settings.DestProperty as Expression<Func<Dest, DestMember>>, opt => opt.AddTransform(x => mappingFunction(x)));

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuPropertyMappingSettings");
        }
    }
}
