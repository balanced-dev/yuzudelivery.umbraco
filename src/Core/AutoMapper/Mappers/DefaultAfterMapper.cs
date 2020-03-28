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
    public class DefaultAfterMapper : IYuzuAfterMapper
    {
        private readonly IYuzuDeliveryImportConfiguration config;
        private readonly IMappingContextFactory contextFactory;

        public DefaultAfterMapper(IYuzuDeliveryImportConfiguration config, IMappingContextFactory contextFactory)
        {
            this.config = config;
            this.contextFactory = contextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuAfterMapperSettings;

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

        public AddedMapContext CreateMap<Source, Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext)
            where TService : class, IYuzuAfterMapResolver<Source, Dest>
        {
            var settings = baseSettings as YuzuAfterMapperSettings;

            if (settings != null)
            {
                var map = mapContext.Get<Source, Dest>(cfg);

                Action<Source, Dest, ResolutionContext> mappingFunction = (Source source, Dest dest, ResolutionContext context) =>
                {
                    var typeConvertor = factory.GetInstance(typeof(TService)) as TService;
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context);

                    typeConvertor.Process(source, dest, yuzuContext);
                };
                map.AfterMap(mappingFunction);

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuTypeMappingSettings");
        }


    }

}
