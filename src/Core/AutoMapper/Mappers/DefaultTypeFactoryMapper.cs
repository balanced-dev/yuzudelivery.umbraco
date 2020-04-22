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
    public class DefaultTypeFactoryMapper : IYuzuTypeFactoryMapper
    {
        private readonly IYuzuConfiguration config;

        public DefaultTypeFactoryMapper(IYuzuConfiguration config)
        {
            this.config = config;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuTypeFactoryMapperSettings;

            if (settings != null)
            {
                var genericArguments = settings.Factory.GetInterfaces().FirstOrDefault().GetGenericArguments().ToList();
                genericArguments.Add(settings.Factory);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuTypeFactoryMapperSettings");
        }

        public AddedMapContext CreateMap<Dest, TService>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
            where TService : class, IYuzuTypeFactory<Dest>
        {
            var settings = baseSettings as YuzuTypeFactoryMapperSettings;

            if (settings != null)
            {
                Func<IYuzuTypeFactory> getFactory = () =>
                {
                    return factory.GetInstance(typeof(TService)) as TService;
                };

                if(!config.ViewmodelFactories.ContainsKey(settings.Dest))
                    config.ViewmodelFactories.Add(settings.Dest, getFactory);
                config.AddActiveManualMap<TService, Dest>();

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuTypeFactoryMapperSettings");
        }
    }

}
