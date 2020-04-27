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
    public class DefaultGlobalMapper : IYuzuGlobalMapper
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;

        public DefaultGlobalMapper(IYuzuDeliveryImportConfiguration importConfig)
        {
            this.importConfig = importConfig;
        }

        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            var settings = baseSettings as YuzuGlobalMapperSettings;

            if (settings != null)
            {
                var genericArguments = new List<Type>();
                genericArguments.Add(settings.Source);
                genericArguments.Add(settings.Dest);

                var method = GetType().GetMethod("CreateMap");
                return method.MakeGenericMethod(genericArguments.ToArray());
            }
            else
                throw new Exception("Mapping settings not of type YuzuGlobalMapperSettings");
        }

        public AddedMapContext CreateMap<Source, Dest>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IFactory factory, AddedMapContext mapContext, IYuzuConfiguration config)
        {
            var settings = baseSettings as YuzuGlobalMapperSettings;

            if (settings != null)
            {
                if (settings.GroupName != null)
                    cfg.RecognizePrefixes(settings.GroupName);

                mapContext.AddOrGet<Source, Dest>(cfg);

                return mapContext;
            }
            else
                throw new Exception("Mapping settings not of type YuzuGlobalMapperSettings");
        }
    }
}

