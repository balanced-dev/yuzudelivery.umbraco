using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;

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

namespace YuzuDelivery.Umbraco.Core
{

#if NETCOREAPP
    public class YuzuStartup : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            Inflector.Inflector.SetDefaultCultureFunc = () => System.Threading.Thread.CurrentThread.CurrentUICulture;

            YuzuConstants.Initialize(new YuzuConstantsConfig());

            builder.Services.AddOptions<CoreSettings>()
                .Bind(builder.Config.GetSection("Yuzu:Core"))
                .ValidateDataAnnotations();

            builder.Services.AddOptions<VmGenerationSettings>()
                .Bind(builder.Config.GetSection("Yuzu:VmGeneration"))
                .ValidateDataAnnotations();

            builder.Services.AddSingleton<IHandlebarsProvider, HandlebarsProvider>();
            builder.Services.AddTransient<IYuzuDefinitionTemplates, YuzuDefinitionTemplates>();
            builder.Services.AddSingleton<IYuzuDefinitionTemplateSetup, YuzuDefinitionTemplateSetup>();
            builder.Services.AddTransient<ISchemaMetaService, SchemaMetaService>();
            builder.Services.AddTransient<ISchemaMetaPropertyService, SchemaMetaPropertyService>();

            //Viewmodel Builder
            builder.Services.AddSingleton<BuildViewModelsService>();
            builder.Services.AddSingleton<ReferencesService>();
            builder.Services.AddSingleton<GenerateViewmodelService>();
            builder.Services.AddTransient(typeof(IViewmodelPostProcessor), typeof(FileRefViewmodelPostProcessor));

            //MUST be transient lifetime
            builder.Services.AddTransient(typeof(IUpdateableConfig), typeof(CoreUmbracoConfig));
            builder.Services.AddTransient(typeof(IUpdateableVmBuilderConfig), typeof(CoreVmBuilderConfig));
            builder.Services.AddTransient(typeof(IUpdateableImportConfiguration), typeof(CoreImportConfig));

            builder.Services.AddSingleton<DefaultUmbracoMappingFactory>();

            //ToDo: not sure what this is Umbraco 9
            //composition.RegisterAuto<AutoMapper.Profile>();

            builder.Services.AddTransient<LinkIPublishedContentConvertor>();
            builder.Services.AddSingleton<LinkConvertor>();
            builder.Services.AddTransient<ImageConvertor>();
            builder.Services.AddTransient<MediWithCropsConvertor>();
            builder.Services.AddTransient(typeof(SubBlocksObjectResolver<,>));

            builder.Services.AddTransient<DefaultPublishedElementCollectionConvertor>();
            builder.Services.AddTransient<DefaultPublishedElementCollectionToSingleConvertor>();

            builder.Services.AddTransient<IMappingContextFactory, UmbracoMappingContextFactory>();
            builder.Services.AddTransient<IYuzuTypeFactoryRunner, UmbracoTypeFactoryRunner>();

            builder.Services.AddTransient<ImageFactory>();

            builder.Services.AddSingleton(typeof(YuzuMappingConfig), typeof(DefaultElementMapping));
            builder.Services.AddSingleton(typeof(YuzuMappingConfig), typeof(ImageMappings));
            builder.Services.AddSingleton(typeof(YuzuMappingConfig), typeof(LinkMappings));
            builder.Services.AddSingleton(typeof(YuzuMappingConfig), typeof(SubBlocksMappings));
            builder.Services.AddSingleton(typeof(YuzuMappingConfig), typeof(ManualMappingsMappings));
            builder.Services.AddSingleton(typeof(YuzuMappingConfig), typeof(GroupedConfigMappings));
            builder.Services.AddSingleton(typeof(YuzuMappingConfig), typeof(GlobalConfigMappings));

