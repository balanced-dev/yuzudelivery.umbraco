using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.GridData;
using AutoMapper;
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
        private readonly IGridService gridService;

        public GridRowConvertor(IGridService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGridRows Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRows destMember, ResolutionContext context)
        {
            return gridService.CreateRows(sourceMember, context);
        }
    }

    public class GridRowConvertor<TSource, TDest, TConfig> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRows>
    {
        private readonly IGridService gridService;

        public GridRowConvertor(IGridService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGridRows Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRows destMember, ResolutionContext context)
        {
            return gridService.CreateRows<TConfig>(sourceMember, context);
        }
    }

    public class GridRowColumnConvertor<TSource, TDest> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRowsColumns>
    {
        private readonly IGridService gridService;

        public GridRowColumnConvertor(IGridService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGridRowsColumns Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRowsColumns destMember, ResolutionContext context)
        {
            return gridService.CreateRowsColumns(sourceMember, context);
        }
    }

    public class GridRowColumnConvertor<TSource, TDest, TConfig> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRowsColumns>
    {
        private readonly IGridService gridService;

        public GridRowColumnConvertor(IGridService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGridRowsColumns Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRowsColumns destMember, ResolutionContext context)
        {
            return gridService.CreateRowsColumns<TConfig>(sourceMember, context);
        }
    }

    public class GridRowColumnConvertor<TSource, TDest, TConfigRow, TConfigCol> : IMemberValueResolver<TSource, TDest, GridDataModel, vmBlock_DataGridRowsColumns>
    {
        private readonly IGridService gridService;

        public GridRowColumnConvertor(IGridService gridService)
        {
            this.gridService = gridService;
        }

        public vmBlock_DataGridRowsColumns Resolve(TSource source, TDest destination, GridDataModel sourceMember, vmBlock_DataGridRowsColumns destMember, ResolutionContext context)
        {
            return gridService.CreateRowsColumns<TConfigRow, TConfigCol>(sourceMember, context);
        }
    }

}
