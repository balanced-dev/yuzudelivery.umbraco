using YuzuDelivery.Core;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;
using YuzuDelivery.Core.AutoMapper.Mappers;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Forms
{
    public class FormAutoMapping : YuzuMappingConfig
    {
        public FormAutoMapping(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
        {
            var formMappings = vmPropertyMappingsFinder.GetMappings<vmBlock_DataForm>();

            foreach (var i in formMappings)
            {
                if(i.SourceType != null)
                {
                    var resolverType = typeof(FormValueResolver<,>).MakeGenericType(i.SourceType, i.DestType);

                    ManualMaps.Add(new YuzuFullPropertyMapperSettings()
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
