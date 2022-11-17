using System.Reflection;
using AutoMapper;
using YuzuDelivery.Core;
using IMapper = YuzuDelivery.Core.IMapper;

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
