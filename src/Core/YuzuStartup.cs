using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
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

            composition.Register<DefaultItemConvertor>();

            composition.Register<IMappingContextFactory, UmbracoMappingContextFactory>(Lifetime.Request);
            composition.Register<IYuzuTypeFactoryRunner, UmbracoTypeFactoryRunner>();

            composition.Register(typeof(YuzuMappingConfig), typeof(DefaultItemMappings));
            composition.Register(typeof(YuzuMappingConfig), typeof(ImageMappings));
            composition.Register(typeof(YuzuMappingConfig), typeof(LinkMappings));
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

            AddDefaultItems(composition);
            SetupHbsHelpers();
        }

        public void SetupHbsHelpers()
        {
            new IfCond();
            new YuzuDelivery.Core.Array();
            new YuzuDelivery.Core.Enum();
            new DynPartial();
            new ModPartial();
            new ToString();
            new PictureSource();
        }

        public void AddDefaultItems(Composition composition)
        {
            composition.Register<IDefaultItem[]>((factory) =>
            {
                var config = factory.GetInstance<IYuzuConfiguration>();
                var mapper = factory.GetInstance<IMapper>();

                var viewmodelAssemblies = config.ViewModelAssemblies;

                var baseItemType = typeof(DefaultItem<,>);
                var items = new List<IDefaultItem>();

                var viewmodelTypes = viewmodelAssemblies.SelectMany(x => x.GetTypes()).Where(x => x.Name.StartsWith(YuzuConstants.Configuration.BlockPrefix));

                foreach (var viewModelType in viewmodelTypes)
                {
                    var umbracoModelTypeName = viewModelType.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "");
                    var umbracoModelType = config.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

                    var alias = umbracoModelTypeName.FirstCharacterToLower();

                    if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                    {
                        var makeme = baseItemType.MakeGenericType(new Type[] { umbracoModelType, viewModelType });
                        var o = Activator.CreateInstance(makeme, new object[] { alias, mapper }) as IDefaultItem;

                        items.Add(o);
                    }
                }

                return items.ToArray();
            }, Lifetime.Singleton);
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

            ManualMappingOptions.Add(new ManualMappingImportOption()
            {
                Id = 1,
                Name = "After Mapping",
                Description = "Change viewmodel after it has been filled using CMS content",
                Template = "TypeAfterMap.cs.pp",
                Filename = "TypeAfterMap",
                ForMapped = true,
                ForType = true,
                Interface = typeof(IYuzuTypeAfterConvertor)
            });
            ManualMappingOptions.Add(new ManualMappingImportOption()
            {
                Id = 2,
                Name = "Replace Mapping",
                Description = "Control viewmodel creation using CMS source",
                Template = "TypeConvertor.cs.pp",
                Filename = "TypeConvertor",
                ForMapped = true,
                ForType = true,
                Interface = typeof(IYuzuTypeConvertor)
            });
            ManualMappingOptions.Add(new ManualMappingImportOption()
            {
                Id = 3,
                Name = "Factory Mapping",
                Description = "Create viewmodel when there is no CMS source",
                Template = "TypeFactory.cs.pp",
                Filename = "TypeFactory",
                ForUnmapped = true,
                ForMapped = true,
                ForType = true,
                Interface = typeof(IYuzuTypeFactory)
            });
            ManualMappingOptions.Add(new ManualMappingImportOption()
            {
                Id = 4,
                Name = "After Mapping",
                Description = "Change viewmodel property after it has been filled using CMS content",
                Template = "MemberAfterMap.cs.pp",
                Filename = "MemberAfterMap",
                ForMember = true,
                ForMapped = true,
                Interface = typeof(IYuzuPropertyAfterResolver)
            });
            ManualMappingOptions.Add(new ManualMappingImportOption()
            {
                Id = 5,
                Name = "Replace Mapping",
                Description = "Take control of data creation for this property",
                Template = "MemberResolver.cs.pp",
                Filename = "MemberResolver",
                ForMember = true,
                ForMapped = true,
                ForUnmapped = true,
                Interface = typeof(IYuzuPropertyReplaceResolver)
            });
            ManualMappingOptions.Add(new ManualMappingImportOption()
            {
                Id = 6,
                Name = "Factory Mapping",
                Description = "Create property value when there is no mapping",
                Template = "MemberFactory.cs.pp",
                Filename = "MemberFactory",
                ForUnmapped = true,
                ForMember = true,
                Interface = typeof(IYuzuTypeFactory)
            });
        }
    }
}