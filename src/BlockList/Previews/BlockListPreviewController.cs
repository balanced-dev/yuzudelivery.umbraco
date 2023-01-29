using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.ViewModels;
using YuzuDelivery.Umbraco.Core;
using Our.Umbraco.DocTypeGridEditor.Helpers;
using System.Reflection;
using YuzuDelivery.Core.Mapping;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;

namespace YuzuDelivery.Umbraco.BlockList
{
    [PluginController("YuzuDeliveryUmbracoImport")]
    public class BlockListPreviewController : UmbracoAuthorizedApiController
    {
        private readonly IYuzuTemplateEngine _yuzuTemplateEngine;
        private readonly IMapper mapper;
        private readonly IOptions<YuzuConfiguration> config;
        private readonly IContentTypeService contentTypeService;
        private readonly DocTypeGridEditorHelper docTypeGridEditorHelper;

        public BlockListPreviewController(IMapper mapper, IOptions<YuzuConfiguration> config, IYuzuTemplateEngine yuzuTemplateEngine, IContentTypeService contentTypeService, DocTypeGridEditorHelper docTypeGridEditorHelper
            )
        {
            this.mapper = mapper;
            this.config = config;
            this._yuzuTemplateEngine = yuzuTemplateEngine;
            this.contentTypeService = contentTypeService;
            this.docTypeGridEditorHelper = docTypeGridEditorHelper;
        }

        [HttpPost]
        public PreviewReturn GetPartialData([FromForm] PreviewDTO data)
        {
            var output = new PreviewReturn();

            try
            {
                var contentType = contentTypeService.Get(Guid.Parse(data.ContentTypeKey));

                var model = docTypeGridEditorHelper.ConvertValueToContent(Guid.NewGuid().ToString(), contentType.Alias, data.Content);

                var link = GetCmsToVmLink(contentType);

                var suspectBlockTypeName = $"{YuzuConstants.Configuration.BlockPrefix}{model.ContentType.Alias.FirstCharacterToUpper()}";

                if(link.vmType == null)
                    link.vmType = config.Value.ViewModels.Where(x => x.Name == suspectBlockTypeName).FirstOrDefault();

                if (link.vmType == null)
                {
                    output.Error = $"Viewmodel for block type {suspectBlockTypeName} not found. Previews of sublocks not supported";
                }
                else if(link.cmsType == null)
                {
                    output.Error = $"Preview not available, document type for {suspectBlockTypeName} not found";
                }
                else
                {
                    var template = link.vmType.GetTemplateName(includeBaseTypes: false);
                    var mapped = mapper.Map(model, link.cmsType, link.vmType, new Dictionary<string, object>()
                    {
                        { "Model", model },
                        { _BlockList_Constants.IsInPreview, true }
                    });
                    output.Preview = _yuzuTemplateEngine.Render(template, mapped);
                }
            }
            catch (Exception ex)
            {
                output.Error = ex.Message;
            }

            return output;

        }

        public (Type cmsType, Type vmType) GetCmsToVmLink(IContentType contentType)
        {
            var cmsType = config.Value.CMSModels.Where(x => contentType.Alias.FirstCharacterToUpper() == x.Name).FirstOrDefault();

            if (cmsType != null)
            {
                var cmsTypeInterfaces = cmsType.GetInterfaces();

                foreach (var vmType in config.Value.ViewModels)
                {
                    foreach (var attribute in vmType.GetCustomAttributes<YuzuMapAttribute>())
                    {
                        if (attribute.SourceTypeName == cmsType.Name) return (cmsType, vmType);
                        var cmsTypeInterface = cmsTypeInterfaces.Where(x => x.Name == attribute.SourceTypeName).FirstOrDefault();
                        if (cmsTypeInterface != null) return (cmsTypeInterface, vmType);
                    }
                }
            }

            return (cmsType, null);
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