            builder.Services.AddUnique<IYuzuGroupMapper, DefaultGroupMapper>();
            builder.Services.AddUnique<IYuzuGlobalMapper, DefaultGlobalMapper>();
            builder.Services.AddUnique<IYuzuFullPropertyMapper, DefaultFullPropertyMapper>();
            builder.Services.AddUnique<IYuzuPropertyAfterMapper, DefaultPropertyAfterMapper>();
            builder.Services.AddUnique<IYuzuPropertyFactoryMapper, DefaultPropertyFactoryMapper>();
            builder.Services.AddUnique<IYuzuPropertyReplaceMapper, DefaultPropertyReplaceMapper>();
            builder.Services.AddUnique<IYuzuTypeAfterMapper, DefaultTypeAfterMapper>();
            builder.Services.AddUnique<IYuzuTypeConvertorMapper, DefaultTypeConvertorMapper>();
            builder.Services.AddUnique<IYuzuTypeFactoryMapper, DefaultTypeFactoryMapper>();

            builder.Services.AddTransient(typeof(IMapperAddItem), typeof(UmbracoMapperAddItems));

            builder.Services.AddTransient<SettingsAbstraction>();

            AddDefaultPublishedElements(builder);
            SetupHbsHelpers();
        }

        public void AddDefaultPublishedElements(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IDefaultPublishedElement[]>((factory) =>
            {
                var config = factory.GetService<IYuzuConfiguration>();
                var mapper = factory.GetService<IMapper>();
                var fallback = factory.GetService<IPublishedValueFallback>();

                var viewmodelAssemblies = config.ViewModelAssemblies;

                var baseItemType = typeof(DefaultPublishedElement<,>);
                var items = new List<IDefaultPublishedElement>();

                var viewmodelTypes = config.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix));

                foreach (var viewModelType in viewmodelTypes)
                {
                    var umbracoModelTypeName = viewModelType.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "");
                    var umbracoModelType = config.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

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
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            Inflector.Inflector.SetDefaultCultureFunc = () => System.Threading.Thread.CurrentThread.CurrentUICulture;

            YuzuConstants.Initialize(new YuzuConstantsConfig());

            composition.Register<IHandlebarsProvider, HandlebarsProvider>(Lifetime.Singleton);
            composition.Register<IYuzuDefinitionTemplates, YuzuDefinitionTemplates>(Lifetime.Singleton);
            composition.Register<IYuzuDefinitionTemplateSetup, YuzuDefinitionTemplateSetup>(Lifetime.Singleton);
            composition.Register<ISchemaMetaService, SchemaMetaService>();
            composition.Register<ISchemaMetaPropertyService, SchemaMetaPropertyService>();

            //Viewmodel Builder
            composition.Register<BuildViewModelsService>(Lifetime.Singleton);
            composition.Register<ReferencesService>(Lifetime.Singleton);
            composition.Register<GenerateViewmodelService>(Lifetime.Singleton);
            composition.Register(typeof(IViewmodelPostProcessor), typeof(FileRefViewmodelPostProcessor));

            //MUST be transient lifetime
            composition.Register(typeof(IUpdateableConfig), typeof(CoreUmbracoConfig), Lifetime.Transient);
            composition.Register(typeof(IUpdateableVmBuilderConfig), typeof(CoreVmBuilderConfig), Lifetime.Transient);
            composition.Register(typeof(IUpdateableImportConfiguration), typeof(CoreImportConfig), Lifetime.Transient);

            composition.Register<DefaultUmbracoMappingFactory>();
            composition.RegisterAuto<AutoMapper.Profile>();

            composition.Register<LinkIPublishedContentConvertor>();
            composition.Register<LinkConvertor>();
            composition.Register<ImageConvertor>();
            composition.Register<MediWithCropsConvertor>();
            composition.Register(typeof(SubBlocksObjectResolver<,>));

            composition.Register<DefaultPublishedElementCollectionConvertor>();
            composition.Register<DefaultPublishedElementCollectionToSingleConvertor>();

            composition.Register<IMappingContextFactory, UmbracoMappingContextFactory>(Lifetime.Request);
            composition.Register<IYuzuTypeFactoryRunner, UmbracoTypeFactoryRunner>();

            composition.Register<ImageFactory>();

            composition.Register(typeof(YuzuMappingConfig), typeof(DefaultElementMapping));
            composition.Register(typeof(YuzuMappingConfig), typeof(ImageMappings));
            composition.Register(typeof(YuzuMappingConfig), typeof(LinkMappings));
            composition.Register(typeof(YuzuMappingConfig), typeof(SubBlocksMappings));
            composition.Register(typeof(YuzuMappingConfig), typeof(ManualMappingsMappings));
            composition.Register(typeof(YuzuMappingConfig), typeof(GroupedConfigMappings));
            composition.Register(typeof(YuzuMappingConfig), typeof(GlobalConfigMappings));

            composition.RegisterUnique<IYuzuGroupMapper, DefaultGroupMapper>();
            composition.RegisterUnique<IYuzuGlobalMapper, DefaultGlobalMapper>();
            composition.RegisterUnique<IYuzuFullPropertyMapper, DefaultFullPropertyMapper>();
            composition.RegisterUnique<IYuzuPropertyAfterMapper, DefaultPropertyAfterMapper>();
            composition.RegisterUnique<IYuzuPropertyFactoryMapper, DefaultPropertyFactoryMapper>();
            composition.RegisterUnique<IYuzuPropertyReplaceMapper, DefaultPropertyReplaceMapper>();
            composition.RegisterUnique<IYuzuTypeAfterMapper, DefaultTypeAfterMapper>();
            composition.RegisterUnique<IYuzuTypeConvertorMapper, DefaultTypeConvertorMapper>();
            composition.RegisterUnique<IYuzuTypeFactoryMapper, DefaultTypeFactoryMapper>();

            composition.Register(typeof(IMapperAddItem), typeof(UmbracoMapperAddItems));

            composition.Register<SettingsAbstraction>();

            AddDefaultPublishedElements(composition);
            SetupHbsHelpers();
        }

        public void AddDefaultPublishedElements(Composition composition)
        {
            composition.Register<IDefaultPublishedElement[]>((factory) =>
            {
                var config = factory.GetInstance<IYuzuConfiguration>();
                var mapper = factory.GetInstance<IMapper>();

                var viewmodelAssemblies = config.ViewModelAssemblies;

                var baseItemType = typeof(DefaultPublishedElement<,>);
                var items = new List<IDefaultPublishedElement>();

                var viewmodelTypes = config.ViewModels.Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix));

                foreach (var viewModelType in viewmodelTypes)
                {
                    var umbracoModelTypeName = viewModelType.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "");
                    var umbracoModelType = config.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

                    var alias = umbracoModelTypeName.FirstCharacterToLower();

                    if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                    {
                        var makeme = baseItemType.MakeGenericType(new Type[] { umbracoModelType, viewModelType });
                        var o = Activator.CreateInstance(makeme, new object[] { alias, mapper }) as IDefaultPublishedElement;

                        items.Add(o);
                    }
                }

                return items.ToArray();
            }, Lifetime.Singleton);
        }

