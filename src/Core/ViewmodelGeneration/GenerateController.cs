using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.WebApi;
using YuzuDelivery.Core.ViewModelBuilder;
using System.IO;
using YuzuDelivery.Core;
using Umbraco.Web.Mvc;
using System.Web.Mvc;
using System.Reflection;

namespace YuzuDelivery.Umbraco.Core
{
    [PluginController("YuzuDeliveryViewModelsBuilder")]
    public class GenerateController : UmbracoAuthorizedApiController
    {
        private readonly BuildViewModelsService buildViewModelsSvc;
        private readonly ReferencesService referencesService;
        private readonly IYuzuViewmodelsBuilderConfig builderConfig;

        public GenerateController()
        {
            this.referencesService = DependencyResolver.Current.GetService<ReferencesService>();
            this.buildViewModelsSvc = DependencyResolver.Current.GetService<BuildViewModelsService>();
            this.builderConfig = DependencyResolver.Current.GetService<IYuzuViewmodelsBuilderConfig>();
        }

        [System.Web.Http.HttpGet]
        public string Build()
        {
            var generatedFolder = builderConfig.GeneratedViewmodelsOutputFolder;

            var genFiles = new Dictionary<string, string>();

            try
            {
                referencesService.FixMultiple(ViewModelType.block);
                buildViewModelsSvc.RunAll(ViewModelType.block, genFiles);

                referencesService.FixMultiple(ViewModelType.page);
                buildViewModelsSvc.RunAll(ViewModelType.page, genFiles);
            }
            catch (Exception ex)
            {
                return GetError(ex);
            }

            if (!Directory.Exists(generatedFolder))
                Directory.CreateDirectory(generatedFolder);

            foreach (var file in genFiles)
            {
                var filename = Path.Combine(generatedFolder, file.Key + ".generated.cs");
                File.WriteAllText(filename, file.Value);
            }

            return string.Empty;
        }

        [System.Web.Http.HttpGet]
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
            sb.Append("\r\n\r\n");
            sb.Append(e.StackTrace);

            return sb.ToString();
        }

        public virtual string GetVersion()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Contains("ViewModelGenerator")).FirstOrDefault();
            if (assembly != null)
                return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            else
                return string.Empty;
        }

    }
}
