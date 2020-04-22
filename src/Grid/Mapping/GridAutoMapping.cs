using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridAutoMapping : YuzuMappingConfig
    {
        private readonly ISchemaMetaService schemaMetaService;
        private readonly IYuzuConfiguration config;
        private readonly IYuzuDeliveryImportConfiguration importConfig;

        public GridAutoMapping(IVmPropertyMappingsFinder vmPropertyMappingsFinder, ISchemaMetaService schemaMetaService, IYuzuConfiguration config, IYuzuDeliveryImportConfiguration importConfig)
        {
            this.schemaMetaService = schemaMetaService;
            this.config = config;
            this.importConfig = importConfig;

            var rowMappings = vmPropertyMappingsFinder.GetMappings<vmBlock_DataRows>();

            foreach (var i in rowMappings)
            {
                var configType = AddConfigMapping(i.DestProperty, ConfigType.Rows);
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
                var rowsConfigType = AddConfigMapping(i.DestProperty, ConfigType.Rows);
                var colsConfigType = AddConfigMapping(i.DestProperty, ConfigType.Cells);

                Type resolverType = null;

                if (rowsConfigType == null && colsConfigType == null)
                    resolverType = typeof(GridRowColumnConvertor<,>).MakeGenericType(i.SourceType, i.DestType);
                else if(colsConfigType == null)
                    resolverType = typeof(GridRowColumnConvertor<,,>).MakeGenericType(i.SourceType, i.DestType, rowsConfigType);
                else
                    resolverType = typeof(GridRowColumnConvertor<,,,>).MakeGenericType(i.SourceType, i.DestType, rowsConfigType, colsConfigType);

                AddResolverMapping(i, resolverType);
            }

        }

        private void AddResolverMapping(VmPropertyMappingsFinder.Settings i, Type resolverType)
        {
            ManualMaps.Add(new YuzuFullPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuFullPropertyMapper),
                Resolver = resolverType,
                SourcePropertyName = i.SourcePropertyName,
                DestPropertyName = i.DestPropertyName
            });
        }

        private Type AddConfigMapping(PropertyInfo destProperty, ConfigType type)
        {
            var path = "/rows/config";
            if(type == ConfigType.Cells)
                path = "/rows/columns/config";

            var ofType = schemaMetaService.GetOfType(destProperty, "refs");
            if(!string.IsNullOrEmpty(ofType))
            {
                var configTypeName = schemaMetaService.Get(typeof(vmBlock_DataRows), "refs", path, ofType).FirstOrDefault();

                if(!string.IsNullOrEmpty(configTypeName))
                {

                    var configType = config.ViewModels.Where(x => x.Name == configTypeName).FirstOrDefault();

                    if(configType != null)
                    {
                        var configResolverType = typeof(GridConfigConverter<>).MakeGenericType(configType);
                        importConfig.IgnoreViewmodels.Add(configType.Name);

                        ManualMaps.Add(new YuzuTypeConvertorMapperSettings()
                        {
                            Mapper = typeof(IYuzuTypeConvertorMapper),
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
