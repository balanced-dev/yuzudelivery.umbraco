using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Core.Settings;
using ConfigurationException = Umbraco.Cms.Core.Exceptions.ConfigurationException;
using StringExtensions = Umbraco.Extensions.StringExtensions;

namespace YuzuDelivery.Umbraco.Core.Startup
{
    public class DefaultUmbracoVmBuilderConfig : YuzuViewmodelsBuilderConfig
    {
        public DefaultUmbracoVmBuilderConfig(
            IEnumerable<IUpdateableVmBuilderConfig> vmBuilderConfigs,
            IHostEnvironment hostEnvironment,
            IOptionsMonitor<CoreSettings> coreSettings,
            IOptionsMonitor<VmGenerationSettings> vmGenerationSettings)
            : base(vmBuilderConfigs)
        {
            EnableViewmodelsBuilder = vmGenerationSettings.CurrentValue.IsActive;
            GeneratedViewmodelsOutputFolder = GetModelsDirectory(hostEnvironment.ContentRootPath,
                vmGenerationSettings.CurrentValue.Directory, vmGenerationSettings.CurrentValue.AcceptUnsafeDirectory);
        }

        private string GetModelsDirectory(string root, string config, bool acceptUnsafe)
        {
            // making sure it is safe, ie under the website root,
            // unless AcceptUnsafeModelsDirectory and then everything is OK.

            if (!Path.IsPathRooted(root))
            {
                ThrowException($"Root is not rooted \"{root}\".");
            }

            var dir = Path.Combine(root, StringExtensions.TrimStart(config, "~/"));

            // sanitize - GetFullPath will take care of any relative
            // segments in path, eg '../../foo.tmp' - it may throw a SecurityException
            // if the combined path reaches illegal parts of the filesystem
            dir = Path.GetFullPath(dir);
            root = Path.GetFullPath(root);

            if (!dir.StartsWith(root) && !acceptUnsafe)
                ThrowException($"Invalid models directory \"{config}\".");

            return dir;
        }

        private void ThrowException(string message)
        {
            throw new ConfigurationException(message);
        }
    }
}