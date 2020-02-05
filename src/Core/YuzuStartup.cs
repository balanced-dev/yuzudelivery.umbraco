using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IHandlebarsProvider, HandlebarsProvider>(Lifetime.Singleton);
            composition.Register<IYuzuDefinitionTemplates, YuzuDefinitionTemplates>(Lifetime.Singleton);
            composition.Register<IYuzuDefinitionTemplateSetup, YuzuDefinitionTemplateSetup>(Lifetime.Singleton);
            composition.Register<IAuthoriseApi, AuthenticateUmbraco>();
            composition.Register<ISchemaMetaService, SchemaMetaService>();
            composition.Register<ISchemaMetaPropertyService, SchemaMetaPropertyService>();

            new IfCond();
            new YuzuDelivery.Core.Array();
            new YuzuDelivery.Core.Enum();
            new DynPartial();
            new ModPartial();
            new ToString();
            new PictureSource();

            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_DataImage>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_DataLink>();

            Yuzu.Configuration.AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.Core;");

            AddDefaultItems(composition);
        }

        public void AddDefaultItems(Composition composition)
        {
            var viewmodelAssemblies = Yuzu.Configuration.ViewModelAssemblies;

            var baseItemType = typeof(DefaultItem<,>);
            var items = new List<IDefaultItem>();

            var viewmodelTypes = viewmodelAssemblies.SelectMany(x => x.GetTypes()).Where(x => x.Name.StartsWith(Yuzu.Configuration.BlockPrefix));

            foreach (var viewModelType in viewmodelTypes)
            {
                var umbracoModelTypeName = viewModelType.Name.Replace(Yuzu.Configuration.BlockPrefix, "");
                var umbracoModelType = Yuzu.Configuration.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

                var alias = umbracoModelTypeName.FirstCharacterToLower();

                if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                {
                    var makeme = baseItemType.MakeGenericType(new Type[] { umbracoModelType, viewModelType });
                    var o = Activator.CreateInstance(makeme, new object[] { alias }) as IDefaultItem;

                    items.Add(o);
                }
            }

            composition.Register(typeof(IDefaultItem[]), items.ToArray());
        }
    }
}