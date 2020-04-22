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
    public class DefaultTypeAfterMapper : IYuzuTypeAfterMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;
        private readonly IMappingContextFactory contextFactory;

        public DefaultTypeAfterMapper(IYuzuDeliveryImportConfiguration importConfig, IMappingContextFactory contextFactory)
        {
            this.importConfig = importConfig;
            this.contextFactory = contextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuTypeAfterMapperSettings;

            if (settings != null)
            {
                var genericArguments = settings.Action.GetInterfaces().FirstOrDefault().GetGenericArguments().ToList();
                genericArguments.Add(settings.Action);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuTypeMappingSettings");
        }

        public AddedMapContext CreateMap<Source, Dest, Resolver>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where Resolver : class, IYuzuTypeAfterConvertor<Source, Dest>
        {
            var settings = baseSettings as YuzuTypeAfterMapperSettings;

            if (settings != null)
            {
                config.AddActiveManualMap<Resolver, Dest>();

                var map = mapContext.AddOrGet<Source, Dest>(cfg);

                Action<Source, Dest, ResolutionContext> mappingFunction = (Source source, Dest dest, ResolutionContext context) =>
                {
                    var typeConvertor = factory.GetInstance(typeof(Resolver)) as Resolver;
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context.Items);

                    typeConvertor.Apply(source, dest, yuzuContext);
                };
                map.AfterMap(mappingFunction);

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuTypeMappingSettings");
        }


    }

}
