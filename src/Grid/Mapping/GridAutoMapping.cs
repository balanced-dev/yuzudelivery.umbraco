using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Core.Mapping.Mappers.Settings;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridAutoMapping : YuzuMappingConfig
    {
        private readonly ISchemaMetaService schemaMetaService;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly IYuzuDeliveryImportConfiguration importConfig;

        public GridAutoMapping(IVmPropertyMappingsFinder vmPropertyMappingsFinder, ISchemaMetaService schemaMetaService, IOptions<YuzuConfiguration> config, IYuzuDeliveryImportConfiguration importConfig)
        {
            this.schemaMetaService = schemaMetaService;
            this.config = config;
            this.importConfig = importConfig;

            var rowMappings = vmPropertyMappingsFinder.GetMappings<vmBlock_DataRows>();

            foreach (var i in rowMappings)
            {
                var configType = AddConfigMapping<vmBlock_DataRows>(i.DestProperty, ConfigType.Rows);
                Type resolverType = null;

                if(i.SourceType != null)
                {
                    if (configType == null)
                        resolverType = typeof(GridRowConvertor<,>).MakeGenericType(i.SourceType, i.DestType);
                    else
                        resolverType = typeof(GridRowConvertor<,,>).MakeGenericType(i.SourceType, i.DestType, configType);

                    AddResolverMapping(i, resolverType);
                }
            }

            var gridMappings = vmPropertyMappingsFinder.GetMappings<vmBlock_DataGrid>();

            foreach (var i in gridMappings)
            {
                var rowsConfigType = AddConfigMapping<vmBlock_DataGrid>(i.DestProperty, ConfigType.Rows);
                var colsConfigType = AddConfigMapping<vmBlock_DataGrid>(i.DestProperty, ConfigType.Cells);

                Type resolverType = null;

                if (i.SourceType != null)
                {
                    if (rowsConfigType == null && colsConfigType == null)
                        resolverType = typeof(GridRowColumnConvertor<,>).MakeGenericType(i.SourceType, i.DestType);
                    else if (colsConfigType == null)
                        resolverType = typeof(GridRowColumnConvertor<,,>).MakeGenericType(i.SourceType, i.DestType, rowsConfigType);
                    else
                        resolverType = typeof(GridRowColumnConvertor<,,,>).MakeGenericType(i.SourceType, i.DestType, rowsConfigType, colsConfigType);

                    AddResolverMapping(i, resolverType);
                }
            }
        }

        private void AddResolverMapping(VmPropertyMappingsFinder.Settings i, Type resolverType)
        {
            ManualMaps.Add(new YuzuFullPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuFullPropertyMapper<UmbracoMappingContext>),
                Resolver = resolverType,
                SourcePropertyName = i.SourcePropertyName,
                DestPropertyName = i.DestPropertyName
            });
        }

        private Type AddConfigMapping<V>(PropertyInfo destProperty, ConfigType type)
        {
            var path = "/rows/config";
            if(type == ConfigType.Cells)
                path = "/rows/columns/config";

            var ofType = schemaMetaService.GetOfType(destProperty, "refs");
            if(!string.IsNullOrEmpty(ofType))
            {
                var configTypeName = schemaMetaService.Get(typeof(V), "refs", path, ofType).FirstOrDefault();

                if(!string.IsNullOrEmpty(configTypeName))
                {

                    var configType = config.Value.ViewModels.Where(x => x.Name == configTypeName).FirstOrDefault();

                    if(configType != null)
                    {
                        var configResolverType = typeof(GridConfigConverter<>).MakeGenericType(configType);
                        importConfig.IgnoreViewmodels.Add(configType.Name);

                        ManualMaps.Add(new YuzuTypeConvertorMapperSettings()
                        {
                            Mapper = typeof(IYuzuTypeReplaceMapper<UmbracoMappingContext>),
                            Convertor = configResolverType
                        });

                        return configType;
                    }
                }
            }

            return null;
        }

        private enum ConfigType
        {
            Rows,
            Cells
        }

    }

}
