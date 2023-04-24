using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Core.Mapping.Mappers;
using YuzuDelivery.Umbraco.Core.Middleware;

namespace YuzuDelivery.Umbraco.Core
{
    public class YuzuStartup : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            Inflector.Inflector.SetDefaultCultureFunc = () => System.Threading.Thread.CurrentThread.CurrentUICulture;

            YuzuConstants.Initialize(new YuzuConstantsConfig());

            builder.Services.AddOptions<CoreSettings>()
                .Bind(builder.Config.GetSection("Yuzu:Core"))
                .ValidateDataAnnotations();

            builder.Services.AddSingleton<YuzuRenderJsonMiddleware>();
            builder.Services.AddSingleton<YuzuLoadedSchemaMiddleware>();
            builder.Services.Configure<UmbracoPipelineOptions>(opt =>
            {
                opt.AddFilter(new UmbracoPipelineFilter("YuzuMiddleware")
                {
                    PostPipeline = app => app.UseYuzuJsonMiddleware()
                });
            });

            builder.Services.AddOptions<ViewModelGenerationSettings>()
                   .Bind(builder.Config.GetSection("Yuzu:VmGeneration"))
                   .Configure<IHostEnvironment>((settings, env) =>
                   {
                       if (!Path.IsPathFullyQualified(settings.Directory))
                       {
                           settings.Directory = Path.GetFullPath(Path.Combine(env.ContentRootPath, settings.Directory));
                       }
                   })
                   .ValidateDataAnnotations()
                   .ValidateOnStart();

            builder.ManifestFilters().Append<YuzuManifestFilter>();

            //Viewmodel Builder
            builder.Services.AddSingleton<BuildViewModelsService>();
            builder.Services.AddSingleton<ReferencesService>();
            builder.Services.AddSingleton<GenerateViewmodelService>();

            builder.Services.Configure<YuzuConfiguration>(cfg => {
                cfg.MappingAssemblies.Add(GetType().Assembly);
            });

            builder.Services.AddOptions<ImportSettings>()
                   .Configure<IVmPropertyFinder>((settings, propertyFinder) =>
                   {
                       settings.IgnoreViewmodels.Add<vmBlock_DataImage>();
                       settings.IgnoreViewmodels.Add<vmBlock_DataLink>();

                       settings.DataStructureProperties["Images"] = propertyFinder.GetProperties(typeof(vmBlock_DataImage));
                       settings.DataStructureProperties["Links"] = propertyFinder.GetProperties(typeof(vmBlock_DataLink));
                   });


            builder.Services.Configure<ViewModelGenerationSettings>(settings =>
            {
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_DataImage>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_DataLink>();

                settings.AddNamespacesAtGeneration.Add("YuzuDelivery.Umbraco.Core");
            });

            builder.Services.AddSingleton<DefaultYuzuMapperFactory>();

            //ToDo: not sure what this is Umbraco 9
            //composition.RegisterAuto<AutoMapper.Profile>();

            builder.Services.AddTransient<LinkIPublishedContentConvertor>();
            builder.Services.AddSingleton<LinkConvertor>();
            builder.Services.AddTransient<ILinkFactory, LinkFactory>();
            builder.Services.AddTransient<ImageConvertor>();
            builder.Services.AddTransient<MediWithCropsConvertor>();
            builder.Services.AddTransient(typeof(SubBlocksObjectResolver<,>));

            builder.Services.AddTransient<DefaultPublishedElementCollectionConvertor>();
            builder.Services.AddTransient<DefaultPublishedElementCollectionToSingleConvertor>();

            builder.Services.AddTransient<IMappingContextFactory<UmbracoMappingContext>, UmbracoMappingContextFactory>();
            builder.Services.AddTransient<IYuzuTypeFactoryRunner, UmbracoTypeFactoryRunner>();

            builder.Services.AddTransient<ImageFactory>();

