using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData;
using AutoMapper;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace YuzuDelivery.Umbraco.Grid
{
    public class GridConfigConverter<T> : ITypeConverter<Dictionary<string, object>, T>
    {
        public T Convert(Dictionary<string, object> source, T destination, ResolutionContext context)
        {
            if (source == null)
                return default(T);

            var config = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(config);
        }
    }

    public class GridRowConvertor<TSource, TDest> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRows>
    {
        public vmBlock_DataGridRows Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRows destMember, ResolutionContext context)
        {
            var grids = DependencyResolver.Current.GetService<IGridService>();

            return grids.CreateRows(sourceMember, context);
        }
    }

    public class GridRowConvertor<TSource, TDest, TConfig> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRows>
    {
        public vmBlock_DataGridRows Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRows destMember, ResolutionContext context)
        {
            var grids = DependencyResolver.Current.GetService<IGridService>();

            return grids.CreateRows<TConfig>(sourceMember, context);
        }
    }

    public class GridRowColumnConvertor<TSource, TDest> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRowsColumns>
    {
        public vmBlock_DataGridRowsColumns Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRowsColumns destMember, ResolutionContext context)
        {
            var grids = DependencyResolver.Current.GetService<IGridService>();
            return grids.CreateRowsColumns(sourceMember, context);
        }
    }

    public class GridRowColumnConvertor<TSource, TDest, TConfig> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRowsColumns>
    {
        public vmBlock_DataGridRowsColumns Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRowsColumns destMember, ResolutionContext context)
        {
            var grids = DependencyResolver.Current.GetService<IGridService>();
            return grids.CreateRowsColumns<TConfig>(sourceMember, context);
        }
    }

    public class GridRowColumnConvertor<TSource, TDest, TConfigRow, TConfigCol> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRowsColumns>
    {
        public vmBlock_DataGridRowsColumns Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRowsColumns destMember, ResolutionContext context)
        {
            var grids = DependencyResolver.Current.GetService<IGridService>();
            return grids.CreateRowsColumns<TConfigRow, TConfigCol>(sourceMember, context);
        }
    }

}
