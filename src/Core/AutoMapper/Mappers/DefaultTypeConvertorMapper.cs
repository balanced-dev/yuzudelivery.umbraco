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
    public class DefaultTypeConvertorMapper : IYuzuTypeConvertorMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;
        private readonly IMappingContextFactory contextFactory;

        public DefaultTypeConvertorMapper(IYuzuDeliveryImportConfiguration importConfig, IMappingContextFactory contextFactory)
        {
            this.importConfig = importConfig;
            this.contextFactory = contextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuTypeConvertorMapperSettings;

            if (settings != null)
            {
                var genericArguments = settings.Convertor.GetInterfaces().FirstOrDefault().GetGenericArguments().ToList();
                genericArguments.Add(settings.Convertor);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuTypeMappingSettings");
        }

        public AddedMapContext CreateMap<Source, Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeConvertor<Source, Dest>
        {
            var settings = baseSettings as YuzuTypeConvertorMapperSettings;

            if (settings != null)
            {
                config.AddActiveManualMap<TService, Dest>();

                if (settings.IgnoreReturnType)
                    importConfig.IgnoreViewmodels.Add(typeof(Dest).Name);

                var map = mapContext.AddOrGet<Source, Dest>(cfg);

                Func<Source, Dest, ResolutionContext, Dest> mappingFunction = (Source source, Dest dest, ResolutionContext context) =>
                {
                    var typeConvertor = factory.GetInstance(typeof(TService)) as TService;
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context.Items);

                    return typeConvertor.Convert(source, yuzuContext);
                };
                map.ConvertUsing(mappingFunction);

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuTypeMappingSettings");
        }
    }
}
