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
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Core.Mapping;
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

            builder.Services.RegisterBlockListStrategies(Assembly.GetEntryAssembly());

            //Global blocklist

            builder.Services.AddTransient<GuidFactory>();
            builder.Services.AddTransient<BlockListDataTypeFactory>();
            builder.Services.AddTransient<BlockListDbModelFactory>();
            builder.Services.AddTransient<BlockGridRowConfigToContent>();

            //Inline blocklist

            builder.Services.AddTransient<BlockListDataService>();

            builder.Services.AddTransient<BlockListToListOfObjectsTypeConvertor>();
            builder.Services.AddTransient<BlockListToObjectTypeConvertor>();
            builder.Services.AddTransient(typeof(BlockListToTypeConvertor<>));
            builder.Services.AddTransient(typeof(BlockListToListOfTypesConvertor<>));
            builder.Services.AddTransient(typeof(BlockListItemToTypeConvertor<,>));

            builder.Services.AddUnique<IInlineBlockCreator, BlockListEditorCreationService>();

            builder.Services.AddTransient<IConfigureOptions<ManualMapping>, BlockListAutoMapping>();
            builder.Services.AddTransient<IConfigureOptions<ManualMapping>, BlockListItemAutoMapping>();

            builder.Services.Configure<ManualMapping>(settings =>
            {
                // BlockListGridAutoMapping
                settings.ManualMaps.AddTypeReplace<BlockListRowsConverter>();
                settings.ManualMaps.AddTypeReplace<BlockGridConverter>();

                // BlockListInlineMapping
                settings.ManualMaps.AddTypeReplace<BlockListToObjectTypeConvertor>();
                settings.ManualMaps.AddTypeReplace<BlockListToListOfObjectsTypeConvertor>();
            });

            //Grid blocklist
            builder.Services.AddTransient<BlockListRowsConverter>();
            builder.Services.AddTransient<BlockGridConverter>();
            builder.Services.AddTransient<IBlockGridDataService, BlockGridDataService>();

            builder.Services.AddUnique<IGridSchemaCreationService, BlockGridCreationService>();



            builder.Services.Configure<ViewModelGenerationSettings>(settings =>
            {
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_DataGrid>();
                settings.ExcludeViewModelsAtGeneration.Add<vmSub_DataGridRow>();
                settings.ExcludeViewModelsAtGeneration.Add<vmSub_DataGridColumn>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_DataRows>();
                settings.ExcludeViewModelsAtGeneration.Add<vmSub_DataRowsRow>();

                settings.AddNamespacesAtGeneration.Add("YuzuDelivery.Umbraco.BlockList");
            });

            //MUST be transient lifetime

            builder.Services.AddOptions<ImportSettings>()
                   .Configure<IVmPropertyFinder>((settings, propertyFinder) =>
                   {
                       settings.IgnoreViewmodels.Add<vmBlock_DataRows>();
                       settings.IgnoreViewmodels.Add<vmBlock_DataGrid>();

                       settings.DataStructureProperties.Add("Grids", propertyFinder.GetProperties(typeof(vmBlock_DataGrid)));
                       settings.DataStructureProperties.Add("Rows", propertyFinder.GetProperties(typeof(vmBlock_DataRows)));

                       settings.GridRowConfigs.Clear();
                       //rowBuilder
                       settings.GridRowConfigs.Add(new GridRowConfig("RowItem", "12", "100", isRow: true));

                       //gridBuilder
                       settings.GridRowConfigs.Add(new GridRowConfig("FullWidthSection", "12", "100", isDefault: true));
                       settings.GridRowConfigs.Add(new GridRowConfig("TwoColumnSection", "6", "50,50"));
                       settings.GridRowConfigs.Add(new GridRowConfig("ThreeColumnSection", "4", "33,33,33"));
                       settings.GridRowConfigs.Add(new GridRowConfig("FourColumnSection", "3", "25,25,25,25"));
                   });

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
}

