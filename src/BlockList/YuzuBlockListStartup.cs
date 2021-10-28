using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Core;
using System.Reflection;

#if NETCOREAPP 
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{

#if NETCOREAPP
    [ComposeAfter(typeof(YuzuUmbracoImportComposer))]
    public class YuzuBlockListStartup : IComposer
    {

        public void Compose(IUmbracoBuilder builder)
        {
            AddDefaultGridItems(builder);

            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAll<IContentMapper>(assembly);

            //Global blocklist

            builder.Services.AddTransient<GuidFactory>();
            builder.Services.AddTransient<BlockListDataTypeFactory>();
            builder.Services.AddTransient<BlockListDbModelFactory>();
            builder.Services.AddTransient<BlockListGridRowConfigToContent>();

            //Inline blocklist

            builder.Services.AddTransient<BlockListDataService>();

            builder.Services.AddTransient<BlockListToListOfObjectsTypeConvertor>();
            builder.Services.AddTransient<BlockListToObjectTypeConvertor>();
            builder.Services.AddTransient(typeof(BlockListToTypeConvertor<>));
            builder.Services.AddTransient(typeof(BlockListToListOfTypesConvertor<>));

            builder.Services.AddUnique<IInlineBlockCreator, BlockListEditorCreationService>();

            builder.Services.AddTransient(typeof(YuzuMappingConfig), typeof(BlockListAutoMapping));

            //Grid blocklist
            builder.Services.AddTransient<BlockListRowsConverter>();
            builder.Services.AddTransient<BlockListGridConverter>();
            builder.Services.AddTransient<BlockListGridDataService>();

            builder.Services.AddUnique<IGridSchemaCreationService, BlockListGridCreationService>();

            builder.Services.AddTransient(typeof(YuzuMappingConfig), typeof(BlockListGridAutoMapping));

            //MUST be transient lifetime
            builder.Services.AddTransient(typeof(IUpdateableVmBuilderConfig), typeof(BlockListGridVmBuilderConfig));
            builder.Services.AddTransient(typeof(IUpdateableImportConfiguration), typeof(BlockListGridImportConfig));

        }

        public void AddDefaultGridItems(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IEnumerable<IGridItemInternal>>((factory) =>
            {
                var config = factory.GetService<IYuzuConfiguration>();
                var mapper = factory.GetService<IMapper>();
                var typeFactoryRunner = factory.GetService<IYuzuTypeFactoryRunner>();
                var publishedValueFallback = factory.GetService<IPublishedValueFallback>();

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
                        var o = Activator.CreateInstance(makeme, new object[] { alias, mapper, typeFactoryRunner, publishedValueFallback }) as IGridItemInternal;

                        gridItems.Add(o);
                    }
                }

                return gridItems;
            });
        }
    }

#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeAfter(typeof(YuzuUmbracoImportComposer))]
    public class YuzuBlockListStartup : IUserComposer
    {

        public void Compose(Composition composition)
        {
            AddDefaultGridItems(composition);

            var assembly = Assembly.GetExecutingAssembly();

            composition.RegisterAll<IContentMapper>(assembly);

            //Global blocklist

            composition.Register<GuidFactory>();
            composition.Register<BlockListDataTypeFactory>();
            composition.Register<BlockListDbModelFactory>();
            composition.Register<BlockListGridRowConfigToContent>();

            //Inline blocklist

            composition.Register<BlockListDataService>();

            composition.Register<BlockListToListOfObjectsTypeConvertor>();
            composition.Register<BlockListToObjectTypeConvertor>();
            composition.Register(typeof(BlockListToTypeConvertor<>));
            composition.Register(typeof(BlockListToListOfTypesConvertor<>));

            composition.RegisterUnique<IInlineBlockCreator, BlockListEditorCreationService>();

            composition.Register(typeof(YuzuMappingConfig), typeof(BlockListAutoMapping));

            //Grid blocklist
            composition.Register<BlockListRowsConverter>();
            composition.Register<BlockListGridConverter>();
            composition.Register<BlockListGridDataService>();

            composition.RegisterUnique<IGridSchemaCreationService, BlockListGridCreationService>();

            composition.Register(typeof(YuzuMappingConfig), typeof(BlockListGridAutoMapping));

            //MUST be transient lifetime
            composition.Register(typeof(IUpdateableVmBuilderConfig), typeof(BlockListGridVmBuilderConfig), Lifetime.Transient);
            composition.Register(typeof(IUpdateableImportConfiguration), typeof(BlockListGridImportConfig), Lifetime.Transient);

        }

        public void AddDefaultGridItems(Composition composition)
        {
            composition.Register<IEnumerable<IGridItemInternal>>((factory) =>
            {
                var config = factory.GetInstance<IYuzuConfiguration>();
                var mapper = factory.GetInstance<IMapper>();
                var typeFactoryRunner = factory.GetInstance<IYuzuTypeFactoryRunner>();

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
                        var o = Activator.CreateInstance(makeme, new object[] { alias, mapper, typeFactoryRunner }) as IGridItemInternal;

                        gridItems.Add(o);
                    }
                }

                return gridItems;
            }, Lifetime.Singleton);
        }
    }
#endif

    public static class StringExtensions
    {
        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

    }

    public class BlockListGridVmBuilderConfig : UpdateableVmBuilderConfig
    {
        public BlockListGridVmBuilderConfig()
            : base()
        {
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataGrid>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRow>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridColumn>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataRows>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataRowsRow>();

            AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.BlockList;");
        }
    }

    public class BlockListGridImportConfig : UpdateableImportConfiguration
    {
        public BlockListGridImportConfig(IVmPropertyFinder vmPropertyFinder)
            : base()
        {
            IgnoreViewmodels.Add<vmBlock_DataRows>();
            IgnoreViewmodels.Add<vmBlock_DataGrid>();

            SpecialistProperties.Add("Grids", vmPropertyFinder.GetProperties(typeof(vmBlock_DataGrid)));
            SpecialistProperties.Add("Rows", vmPropertyFinder.GetProperties(typeof(vmBlock_DataRows)));

            GridRowConfigs = new List<GridRowConfig>();
            GridRowConfigs.Add(new GridRowConfig(true, "FullWidthSection", "12", "100"));
            GridRowConfigs.Add(new GridRowConfig(false, "TwoColumnSection", "6", "50,50"));
            GridRowConfigs.Add(new GridRowConfig(false, "ThreeColumnSection", "4", "33,33,33"));
            GridRowConfigs.Add(new GridRowConfig(false, "FourColumnSection", "3", "25,25,25,25"));
        }
    }
}
