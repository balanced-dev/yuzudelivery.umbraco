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
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Grid 
{

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeAfter(typeof(YuzuImportComposer))]
    public class YuzuGridStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            AddDefaultGridItems(composition);

            composition.Register<IGridService, GridService>(Lifetime.Singleton);

            composition.RegisterUnique<IGridSchemaCreationService, GridSchemaCreationService>();
            composition.Register<IDTGEService, DTGEService>(Lifetime.Singleton);

            composition.Register(typeof(GridRowConvertor<,>));
            composition.Register(typeof(GridRowConvertor<,,>));
            composition.Register(typeof(GridRowColumnConvertor<,>));
            composition.Register(typeof(GridRowColumnConvertor<,,>));
            composition.Register(typeof(GridRowColumnConvertor<,,,>));
            composition.Register(typeof(GridConfigConverter<>));

            //MUST be transient lifetime
            composition.Register(typeof(IUpdateableVmBuilderConfig), typeof(GridVmBuilderConfig), Lifetime.Transient);
            composition.Register(typeof(IUpdateableImportConfiguration), typeof(GridImportConfig), Lifetime.Transient);

            composition.Register(typeof(YuzuMappingConfig), typeof(GridAutoMapping));

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

    public class GridVmBuilderConfig : UpdateableVmBuilderConfig
    {
        public GridVmBuilderConfig()
            : base()
        {
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataGrid>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRow>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridColumn>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataRows>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataRowsRow>();

            AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.Grid;");
        }
    }

    public class GridImportConfig : UpdateableImportConfiguration
    {
        public GridImportConfig(IVmPropertyFinder vmPropertyFinder)
            : base()
        {
            IgnoreViewmodels.Add<vmBlock_DataRows>();
            IgnoreViewmodels.Add<vmBlock_DataGrid>();

            SpecialistProperties.Add("Grids", vmPropertyFinder.GetProperties(typeof(vmBlock_DataGrid)));
            SpecialistProperties.Add("Rows", vmPropertyFinder.GetProperties(typeof(vmBlock_DataRows)));
        }
    }
}