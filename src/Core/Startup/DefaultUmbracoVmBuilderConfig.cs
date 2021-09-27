using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Core.ViewModelBuilder;
using System.Web;
using System.Configuration;
using System.IO;
using Imp = YuzuDelivery.Umbraco.Import;

#if NETCOREAPP
using Umbraco.Extensions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Exceptions;
#else
using Umbraco.Core;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultUmbracoVmBuilderConfig : YuzuViewmodelsBuilderConfig
    {
        public DefaultUmbracoVmBuilderConfig(IFactory factory)
            : base(factory.GetAllInstances<IUpdateableVmBuilderConfig>())
        {
            var mapPath = factory.GetInstance<Imp.MapPathAbstraction>();
            var settings = factory.GetInstance<SettingsAbstraction>();

            EnableViewmodelsBuilder = settings.ViewmodelActive;

            var acceptUnsafe = settings.ViewmodelAcceptUnsafeDirectory;

            GeneratedViewmodelsOutputFolder = settings.ViewmodelDirectory;

#if NETCOREAPP
            var root = mapPath.Get("");

            if (string.IsNullOrEmpty(GeneratedViewmodelsOutputFolder))
                GeneratedViewmodelsOutputFolder = mapPath.Get("/yuzu/viewModels");
            else
                GeneratedViewmodelsOutputFolder = GetModelsDirectory(root, GeneratedViewmodelsOutputFolder, acceptUnsafe);
#else
            var root = mapPath.Get("~/");

            if (string.IsNullOrEmpty(GeneratedViewmodelsOutputFolder))
                GeneratedViewmodelsOutputFolder = mapPath.Get("/App_Data/ViewModels");
            else
                GeneratedViewmodelsOutputFolder = GetModelsDirectory(root, GeneratedViewmodelsOutputFolder, acceptUnsafe);
#endif
        }



        private string GetModelsDirectory(string root, string config, bool acceptUnsafe)
        {
            // making sure it is safe, ie under the website root,
            // unless AcceptUnsafeModelsDirectory and then everything is OK.

            if (!Path.IsPathRooted(root))
                ThrowException($"Root is not rooted \"{root}\".");

            if (config.StartsWith("~/"))
            {
                var dir = Path.Combine(root, config.TrimStart("~/"));

                // sanitize - GetFullPath will take care of any relative
                // segments in path, eg '../../foo.tmp' - it may throw a SecurityException
                // if the combined path reaches illegal parts of the filesystem
                dir = Path.GetFullPath(dir);
                root = Path.GetFullPath(root);

                if (!dir.StartsWith(root) && !acceptUnsafe)
                    ThrowException($"Invalid models directory \"{config}\".");

                return dir;
            }

            if (acceptUnsafe)
                return Path.GetFullPath(config);

            ThrowException($"Invalid models directory \"{config}\".");
            throw new Exception();
        }

#if NETCOREAPP
        private void ThrowException(string message)
        {
            throw new ConfigurationException(message);
        }
#else
        private void ThrowException(string message)
        {
            throw new ConfigurationErrorsException(message);
        }
#endif


    }
}
