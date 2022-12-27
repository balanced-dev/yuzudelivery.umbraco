using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Core.ViewModelBuilder;
using IO = System.IO;
using YuzuDelivery.Core;
using System.Reflection;
using YuzuDelivery.Core.Mapping;

#if NETCOREAPP
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
#else
using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
using System.Web.Http;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    [PluginController("YuzuDeliveryViewModelsBuilder")]
    public class GenerateController : UmbracoAuthorizedApiController
    {
        private readonly BuildViewModelsService buildViewModelsSvc;
        private readonly IYuzuViewmodelsBuilderConfig builderConfig;
        private readonly IMapper mapper;

        public GenerateController(ReferencesService referencesService, BuildViewModelsService buildViewModelsSvc, IYuzuViewmodelsBuilderConfig builderConfig, IMapper mapper)
        {
            this.buildViewModelsSvc = buildViewModelsSvc;
            this.builderConfig = builderConfig;

            //must be resolved here so that all profiles are created before viewmodel generation
            this.mapper = mapper;
        }

        [HttpGet]
        public string Build()
        {
            var generatedFolder = builderConfig.GeneratedViewmodelsOutputFolder;

            var genFiles = new Dictionary<string, string>();

            try
            {
                buildViewModelsSvc.RunAll(ViewModelType.block, genFiles);
                buildViewModelsSvc.RunAll(ViewModelType.page, genFiles);
            }
            catch (Exception ex)
            {
                return GetError(ex);
            }

            if (!IO.Directory.Exists(generatedFolder))
                IO.Directory.CreateDirectory(generatedFolder);

            foreach (var file in IO.Directory.GetFiles(generatedFolder, "*.generated.cs"))
                IO.File.Delete(file);

            foreach (var file in genFiles)
            {
                var filename = IO.Path.Combine(generatedFolder, file.Key + ".generated.cs");
                IO.File.WriteAllText(filename, file.Value);
            }

            return string.Empty;
        }

        [HttpGet]
        public (bool Enabled, string Dashboard) GetDashboard()
        {
            var sb = new StringBuilder();
            if (builderConfig.EnableViewmodelsBuilder)
            {
                sb.AppendFormat("Version {0}", GetVersion());
                sb.Append("<br />&nbsp;<br />");
                sb.Append("Yuzu ViewmodelsBuilder is enabled, with the following configuration:");

                sb.Append("<ul>");
                sb.Append($"<li>Models namespace is {builderConfig.GeneratedViewmodelsNamespace}.</li>");
                sb.Append("</ul>");

                return (true, sb.ToString());
            }
            else
            {
                sb.AppendFormat("Version {0}", GetVersion());
                sb.Append("<br />&nbsp;<br />");
                sb.Append("Yuzu ViewmodelsBuilder is not enabled.");

                return (false, sb.ToString());
            }
        }

        public string GetError(Exception e)
        {
            var sb = new StringBuilder();
            sb.Append(e.Message);
            sb.Append("\r\n");
            if(e.InnerException != null && e.InnerException.InnerException != null)
            {
                sb.Append(string.Format("Details : {0}", e.InnerException.InnerException.Message));
            }
            else if (e.InnerException != null)
            {
                sb.Append(string.Format("Details : {0}", e.InnerException.Message));
            }
            sb.Append("\r\n\r\n");
            sb.Append(e.StackTrace);

            return sb.ToString();
        }

        public virtual string GetVersion()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Yuzu") && x.GetTypes().Any(y => y == typeof(GenerateController))).FirstOrDefault();
            if (assembly != null)
                return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            else
                return string.Empty;
        }

    }
}
