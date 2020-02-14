using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using System.Web.Mvc;
using AutoMapper;
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

            var loadedProfiles = GetProfiles(config.MappingAssemblies);

            var cfg = new MapperConfigurationExpression();

            foreach (var profile in loadedProfiles)
            {
                var resolvedProfile = factory.GetInstance(profile) as Profile;
                cfg.AddProfile(resolvedProfile);
            }

            cfg.AddMaps(config.ViewModels);

            cfg.ConstructServicesUsing(DependencyResolver.Current.GetService);

            var mapperConfig = new MapperConfiguration(cfg);

            return new Mapper(mapperConfig);
        }

        private static List<Type> GetProfiles(IEnumerable<Assembly> assemblies)
        {
            var profiles = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var assemblyProfiles = assembly.ExportedTypes.Where(type => type.IsSubclassOf(typeof(Profile)));
                profiles.AddRange(assemblyProfiles);
            }
            return profiles;
        }
    }
}
