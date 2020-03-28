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
    public class DefaultTypeMapper : IYuzuTypeMapper
    {
        private readonly IYuzuDeliveryImportConfiguration config;
        private readonly IMappingContextFactory contextFactory;

        public DefaultTypeMapper(IYuzuDeliveryImportConfiguration config, IMappingContextFactory contextFactory)
        {
            this.config = config;
            this.contextFactory = contextFactory;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuTypeMapperSettings;

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

        public AddedMapContext CreateMap<Source, Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext)
            where TService : class, IYuzuTypeConvertor<Source, Dest>
        {
            var settings = baseSettings as YuzuTypeMapperSettings;

            if (settings != null)
            {
                if (settings.IgnoreReturnType)
                    config.IgnoreViewmodels.Add(typeof(Type).Name);

                var map = mapContext.Get<Source, Dest>(cfg);

                Func<Source, Dest, ResolutionContext, Dest> mappingFunction = (Source source, Dest dest, ResolutionContext context) =>
                {
                    var typeConvertor = factory.GetInstance(typeof(TService)) as TService;
                    var yuzuContext = contextFactory.From<UmbracoMappingContext>(context);

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
