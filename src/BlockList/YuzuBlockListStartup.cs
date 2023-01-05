using System;
using System.Collections.Generic;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;
using System.Reflection;
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Umbraco.Import.Settings;

namespace YuzuDelivery.Umbraco.BlockList
{

    [ComposeAfter(typeof(YuzuUmbracoImportComposer))]
    public class YuzuBlockListStartup : IComposer
    {

        public void Compose(IUmbracoBuilder builder)
        {
            AddDefaultGridItems(builder);

            var assembly = Assembly.GetExecutingAssembly();

            builder.Services.RegisterAll<IContentMapper>(assembly);

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
            builder.Services.AddTransient<IBlockListGridDataService, BlockListGridDataService>();

            builder.Services.AddUnique<IGridSchemaCreationService, BlockListGridCreationService>();

            builder.Services.AddTransient<YuzuMappingConfig, BlockListInlineMapping>();
            builder.Services.AddTransient(typeof(YuzuMappingConfig), typeof(BlockListGridAutoMapping));

            //MUST be transient lifetime
            builder.Services.AddTransient(typeof(IUpdateableVmBuilderConfig), typeof(BlockListGridVmBuilderConfig));
            builder.Services.AddTransient(typeof(IUpdateableImportConfiguration), typeof(BlockListGridImportConfig));

            builder.ManifestFilters().Append<YuzuBlockListManifestFilter>();

            builder.Services.AddOptions<DataTypeFolderSettings>("YuzuDelivery.Umbraco.BlockList")
                    .Configure(x =>
                    {
                        x.GetCustomFolderName = (name, editor) =>
                        {
                            if (editor == Constants.PropertyEditors.Aliases.BlockList && name.Contains("Builder"))
                            {
                                return "Grid";
                            }

                            return null;
                        };
                    });

        }

        public void AddDefaultGridItems(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IEnumerable<IGridItemInternal>>((factory) =>
            {
                var config = factory.GetRequiredService<IOptions<YuzuConfiguration>>();

                var baseGridType = typeof(DefaultGridItem<,>);
                var gridItems = new List<IGridItemInternal>();
                var viewmodelTypes = config.Value.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix));

                foreach (var viewModelType in viewmodelTypes)
                {
                    var umbracoModelTypeName = viewModelType.GetModelName();
                    var alias = umbracoModelTypeName.FirstCharacterToLower();
                    var umbracoModelType = config.Value.CMSModels.FirstOrDefault(x => x.Name == umbracoModelTypeName);

                    if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                    {
                        var gridItemType = baseGridType.MakeGenericType(umbracoModelType, viewModelType );
                        var o = ActivatorUtilities.CreateInstance(factory, instanceType: gridItemType, parameters: alias)as IGridItemInternal;

                        gridItems.Add(o);
                    }
                }

                return gridItems;
            });
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

            AddNamespacesAtGeneration.Add("YuzuDelivery.Umbraco.BlockList");
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
            //rowBuilder
            GridRowConfigs.Add(new GridRowConfig("RowItem", "12", "100", isRow: true));

            //gridBuilder
            GridRowConfigs.Add(new GridRowConfig("FullWidthSection", "12", "100", isDefault: true));
            GridRowConfigs.Add(new GridRowConfig("TwoColumnSection", "6", "50,50"));
            GridRowConfigs.Add(new GridRowConfig("ThreeColumnSection", "4", "33,33,33"));
            GridRowConfigs.Add(new GridRowConfig("FourColumnSection", "3", "25,25,25,25"));
        }
    }
}
