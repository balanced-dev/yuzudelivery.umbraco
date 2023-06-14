using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.ViewModels;
using YuzuDelivery.Umbraco.Core;
using System.Reflection;
using YuzuDelivery.Core.Mapping;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;
using NPoco.fastJSON;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace YuzuDelivery.Umbraco.BlockList
{
    [PluginController("YuzuDeliveryUmbracoImport")]
    public class BlockListPreviewController : UmbracoAuthorizedApiController
    {
        private readonly IYuzuTemplateEngine _yuzuTemplateEngine;
        private readonly IMapper _mapper;
        private readonly IOptions<YuzuConfiguration> _config;
        private readonly BlockEditorConverter _blockEditorConverter;
        private readonly IYuzuMappingIndex _mappingIndex;
        private readonly IContentTypeService _contentTypeService;
        private readonly DocTypeGridEditorHelper _docTypeGridEditorHelper;
        private readonly IEnumerable<IBlockPreviewOutputOverride> _previewOverrides;

        public BlockListPreviewController(IMapper mapper, IOptions<YuzuConfiguration> config, IYuzuTemplateEngine yuzuTemplateEngine, IContentTypeService contentTypeService,
            BlockEditorConverter blockEditorConverter, IYuzuMappingIndex mappingIndex, 
            DocTypeGridEditorHelper docTypeGridEditorHelper, IEnumerable<IBlockPreviewOutputOverride> previewOverrides)
        {
            _mapper = mapper;
            _config = config;
            _yuzuTemplateEngine = yuzuTemplateEngine;
            _contentTypeService = contentTypeService;
            _blockEditorConverter = blockEditorConverter;
            _mappingIndex = mappingIndex;
            _docTypeGridEditorHelper = docTypeGridEditorHelper;
            _previewOverrides = previewOverrides;
        }

        [HttpPost]
        public PreviewReturn GetPartialData([FromForm] PreviewDTO data)
        {
            var output = new PreviewReturn();

            try
            {
                var contentType = _contentTypeService.Get(Guid.Parse(data.ContentTypeKey));

                var blockData = JsonConvert.DeserializeObject<BlockItemData>(data.Content);

                var model = _docTypeGridEditorHelper.ConvertValueToContent(Guid.NewGuid().ToString(), contentType.Alias, data.Content);

                var cmsType = model.GetType();
                var vmType = _mappingIndex.GetViewModelType(cmsType);

                var suspectBlockTypeName = $"{YuzuConstants.Configuration.BlockPrefix}{model.ContentType.Alias.FirstCharacterToUpper()}";

                if(vmType == null)
                    vmType = _config.Value.ViewModels.Where(x => x.Name == suspectBlockTypeName).FirstOrDefault();

                if (vmType == null)
                {
                    output.Error = $"Viewmodel for block type {suspectBlockTypeName} not found. Previews of sublocks not supported";
                }
                else if(cmsType == null)
                {
                    output.Error = $"Preview not available, document type for {suspectBlockTypeName} not found";
                }
                else
                {
                    var template = vmType.GetTemplateName(includeBaseTypes: false);
                    var mapped = _mapper.Map(model, cmsType, vmType, new Dictionary<string, object>()
                    {
                        { "Model", model },
                        { _BlockList_Constants.IsInPreview, true }
                    });
                    output.Preview = _yuzuTemplateEngine.Render(template, mapped);
                }

                // Work out the settings
                if (data.ColSettings != null)
                {
                    var rowSettings = _blockEditorConverter.ConvertToElement(JsonConvert.DeserializeObject<BlockItemData>(data.RowSettings), PropertyCacheLevel.None, true);
                    var colSettings = _blockEditorConverter.ConvertToElement(JsonConvert.DeserializeObject<BlockItemData>(data.ColSettings), PropertyCacheLevel.None, true);
                    var itemSettings = _blockEditorConverter.ConvertToElement(JsonConvert.DeserializeObject<BlockItemData>(data.ItemSettings), PropertyCacheLevel.None, true);

                    foreach(var item in _previewOverrides)
                    {
                        item.Update(output, data.NodeId, model, itemSettings, colSettings, rowSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                output.Error = ex.Message;
            }

            return output;

        }
    }

    public interface IBlockPreviewOutputOverride
    {
        public void Update(PreviewReturn output, int nodeId, IPublishedElement content, IPublishedElement itemSettings, IPublishedElement colSettings, IPublishedElement rowSettings);
    }

    public class PreviewReturn
    {
        public PreviewReturn()
        {
            Classes = new List<string>();
        }

        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("classes")]
        public List<string> Classes { get; set; }
    }

    public class PreviewDTO
    {
        [JsonProperty("nodeId")]
        public int NodeId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("contentTypeKey")]
        public string ContentTypeKey { get; set; }

        [JsonProperty("itemSettings")]
        public string ItemSettings { get; set; }

        [JsonProperty("colSettings")]
        public string ColSettings { get; set; }

        [JsonProperty("rowSettings")]
        public string RowSettings { get; set; }
    }
}
