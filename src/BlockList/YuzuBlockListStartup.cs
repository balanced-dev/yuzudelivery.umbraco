using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Core;
using System.Reflection;

namespace YuzuDelivery.Umbraco.BlockList
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeAfter(typeof(YuzuImportComposer))]
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

            //Grid blocklist
            composition.Register<BlockListGridMapping>();
            composition.Register<BlockListRowsConverter>();
            composition.Register<BlockListGridConverter>();
            composition.Register<BlockListGridDataService>();

            composition.RegisterUnique<IGridSchemaCreationService, BlockListGridCreationService>();

            composition.Register(typeof(YuzuMappingConfig), typeof(BlockListAutoMapping));

            //MUST be transient lifetime
            composition.Register(typeof(IUpdateableVmBuilderConfig), typeof(BlockListGridVmBuilderConfig), Lifetime.Transient);
            composition.Register(typeof(IUpdateableImportConfiguration), typeof(BlockListGridImportConfig), Lifetime.Transient);

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
            GridRowConfigs.Add(new GridRowConfig(true, "FullWidthSection", "100", "100"));
            GridRowConfigs.Add(new GridRowConfig(false, "TwoColumnSection", "50", "50,50"));
            GridRowConfigs.Add(new GridRowConfig(false, "ThreeColumnSection", "33", "33,33,33"));
            GridRowConfigs.Add(new GridRowConfig(false, "FourColumnSection", "25", "25,25,25,25"));
        }
    }
}
