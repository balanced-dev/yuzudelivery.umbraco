using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Composing;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Dtge;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Grid 
{

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuGridStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var baseGridType = typeof(DefaultGridItem<,>);
            var gridItems = new List<IGridItem>();
            var viewmodelTypes = Yuzu.Configuration.ViewModels.Where(x => x.Name.StartsWith(Yuzu.Configuration.BlockPrefix));

            foreach (var viewModelType in viewmodelTypes)
            {
                var umbracoModelTypeName = viewModelType.Name.Replace(Yuzu.Configuration.BlockPrefix, "");
                var alias = umbracoModelTypeName.FirstCharacterToLower();
                var umbracoModelType = Yuzu.Configuration.CMSModels.Where(x => x.Name == umbracoModelTypeName).FirstOrDefault();

                if (umbracoModelType != null && umbracoModelType.BaseType == typeof(PublishedElementModel))
                {
                    var makeme = baseGridType.MakeGenericType(new Type[] { umbracoModelType, viewModelType });
                    var o = Activator.CreateInstance(makeme, new object[] { alias }) as IGridItem;

                    gridItems.Add(o);
                }
            }

            var types = Yuzu.Configuration.ViewModelAssemblies.SelectMany(x => x.GetTypes()).Where(x => x.GetInterfaces().Any(y => y == typeof(IAutomaticGridConfig)));
            foreach (var f in types)
            {
                composition.Register(typeof(IAutomaticGridConfig), f);
            }

            types = Yuzu.Configuration.ViewModelAssemblies.SelectMany(x => x.GetTypes()).Where(x => x != baseGridType && x.GetInterfaces().Any(y => y == typeof(IGridItem)));
            foreach (var f in types)
            {
                composition.Register(typeof(IGridItem), f);
            }

            composition.Register(typeof(IGridItem[]), gridItems.ToArray());
            composition.Register<IGridService, GridService>(Lifetime.Singleton);

            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_DataGridRows>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_DataGridRowsColumns>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRowsRow>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRowsColumnsRow>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmSub_DataGridRowsColumnsColumn>();

            Yuzu.Configuration.AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.Grid;");


            GridContext.Current.Converters.Add(new DtgeGridConverter());
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