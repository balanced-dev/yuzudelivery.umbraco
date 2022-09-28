using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace YuzuDelivery.Umbraco.Core
{
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
}
