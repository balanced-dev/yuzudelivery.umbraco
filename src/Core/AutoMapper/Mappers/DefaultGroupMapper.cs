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
    public class DefaultGroupMapper : IYuzuGroupMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;

        public DefaultGroupMapper(IYuzuDeliveryImportConfiguration importConfig)
        {
            this.importConfig = importConfig;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuGroupMapperSettings;

            if (settings != null)
            {
                var genericArguments = new List<Type>();
                genericArguments.Add(settings.Source);
                genericArguments.Add(settings.DestParent);
                genericArguments.Add(settings.DestChild);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuGroupMapperSettings");
        }

        public AddedMapContext CreateMap<Source, DestParent, DestChild>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config)
        {
            var settings = baseSettings as YuzuGroupMapperSettings;

            if (settings != null)
            {
                var groupNameWithoutSpaces = settings.GroupName.Replace(" ", "");

                cfg.RecognizePrefixes(groupNameWithoutSpaces);

                mapContext.AddOrGet<Source, DestChild>(cfg);

                var parentMap = mapContext.AddOrGet<Source, DestParent>(cfg);
                parentMap.ForMember(settings.PropertyName, opt => opt.MapFrom(y => y));


                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuGroupMapperSettings");
        }
    }
}
