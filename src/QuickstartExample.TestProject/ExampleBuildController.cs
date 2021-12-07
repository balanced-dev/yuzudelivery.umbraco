using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YuzuDelivery.Core;
using System.Reflection;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core.ViewModelBuilder;

#if NETCOREAPP
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Umb = Umbraco.Cms.Core.Services;
using Mod = Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.BackOffice.ModelsBuilder;
#else
using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
using System.Web.Http;
using Umb = Umbraco.Core.Services;
using Mod = Umbraco.Core.Models;
using Umbraco.ModelsBuilder.Embedded.BackOffice;
#endif

namespace YuzuDelivery.Umbraco.TestProject
{
    /* /umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/GenerateViewModels */
    /* /umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/GenerateDocumentTypes */
    /* /umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/AddTemplates */
    /* /umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/CreateContent */

    [PluginController("YuzuDeliveryExamples")]
    public class ExampleBuildController : UmbracoAuthorizedApiController
    {

        private readonly GenerateController generate;
        private readonly SchemaChangeController schemaChange;
        private readonly ModelsBuilderDashboardController modelsBuilder;
        private readonly YuzuContentImportController contentImport;

        private readonly Umb.IFileService fileService;
        private readonly Umb.IContentTypeService umbContentTypeService;
        private readonly Umb.IContentService contentService;
        private readonly IContentTypeService contentTypeService;

        public ExampleBuildController(GenerateController generate, SchemaChangeController schemaChange, Umb.IFileService fileService, IContentTypeService contentTypeService, Umb.IContentTypeService umbContentTypeService, Umb.IContentService contentService, YuzuContentImportController contentImport, ModelsBuilderDashboardController modelsBuilder)
        {
            this.generate = generate;
            this.schemaChange = schemaChange;
            this.modelsBuilder = modelsBuilder;
            this.contentImport = contentImport;
            this.fileService = fileService;
            this.contentTypeService = contentTypeService;
            this.umbContentTypeService = umbContentTypeService;
            this.contentService = contentService;
        }

        [HttpGet]
        public void CreateGroups()
        {

            schemaChange.ApplyMultipleGroups(new List<GroupReturn>() {
                new GroupReturn()
                {
                    ChildViewmodel = "vmBlock_Grouped",
                    PropertyName = "Grouped",
                    Group = "Grouped",
                    ParentViewmodel = "vmPage_Transmuted",
                    ToAdd = true
                }
            });

        }

        [HttpGet]
        public void CreateGlobal()
        {
            var globalArea = contentTypeService.GetByAlias("globalArea").Umb();
            if(globalArea == null)
            {
                globalArea = contentTypeService.Create("Global Area", "globalArea", false).Umb();
            }

            var globalAreaContent = contentService.Create("Global", -1, "globalArea");
            var result = contentService.SaveAndPublish(globalAreaContent);


            var globalSettings = new GlobalStoreContentAs()
            {
                ParentContentId = globalAreaContent.Id.ToString(),
                PrimaryPropertyName = "string",
                Type = StoreContentAsType.Global
            };

            schemaChange.ApplySettings(new ApplySettingsReturn()
            {
                ViewmodelName = "vmBlock_Global",
                StoreContentAs = Newtonsoft.Json.JsonConvert.SerializeObject(globalSettings)
            });
        }

        [HttpGet]
        public string GenerateViewModels()
        {

            return generate.Build();
        
        }

        [HttpGet]
        public bool GenerateDocumentTypes()
        {
            schemaChange.ChangeDocumentTypes();

            try
            {
                modelsBuilder.BuildModels();
            }
            catch(Exception ex)
            {
                if(!(ex is NullReferenceException))
                    throw new Exception("Error creating models", ex);
            }
            return true;
        }

        [HttpGet]
        public bool AddTemplates()
        {
            var home = contentTypeService.Create("Home", "home", false).Umb();
            home.AllowedAsRoot = true;
            AddTemplateToContentType(home);

            var rowPage = GetContentAddTemplate("rowPage");
            var gridPage = GetContentAddTemplate("gridPage");
            var basicPage = GetContentAddTemplate("basicPage");
            var formPage = GetContentAddTemplate("formPage");
            var transmuted = GetContentAddTemplate("transmuted");

            contentTypeService.AddPermissions(home.Id, new List<IContentType>() { rowPage.Yuzu(), gridPage.Yuzu(), basicPage.Yuzu(), formPage.Yuzu(), transmuted.Yuzu() });

            return true;
        }

        [HttpGet]
        public bool CreateContent()
        {

            var homeContent = contentService.Create("Home", -1, "home");
            contentService.SaveAndPublish(homeContent);

            CreateContent("gridPage", homeContent);
            CreateContent("rowPage", homeContent);
            CreateContent("basicPage", homeContent);
            CreateContent("formPage", homeContent, false);
            CreateContent("transmuted", homeContent);

            return true;

        }

        private void CreateContent(string name, Mod.IContent parent, bool import = true)
        {
            var contenttype = contentTypeService.GetByAlias(name);
            var content = new Mod.Content(name.FirstCharacterToUpper(), parent, contenttype.Umb());
            content.TemplateId = GetTemplateId(name);
            contentService.SaveAndPublish(content);

            if(import)
                ImportContent(name, content);
        }

        private void ImportContent(string name, Mod.IContent content)
        {
            contentImport.Import(new ImportContentFromFileVm()
            {
                Content = content.Id,
                DocumentTypeName = name,
                File = new DataFileLocationsVm()
                {
                    Location = "Main",
                    Filename = $"{name}.json"
                },
                Viewmodel = $"vmPage_{name.FirstCharacterToUpper()}"
            });
        }

        private Mod.IContentType GetContentAddTemplate(string alias)
        {
            var contentType = umbContentTypeService.Get(alias);
            AddTemplateToContentType(contentType);
            return contentType;
        }

        private void AddTemplateToContentType(Mod.IContentType contentType)
        {
            var result = fileService.CreateTemplateForContentType(contentType.Alias, contentType.Name);
            contentType.AllowedTemplates = new List<Mod.ITemplate>() { result.Result.Entity };
            umbContentTypeService.Save(contentType);
            contentType.SetDefaultTemplate(result.Result.Entity);
        }

        private int GetTemplateId(string alias)
        {
            var template = fileService.GetTemplate(alias);
            return template != null ? template.Id : -1;
        }
    }
}
