using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Composing;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using AutoMapper;

namespace YuzuDelivery.Umbraco.Grid 
{

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuGridStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            AddDefaultGridItems(composition);

            composition.Register<IGridService, GridService>(Lifetime.Singleton);

            composition.Register(typeof(GridRowConvertor<,>));
            composition.Register(typeof(GridRowConvertor<,,>));
            composition.Register(typeof(GridRowColumnConvertor<,,>));
            composition.Register(typeof(GridRowColumnConvertor<,,,>));

            //MUST be tranient lifetime
            composition.Register(typeof(IUpdateableVmBuilderConfig), typeof(GridVmBuilderConfig), Lifetime.Transient);

            GridContext.Current.Converters.Add(new DtgeGridConverter());
        }


        public void AddDefaultGridItems(Composition composition)
        {
            composition.Register<IGridItemInternal[]>((factory) =>
            {
                var config = factory.GetInstance<IYuzuConfiguration>();
                var mapper = factory.GetInstance<IMapper>();

                var baseGridType = typeof(DefaultGridItem<,>);
                var gridItems = new List<IGridItemInternal>();
                var viewmodelTypes = config.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix));

                foreach (var viewModelType in viewmodelTypes)
                {
                    var umbracoModelTypeName = viewModelType.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "");
                    var alias = umbracoModelTypeName.FirstCharacterToLower();
                    var umbracoModelType = config.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

                    if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                    {
                        var makeme = baseGridType.MakeGenericType(new Type[] { umbracoModelType, viewModelType });
                        var o = Activator.CreateInstance(makeme, new object[] { alias, mapper }) as IGridItemInternal;

                        gridItems.Add(o);
                    }
                }

                return gridItems.ToArray();
            }, Lifetime.Singleton);
        }
    }


    public static class StringExtensions
    {
        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }

    public class GridVmBuilderConfig : UpdateableVmBuilderConfig
    {
        public GridVmBuilderConfig()
            : base()
        {
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataGridRows>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataGridRowsColumns>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRowsRow>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRowsColumnsRow>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRowsColumnsColumn>();

            AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.Grid;");
        }
    }
}