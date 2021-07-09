using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData;
using System.Linq.Expressions;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using System.Web.Mvc;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Grid
{
    public static class GridMappingsExtensions
    {
        public static void AddGridWithRows<TSource, TDest>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataRows>> destMember)
        {
            AddGrid<GridRowConvertor<TSource, TDest>, TSource, TDest, vmBlock_DataRows>(resolvers, sourceMember, destMember);
        }

        public static void AddGridWithRows<TSource, TDest, TConfig>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataRows>> destMember)
        {
            AddGrid<GridRowConvertor<TSource, TDest, TConfig>, TSource, TDest, vmBlock_DataRows>(resolvers, sourceMember, destMember);
            AddConfig<TConfig>(resolvers);
        }

        public static void AddGridRowsWithColumns<TSource, TDest>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember)
        {
            AddGrid<GridRowConvertor<TSource, TDest>, TSource, TDest, vmBlock_DataGrid>(resolvers, sourceMember, destMember);
        }

        public static void AddGridRowsWithColumns<TSource, TDest, TConfig>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember)
        {
            AddGrid<GridRowColumnConvertor<TSource, TDest, TDest, TConfig>, TSource, TDest, vmBlock_DataGrid>(resolvers, sourceMember, destMember);
            AddConfig<TConfig>(resolvers);
        }

        public static void AddGridRowsWithColumns<TSource, TDest, TConfigRow, TConfigCol>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember)
        {
            AddGrid<GridRowColumnConvertor<TSource, TDest, TConfigRow, TConfigCol>, TSource, TDest, vmBlock_DataGrid>(resolvers, sourceMember, destMember);
            AddConfig<TConfigRow>(resolvers);
            AddConfig<TConfigCol>(resolvers);
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

        private static void AddConfig<TConfig>(List<YuzuMapperSettings> resolvers)
        {
            var config = DependencyResolver.Current.GetService<IYuzuDeliveryImportConfiguration>();
            config.IgnoreViewmodels.Add<TConfig>();

            resolvers.Add(new YuzuTypeConvertorMapperSettings()
            {
                Mapper = typeof(IYuzuTypeConvertorMapper),
                Convertor = typeof(GridConfigConverter<TConfig>)
            });
        }
    }
}
