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
    public class DefaultGroupMapper : IYuzuGroupMapper
    {
        private readonly IYuzuDeliveryImportConfiguration config;

        public DefaultGroupMapper(IYuzuDeliveryImportConfiguration config)
        {
            this.config = config;
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

        public AddedMapContext CreateMap<Source, DestParent, DestChild>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext)
        {
            var settings = baseSettings as YuzuGroupMapperSettings;

            if (settings != null)
            {
                cfg.RecognizePrefixes(settings.GroupName);

                mapContext.Get<Source, DestChild>(cfg);

                var parentMap = mapContext.Get<Source, DestParent>(cfg);

                parentMap.ForMember(settings.PropertyName, opt => opt.MapFrom(y => y));

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuGroupMapperSettings");
        }
    }

}
