using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public static class AutomapperConfigExtensions
    {
        public static void AddProfilesForAttributes(this MapperConfigurationExpression cfg, IEnumerable<Assembly> assembliesToScan, AddedMapContext mapContext, IFactory factory)
        {
            var allTypes = assembliesToScan.Where(a => !a.IsDynamic && a != typeof(NamedProfile).Assembly).SelectMany(a => a.DefinedTypes).ToArray();
            var autoMapAttributeProfile = new NamedProfile(nameof(YuzuMapAttribute));
            var config = factory.GetInstance<IYuzuConfiguration>();
            var importConfig = factory.GetInstance<IYuzuDeliveryImportConfiguration>();

            foreach (var viewModels in allTypes)
            {
                foreach (var attribute in viewModels.GetCustomAttributes<YuzuMapAttribute>())
                {
                    var cmsModel = config.CMSModels.Where(x => x.Name == attribute.SourceTypeName).FirstOrDefault();
                    if(cmsModel != null && !mapContext.Has(cmsModel, viewModels) && !importConfig.IgnoreUmbracoModelsForAutomap.Contains(cmsModel.Name))
                    {
                        cfg.CreateMap(cmsModel, viewModels);
                    }
                }
            }
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

        public static AddedMapContext AddYuzuMappersFromContainer(this MapperConfigurationExpression cfg, IFactory factory)
        {
            var mappingConfigs = factory.GetAllInstances<YuzuMappingConfig>();
            var mapContext = new AddedMapContext();

            foreach (var config in mappingConfigs)
            {
                foreach (var item in config.ManualMaps)
                {
                    var propertyOverride = factory.GetInstance(item.Mapper) as IYuzuBaseMapper;
                    var generic = propertyOverride.MakeGenericMethod(item);
                    mapContext = generic.Invoke(propertyOverride, new object[] { cfg, item, factory, mapContext }) as AddedMapContext;
                }
            }

            return mapContext;
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
