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

#if NETCOREAPP
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;
#else
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
using System.Web.Http;
#endif

namespace YuzuDelivery.Umbraco.BlockList
{
    [PluginController("YuzuDeliveryUmbracoImport")]
    public class BlockListPreviewController : UmbracoAuthorizedApiController
    {
        private readonly IYuzuDefinitionTemplates yuzuDefinitionTemplates;
        private readonly IMapper mapper;
        private readonly IYuzuConfiguration config;
        private readonly IContentTypeService contentTypeService;
        #if NETCOREAPP 
        private readonly DocTypeGridEditorHelper docTypeGridEditorHelper; 
        #endif

        public BlockListPreviewController(IMapper mapper, IYuzuConfiguration config, IYuzuDefinitionTemplates yuzuDefinitionTemplates, IContentTypeService contentTypeService
#if NETCOREAPP
            , DocTypeGridEditorHelper docTypeGridEditorHelper
#endif
            )
        {
            this.mapper = mapper;
            this.config = config;
            this.yuzuDefinitionTemplates = yuzuDefinitionTemplates;
            this.contentTypeService = contentTypeService;
#if NETCOREAPP
            this.docTypeGridEditorHelper = docTypeGridEditorHelper;
#endif
        }

        [HttpPost]
#if NETCOREAPP
        public PreviewReturn GetPartialData([FromForm] PreviewDTO data)
#else
        public PreviewReturn GetPartialData([FromBody] PreviewDTO data)
#endif
        {
            var output = new PreviewReturn();

            try
            {
                var contentType = contentTypeService.Get(Guid.Parse(data.ContentTypeKey));

#if NETCOREAPP
                var model = docTypeGridEditorHelper.ConvertValueToContent(Guid.NewGuid().ToString(), contentType.Alias, data.Content);
#else
                var model = DocTypeGridEditorHelper.ConvertValueToContent(Guid.NewGuid().ToString(), contentType.Alias, data.Content);
#endif

                var link = GetCmsToVmLink(contentType);

                var suspectBlockTypeName = $"{YuzuConstants.Configuration.BlockPrefix}{model.ContentType.Alias.FirstCharacterToUpper()}";

                if(link.vmType == null)
                    link.vmType = config.ViewModels.Where(x => x.Name == suspectBlockTypeName).FirstOrDefault();

                if (link.vmType == null)
                {
                    output.Error = $"Viewmodel for block type {suspectBlockTypeName} not found. Previews of sublocks not supported";
                }
                else
                {
                    output.Preview = yuzuDefinitionTemplates.Render(new RenderSettings()
                    {
                        Data = () => { return mapper.Map(model, link.cmsType, link.vmType, new Dictionary<string, object>() { { "Model", model }, { _BlockList_Constants.IsInPreview, true } }); },
                        Template = yuzuDefinitionTemplates.GetSuspectTemplateNameFromVm(link.vmType)
                    });
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
            var cmsType = config.CMSModels.Where(x => contentType.Alias.FirstCharacterToUpper() == x.Name).FirstOrDefault();
            var cmsTypeInterfaces = cmsType.GetInterfaces();

            foreach (var vmType in config.ViewModels)
            {
                foreach (var attribute in vmType.GetCustomAttributes<YuzuMapAttribute>())
                {
                    if (attribute.SourceTypeName == cmsType.Name) return (cmsType, vmType);
                    var cmsTypeInterface = cmsTypeInterfaces.Where(x => x.Name == attribute.SourceTypeName).FirstOrDefault();
                    if (cmsTypeInterface != null) return (cmsTypeInterface, vmType);
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