using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AutoMapper;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;

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

            composition.Register<DefaultUmbracoMappingFactory>();
            composition.RegisterAuto<Profile>();

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
                        var o = Activator.CreateInstance(makeme, new object[] { alias }) as IDefaultItem;

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

}