            builder.Services.Configure<ManualMapping>(settings =>
            {
                // Default
                settings.ManualMaps.AddTypeReplace<DefaultPublishedElementCollectionConvertor>(false);
                settings.ManualMaps.AddTypeReplace<DefaultPublishedElementCollectionToSingleConvertor>(false);

                // Images
                settings.ManualMaps.AddTypeReplace<ImageConvertor>(false);
                settings.ManualMaps.AddTypeReplace<MediWithCropsConvertor>(false);

                // Links
                settings.ManualMaps.AddTypeReplace<LinkIPublishedContentConvertor>(false);
                settings.ManualMaps.AddTypeReplace<LinkConvertor>(false);
            });

            builder.Services.AddTransient<IConfigureOptions<ManualMapping>, SubBlocksMappings>();
            builder.Services.AddTransient<IConfigureOptions<ManualMapping>, ManualMappingsMappings>();
            builder.Services.AddTransient<IConfigureOptions<ManualMapping>, GroupedConfigMappings>();
            builder.Services.AddTransient<IConfigureOptions<ManualMapping>, GlobalConfigMappings>();

            builder.Services.AddUnique<IYuzuGroupMapper, DefaultGroupMapper>();
            builder.Services.AddUnique<IYuzuGlobalMapper, UmbracoGlobalMapper>();
            builder.Services.AddUnique<IYuzuFullPropertyMapper<UmbracoMappingContext>, DefaultFullPropertyMapper<UmbracoMappingContext>>();
            builder.Services.AddUnique<IYuzuPropertyAfterMapper, DefaultPropertyAfterMapper>();
            builder.Services.AddUnique<IYuzuPropertyFactoryMapper<UmbracoMappingContext>, DefaultPropertyFactoryMapper<UmbracoMappingContext>>();
            builder.Services.AddUnique<IYuzuPropertyReplaceMapper<UmbracoMappingContext>, DefaultPropertyReplaceMapper<UmbracoMappingContext>>();
            builder.Services.AddUnique<IYuzuTypeAfterMapper<UmbracoMappingContext>, DefaultTypeAfterMapper<UmbracoMappingContext>>();
            builder.Services.AddUnique<IYuzuTypeReplaceMapper<UmbracoMappingContext>, DefaultTypeReplaceMapper<UmbracoMappingContext>>();
            builder.Services.AddUnique<IYuzuTypeFactoryMapper<UmbracoMappingContext>, DefaultTypeFactoryMapper<UmbracoMappingContext>>();

            builder.Services.AddTransient(typeof(GlobalMappingEnumerableTypeConverter<,>));

            builder.Services.AddTransient(typeof(IMapperAddItem), typeof(UmbracoMapperAddItems));

            builder.Services.RegisterYuzuAutoMapping();

            builder.Services.AddSingleton<IYuzuMappingIndex, YuzuUmbracoMappingIndex>();

            builder.Services.Configure<YuzuConfiguration>(cfg =>
            {
                var assembly = Assembly.GetEntryAssembly();
                cfg.AddToModelRegistry(assembly);
                cfg.AddInstalledManualMaps(assembly);
            });

            AddDefaultPublishedElements(builder);
        }

        public void AddDefaultPublishedElements(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IDefaultPublishedElement[]>((factory) =>
            {
                var config = factory.GetRequiredService<IOptions<YuzuConfiguration>>();
                var mapper = factory.GetService<IMapper>();
                var fallback = factory.GetService<IPublishedValueFallback>();

                var viewmodelAssemblies = config.Value.ViewModelAssemblies;

                var baseItemType = typeof(DefaultPublishedElement<,>);
                var items = new List<IDefaultPublishedElement>();

                var viewmodelTypes = config.Value.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix));

                foreach (var viewModelType in viewmodelTypes)
                {
                    var umbracoModelTypeName = viewModelType.GetModelName();
                    var umbracoModelType = config.Value.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

                    var alias = umbracoModelTypeName.FirstCharacterToLower();

                    if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                    {
                        var makeme = baseItemType.MakeGenericType(new Type[] { umbracoModelType, viewModelType });
                        var o = Activator.CreateInstance(makeme, new object[] { alias, mapper, fallback }) as IDefaultPublishedElement;

                        items.Add(o);
                    }
                }

                return items.ToArray();
            });
        }
    }
}
