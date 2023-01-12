using Microsoft.Extensions.Options;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormAutoMapping : IConfigureOptions<ManualMapping>
    {
        private readonly IVmPropertyMappingsFinder vmPropertyMappingsFinder;

        public FormAutoMapping(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
        {
            this.vmPropertyMappingsFinder = vmPropertyMappingsFinder;
        }

        public void Configure(ManualMapping options)
        {
            var formMappings = vmPropertyMappingsFinder.GetMappings<vmBlock_DataForm>();

            foreach (var i in formMappings)
            {
                if(i.SourceType != null)
                {
                    var resolverType = typeof(FormValueResolver<,>).MakeGenericType(i.SourceType, i.DestType);

                    options.ManualMaps.Add(new YuzuFullPropertyMapperSettings()
                    {
                        Mapper = typeof(IYuzuFullPropertyMapper<UmbracoMappingContext>),
                        Resolver = resolverType,
                        SourcePropertyName = i.SourcePropertyName,
                        DestPropertyName = i.DestPropertyName
                    });
                }
            }
        }
    }



}
