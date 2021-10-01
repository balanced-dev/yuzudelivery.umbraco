using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Core;
using System.Reflection;

#if NETCOREAPP 
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.GridData.Composers;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Grid 
{

#if NETCOREAPP
    [ComposeAfter(typeof(YuzuUmbracoImportComposer))]
    public class YuzuGridStartup : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            AddDefaultGridItems(builder);

            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAll<IContentMapper>(assembly);

            builder.Services.AddSingleton<IGridService, GridService>();

            builder.Services.AddUnique<IGridSchemaCreationService, GridSchemaCreationService>();
            builder.Services.AddSingleton<IDTGEService, DTGEService>();

            builder.Services.AddTransient(typeof(GridRowConvertor<,>));
            builder.Services.AddTransient(typeof(GridRowConvertor<,,>));
            builder.Services.AddTransient(typeof(GridRowColumnConvertor<,>));
            builder.Services.AddTransient(typeof(GridRowColumnConvertor<,,>));
            builder.Services.AddTransient(typeof(GridRowColumnConvertor<,,,>));
            builder.Services.AddTransient(typeof(GridConfigConverter<>));

            //MUST be transient lifetime
            builder.Services.AddSingleton(typeof(IUpdateableVmBuilderConfig), typeof(GridVmBuilderConfig));
            builder.Services.AddSingleton(typeof(IUpdateableImportConfiguration), typeof(GridImportConfig));

            builder.Services.AddTransient(typeof(YuzuMappingConfig), typeof(GridAutoMapping));

            builder.GridConverters().Append<DtgeGridConverter>();
        }


        public void AddDefaultGridItems(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IGridItemInternal[]>((factory) =>
            {
                var config = factory.GetService<IYuzuConfiguration>();
                var mapper = factory.GetService<IMapper>();

                var baseGridType = typeof(DefaultGridItem<,>);
                var gridItems = new List<IGridItemInternal>();
                var typeFactoryRunner = factory.GetService<IYuzuTypeFactoryRunner>();
                var publishedValueFallback = factory.GetService<IPublishedValueFallback>();
                var viewmodelTypes = config.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix));

                foreach (var viewModelType in viewmodelTypes)
                {
                    var umbracoModelTypeName = viewModelType.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "");
                    var alias = umbracoModelTypeName.FirstCharacterToLower();
                    var umbracoModelType = config.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

                    if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                    {
                        var makeme = baseGridType.MakeGenericType(new Type[] { umbracoModelType, viewModelType });
                        var o = Activator.CreateInstance(makeme, new object[] { alias, mapper, typeFactoryRunner, publishedValueFallback }) as IGridItemInternal;

                        gridItems.Add(o);
                    }
                }

                return gridItems.ToArray();
            });
        }
    }
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeAfter(typeof(YuzuUmbracoImportComposer))]
    public class YuzuGridStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            AddDefaultGridItems(composition);

            var assembly = Assembly.GetExecutingAssembly();

            composition.RegisterAll<IContentMapper>(assembly);

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
#endif

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