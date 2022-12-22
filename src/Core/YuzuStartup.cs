﻿using System;
using System.Collections.Generic;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Mapping.Mappers;
using YuzuDelivery.Umbraco.Core.Mapping;
using YuzuDelivery.Umbraco.Core.Mapping.Mappers;
using YuzuDelivery.Umbraco.Core.Settings;

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

            builder.Services.AddOptions<VmGenerationSettings>()
                .Bind(builder.Config.GetSection("Yuzu:VmGeneration"))
                .ValidateDataAnnotations();

            builder.ManifestFilters().Append<YuzuManifestFilter>();

            builder.Services.AddTransient<ISchemaMetaService, SchemaMetaService>();
            builder.Services.AddTransient<ISchemaMetaPropertyService, SchemaMetaPropertyService>();

            //Viewmodel Builder
            builder.Services.AddSingleton<BuildViewModelsService>();
            builder.Services.AddSingleton<ReferencesService>();
            builder.Services.AddSingleton<GenerateViewmodelService>();

            //MUST be transient lifetime
            builder.Services.AddTransient(typeof(IUpdateableConfig), typeof(CoreUmbracoConfig));
            builder.Services.AddTransient(typeof(IUpdateableVmBuilderConfig), typeof(CoreVmBuilderConfig));
            builder.Services.AddTransient(typeof(IUpdateableImportConfiguration), typeof(CoreImportConfig));

            builder.Services.AddSingleton<DefaultYuzuMapperFactory>();

            //ToDo: not sure what this is Umbraco 9
            //composition.RegisterAuto<AutoMapper.Profile>();

            builder.Services.AddTransient<LinkIPublishedContentConvertor>();
            builder.Services.AddSingleton<LinkConvertor>();
            builder.Services.AddTransient<ImageConvertor>();
            builder.Services.AddTransient<MediWithCropsConvertor>();
            builder.Services.AddTransient(typeof(SubBlocksObjectResolver<,>));

            builder.Services.AddTransient<DefaultPublishedElementCollectionConvertor>();
            builder.Services.AddTransient<DefaultPublishedElementCollectionToSingleConvertor>();

            builder.Services.AddTransient<IMappingContextFactory<UmbracoMappingContext>, UmbracoMappingContextFactory>();
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

            AddDefaultPublishedElements(builder);
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
                    var umbracoModelTypeName = viewModelType.GetModelName();
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

            AddNamespacesAtGeneration.Add("YuzuDelivery.Umbraco.Core");
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
