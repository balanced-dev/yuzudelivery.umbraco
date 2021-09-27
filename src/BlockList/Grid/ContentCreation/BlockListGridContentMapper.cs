using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Cms.Core;
#else
using Umbraco.Core;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    public partial class BlockListGridContentMapper : IContentMapper
    {
        private readonly BlockListGridRowConfigToContent gridRowConfigToContent;
        private readonly BlockListDbModelFactory blockListDbModelFactory;
        private readonly IContentTypeService contentTypeService;
        private readonly GuidFactory guidFactory;

        public BlockListGridContentMapper(BlockListGridRowConfigToContent gridRowConfigToContent, BlockListDbModelFactory blockListDbModelFactory, IContentTypeService contentTypeService, GuidFactory guidFactory)
        {
            this.gridRowConfigToContent = gridRowConfigToContent;
            this.blockListDbModelFactory = blockListDbModelFactory;
            this.contentTypeService = contentTypeService;
            this.guidFactory = guidFactory;
        }

        public bool IsValid(string propertyEditorAlias, ContentPropertyConfig config)
        {
            return propertyEditorAlias == "Umbraco.BlockList" && config.IsGrid;
        }

        public string GetImportValue(VmToContentPropertyLink mapping, ContentPropertyConfig config, string content, string source, IContentMapperFactory factory, IContentImportPropertyService import)
        {
            var contentObj = JObject.Parse(content);

            var output = Rows(contentObj, config, factory, import);

            return JsonConvert.SerializeObject(output, Formatting.Indented);
        }

        public virtual BlockListDbModel Rows(JObject contentObj, ContentPropertyConfig config, IContentMapperFactory contentMapperFactory, IContentImportPropertyService contentImportPropertyService)
        {
            var outputModel = new BlockListDbModel();

            var gridconfigs = gridRowConfigToContent.GetSectionBlocks(config);
            var sectionProperties = gridRowConfigToContent.GetProperties(gridconfigs);
            var sectionNames = sectionProperties.Select(x => new { ContentTypeName = x.Type, Size = x.Size }).Distinct();

            var contentTypes = sectionNames.Select(x => {
                var contentType = contentTypeService.GetByAlias(x.ContentTypeName);
                if (contentType == null) throw new Exception($"Grid row not found {x.ContentTypeName} for import");

                return new GridContentType()
                {
                    Alias = contentType.Alias,
                    Key = contentType.Umb().Key,
                    Size = x.Size
                };
            }).ToList();

            foreach (var row in contentObj["rows"])
            {
                var layout = new BlockListDbModel.LayoutItem();
                var isRowBuilder = row["items"] != null;

                var sections = isRowBuilder
                    ? new List<JToken>() { row } //rowBuilder items
                    : row["columns"].ToList(); //gridBuilder items

                var content = CreateFromItems(sections, sectionProperties, contentTypes, isRowBuilder, config, contentMapperFactory, contentImportPropertyService);

                outputModel.ContentData.Add(JObject.FromObject(content));
                layout.ContentUdi = content["udi"].ToString();

                if (row["config"] != null)
                {

                    var settings = blockListDbModelFactory.GetObject(row["config"], contentImportPropertyService, vmName: config.Grid.RowConfigOfType);
                    outputModel.SettingsData.Add(JObject.FromObject(settings));
                    layout.SettingsUdi = settings["udi"].ToString();
                }

                outputModel.Layout.UmbracoBlockList.Add(layout);
            }

            return outputModel;
        }

        private Dictionary<string, object> CreateFromItems(List<JToken> sections, BlockListGridRowConfigToContent.Property[] allSectionProperties, List<GridContentType> contentTypes, bool isRowBuilder, ContentPropertyConfig config, IContentMapperFactory contentMapperFactory, IContentImportPropertyService contentImportPropertyService)
        {
            var gridSize = sections.Count;

            var key = contentTypes.Where(x => x.Size == gridSize).Select(x => x.Key).FirstOrDefault();

            var output = new Dictionary<string, object>();
            output["contentTypeKey"] = key;
            output["udi"] = Udi.Create("element", guidFactory.CreateNew(key));

            var contentSectionProperties = allSectionProperties.Where(x => x.Size == gridSize && !x.IsSettings).ToList();
            var index = 0;
            foreach(var property in contentSectionProperties)
            {
                var items = sections[index]["items"] as JArray;
                output[property.Alias] = blockListDbModelFactory.Create(items, contentMapperFactory, contentImportPropertyService);
                index++;
            }

            if(!isRowBuilder)
            {
                var configSectionProperties = allSectionProperties.Where(x => x.Size == gridSize && x.IsSettings).ToList();
                index = 0;
                foreach (var property in configSectionProperties)
                {
                    var configObj = sections[index]["config"] as JToken;
                    if (configObj != null)
                    {
                        var items = new JArray(configObj);
                        output[property.Alias] = blockListDbModelFactory.Create(items, contentMapperFactory, contentImportPropertyService, contentVmName: config.Grid.ColumnConfigOfType);
                    }
                    index++;
                }
            }

            return output;
        }

        private class GridContentType
        {
            public string Alias { get; set; }
            public Guid Key { get; set; }
            public int Size { get; set; }
        }

    }
}
