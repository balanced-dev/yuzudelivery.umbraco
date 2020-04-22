using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using AutoMapper.Configuration;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultUmbracoMappingFactory
    {
        private readonly IYuzuConfiguration config;

        public DefaultUmbracoMappingFactory(IYuzuConfiguration config)
        {
            this.config = config;
        }

        public IMapper Create(Assembly profilesAssembly, IFactory factory)
        {
            config.MappingAssemblies.Add(profilesAssembly);

            var cfg = new MapperConfigurationExpression();

            var mapContext = cfg.AddYuzuMappersFromContainer(factory);
            cfg.AddProfilesFromContainer(config.MappingAssemblies, factory);
            cfg.AddProfilesForAttributes(config.MappingAssemblies, mapContext, factory);
            cfg.ConstructServicesUsing(factory.GetInstance);

            var mapperConfig = new AutoMapper.MapperConfiguration(cfg);

            var autoMapper = new AutoMapper.Mapper(mapperConfig);

            return new AutoMapperIntegration(autoMapper);
        }
    }

}
