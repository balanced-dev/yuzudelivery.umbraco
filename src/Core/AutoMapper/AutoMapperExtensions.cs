using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Umbraco.Core.Composing;

namespace YuzuDelivery.Umbraco.Core
{
    public static class AutomapperConfigExtensions
    {
        public static void AddProfilesForAttributes(this MapperConfigurationExpression cfg, IEnumerable<Assembly> assembliesToScan)
        {
            var allTypes = assembliesToScan.Where(a => !a.IsDynamic && a != typeof(NamedProfile).Assembly).SelectMany(a => a.DefinedTypes).ToArray();
            var autoMapAttributeProfile = new NamedProfile(nameof(AutoMapAttribute));

            foreach (var type in allTypes)
            {
                foreach (var autoMapAttribute in type.GetCustomAttributes<AutoMapAttribute>())
                {
                    var mappingExpression = (MappingExpression)autoMapAttributeProfile.CreateMap(autoMapAttribute.SourceType, type);
                    autoMapAttribute.ApplyConfiguration(mappingExpression);
                }
            }

            cfg.AddProfile(autoMapAttributeProfile);
        }

        public static void AddProfilesFromContainer(this MapperConfigurationExpression cfg, IEnumerable<Assembly> assembliesToScan, IFactory factory)
        {
            var loadedProfiles = GetProfiles(assembliesToScan);

            foreach (var profile in loadedProfiles)
            {
                var resolvedProfile = factory.GetInstance(profile) as Profile;
                cfg.AddProfile(resolvedProfile);
            }
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

        private class NamedProfile : Profile
        {
            public NamedProfile(string profileName) : base(profileName) { }

            public NamedProfile(string profileName, Action<IProfileExpression> config) : base(profileName, config) { }
        }
    }

}