#endif
        public void SetupHbsHelpers()
        {
            new IfCond();
            new YuzuDelivery.Core.Array();
            new YuzuDelivery.Core.Enum();
            new DynPartial();
            new ModPartial();
            new ToString();
            new ToLowerCase();
            new PictureSource();
        }
    }


    public class CoreUmbracoConfig : UpdateableConfig
    {
        public CoreUmbracoConfig()
            : base()
        {
            MappingAssemblies.Add(typeof(YuzuStartup).Assembly);
        }
    }

    public class CoreVmBuilderConfig : UpdateableVmBuilderConfig
    {
        public CoreVmBuilderConfig()
            : base()
        {
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataImage>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataLink>();

            AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.Core;");
        }
    }

    public class CoreImportConfig : UpdateableImportConfiguration
    {
        public CoreImportConfig(IVmPropertyFinder vmPropertyFinder)
            : base()
        {
            IgnoreViewmodels.Add<vmBlock_DataImage>();
            IgnoreViewmodels.Add<vmBlock_DataLink>();

            if(vmPropertyFinder != null)
            {
                SpecialistProperties.Add("Images", vmPropertyFinder.GetProperties(typeof(vmBlock_DataImage)));
                SpecialistProperties.Add("Links", vmPropertyFinder.GetProperties(typeof(vmBlock_DataLink)));
            }
        }
    }

}