using System.IO;
#if NETCOREAPP
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
#else
using System.Web;
using System.Configuration;
#endif
namespace YuzuDelivery.Umbraco.Core
{
#if NETCOREAPP
    public class SettingsAbstraction
    {
        private readonly IOptions<CoreSettings> coreSettings;
        private readonly IOptions<VmGenerationSettings> generationSettings;

        public SettingsAbstraction(IOptions<CoreSettings> coreSettings, IOptions<VmGenerationSettings> generationSettings)
        {
            this.coreSettings = coreSettings;
            this.generationSettings = generationSettings;
        }

        public string Pages => coreSettings.Value.Pages;

        public string Partials => coreSettings.Value.Partials;

        public string SchemaMeta => coreSettings.Value.SchemaMeta;

        public bool ViewmodelActive => generationSettings.Value.IsActive;

        public bool ViewmodelAcceptUnsafeDirectory => generationSettings.Value.AcceptUnsafeDirectory;

        public string ViewmodelDirectory => generationSettings.Value.Directory;

    }
#else
    public class SettingsAbstraction
    {
        public string Pages => ConfigurationManager.AppSettings["YuzuPages"];

        public string Partials => ConfigurationManager.AppSettings["YuzuPartials"];

        public string SchemaMeta => ConfigurationManager.AppSettings["YuzuSchemaMeta"];

        public bool ViewmodelActive => ConfigurationManager.AppSettings["YuzuViewmodelBuilderActive"] == "true";

        public bool ViewmodelAcceptUnsafeDirectory => ConfigurationManager.AppSettings["YuzuViewmodelBuilderAcceptUnsafeDirectory"] == "true";

        public string ViewmodelDirectory => ConfigurationManager.AppSettings["YuzuViewmodelBuilderDirectory"];
    }
#endif
}
