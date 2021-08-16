using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Composing;
using YuzuDelivery.Umbraco.Import;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models.Blocks;
using YuzuDelivery.Core;
using Umbraco.Web.Mvc;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using YuzuDelivery.ViewModels;

namespace YuzuDelivery.Umbraco.BlockList
{
    [PluginController("YuzuDeliveryUmbracoImport")]
    public class BlockListPreviewController : UmbracoAuthorizedApiController
    {
        private readonly IYuzuDefinitionTemplates yuzuDefinitionTemplates;
        private readonly IMapper mapper;
        private readonly IYuzuConfiguration config;

        public BlockListPreviewController(IMapper mapper, IYuzuConfiguration config, IYuzuDefinitionTemplates yuzuDefinitionTemplates)
        {
            this.mapper = mapper;
            this.config = config;
            this.yuzuDefinitionTemplates = yuzuDefinitionTemplates;
        }

        [HttpPost]
        public PreviewReturn GetPartialData([FromBody] PreviewDTO data)
        {
            var output = new PreviewReturn();

            try
            {
                var contentType = Current.Services.ContentTypeService.Get(Guid.Parse(data.ContentTypeKey));

                var model = DocTypeGridEditorHelper.ConvertValueToContent(Guid.NewGuid().ToString(), contentType.Alias, data.Content);

                var modelType = config.CMSModels.Where(x => contentType.Alias.FirstCharacterToUpper() == x.Name).FirstOrDefault();

                var suspectBlockTypeName = $"{YuzuConstants.Configuration.BlockPrefix}{model.ContentType.Alias.FirstCharacterToUpper()}";

                var vmType = config.ViewModels.Where(x => x.Name == suspectBlockTypeName).FirstOrDefault();

                if (vmType == null)
                {
                    output.Error = $"Viewmodel for block type {suspectBlockTypeName} not found. Previews of sublocks not supported";
                }
                else
                {
                    output.Preview = yuzuDefinitionTemplates.Render(new RenderSettings()
                    {
                        Data = () => { return mapper.Map(model, modelType, vmType, new Dictionary<string, object>() { { "Model", model } }); },
                        Template = yuzuDefinitionTemplates.GetSuspectTemplateName(modelType)
                    });
                }
            }
            catch (Exception ex)
            {
                output.Error = ex.Message;
            }

            return output;

        }
    }

    public class PreviewReturn
    {
        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class PreviewDTO
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("contentTypeKey")]
        public string ContentTypeKey { get; set; }
    }
}