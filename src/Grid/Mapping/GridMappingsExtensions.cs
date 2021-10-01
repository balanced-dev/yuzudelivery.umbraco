using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData;
using System.Linq.Expressions;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Skybrud.Umbraco.GridData.Models;
#else
using Skybrud.Umbraco.GridData;
#endif

namespace YuzuDelivery.Umbraco.Grid
{
    public static class GridMappingsExtensions
    {
        public static void AddGridWithRows<TSource, TDest>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataRows>> destMember, IYuzuDeliveryImportConfiguration importConfig)
        {
            AddGrid<GridRowConvertor<TSource, TDest>, TSource, TDest, vmBlock_DataRows>(resolvers, sourceMember, destMember);
        }

        public static void AddGridWithRows<TSource, TDest, TConfig>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataRows>> destMember, IYuzuDeliveryImportConfiguration importConfig)
        {
            AddGrid<GridRowConvertor<TSource, TDest, TConfig>, TSource, TDest, vmBlock_DataRows>(resolvers, sourceMember, destMember);
            AddConfig<TConfig>(resolvers, importConfig);
        }

        public static void AddGridRowsWithColumns<TSource, TDest>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember, IYuzuDeliveryImportConfiguration importConfig)
        {
            AddGrid<GridRowConvertor<TSource, TDest>, TSource, TDest, vmBlock_DataGrid>(resolvers, sourceMember, destMember);
        }

        public static void AddGridRowsWithColumns<TSource, TDest, TConfig>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember, IYuzuDeliveryImportConfiguration importConfig)
        {
            AddGrid<GridRowColumnConvertor<TSource, TDest, TDest, TConfig>, TSource, TDest, vmBlock_DataGrid>(resolvers, sourceMember, destMember);
            AddConfig<TConfig>(resolvers, importConfig);
        }

        public static void AddGridRowsWithColumns<TSource, TDest, TConfigRow, TConfigCol>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember, IYuzuDeliveryImportConfiguration importConfig)
        {
            AddGrid<GridRowColumnConvertor<TSource, TDest, TConfigRow, TConfigCol>, TSource, TDest, vmBlock_DataGrid>(resolvers, sourceMember, destMember);
            AddConfig<TConfigRow>(resolvers, importConfig);
            AddConfig<TConfigCol>(resolvers, importConfig);
        }

        private static void AddGrid<ResolverType, TSource, TDest, TDestMember>(List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, TDestMember>> destMember)
        {
            resolvers.Add(new YuzuFullPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuFullPropertyMapper),
                Resolver = typeof(ResolverType),
                SourcePropertyName = sourceMember.GetMemberName(),
                DestPropertyName = destMember.GetMemberName()
            });
        }

        private static void AddConfig<TConfig>(List<YuzuMapperSettings> resolvers, IYuzuDeliveryImportConfiguration importConfig)
        {
            importConfig.IgnoreViewmodels.Add<TConfig>();

            resolvers.Add(new YuzuTypeConvertorMapperSettings()
            {
                Mapper = typeof(IYuzuTypeConvertorMapper),
                Convertor = typeof(GridConfigConverter<TConfig>)
            });
        }
    }
}
