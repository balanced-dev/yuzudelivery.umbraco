using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData;
using AutoMapper;
using System.Linq.Expressions;

namespace YuzuDelivery.Umbraco.Grid
{
    public static class GridMappingsService
    {
        public static void AddGridWithRows<TSource, TDest>(this Profile profile, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataRows>> destMember)
        {
            profile.CreateMap<TSource, TDest>()
                .ForMember(destMember, opt => opt.MapFrom<GridRowConvertor<TSource, TDest>, GridDataModel>(sourceMember));
        }

        public static void AddGridWithRows<TSource, TDest, TConfig>(this Profile profile, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataRows>> destMember)
        {
            profile.CreateMap<TSource, TDest>()
                .ForMember(destMember, opt => opt.MapFrom<GridRowConvertor<TSource, TDest, TConfig>, GridDataModel>(sourceMember));

            AddConfig<TConfig>(profile);
        }

        public static void AddGridRowsWithColumns<TSource, TDest>(this Profile profile, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember)
        {
            profile.CreateMap<TSource, TDest>()
                .ForMember(destMember, opt => opt.MapFrom<GridRowColumnConvertor<TSource, TDest>, GridDataModel>(sourceMember));
        }

        public static void AddGridRowsWithColumns<TSource, TDest, TConfig>(this Profile profile, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember)
        {
            profile.CreateMap<TSource, TDest>()
                .ForMember(destMember, opt => opt.MapFrom<GridRowColumnConvertor<TSource, TDest, TConfig>, GridDataModel>(sourceMember));

            AddConfig<TConfig>(profile);
        }

        public static void AddGridRowsWithColumns<TSource, TDest, TConfigRow, TConfigCol>(this Profile profile, Expression<Func<TSource, GridDataModel>> sourceMember, Expression<Func<TDest, vmBlock_DataGrid>> destMember)
        {
            profile.CreateMap<TSource, TDest>()
                .ForMember(destMember, opt => opt.MapFrom<GridRowColumnConvertor<TSource, TDest, TConfigRow, TConfigCol>, GridDataModel>(sourceMember));

            AddConfig<TConfigRow>(profile);
            AddConfig<TConfigCol>(profile);
        }

        private static void AddConfig<TConfig>(Profile profile)
        {
            profile.CreateMap<Dictionary<string, object>, TConfig>()
                .ConvertUsing<GridConfigConverter<TConfig>>();
        }
    }

}
