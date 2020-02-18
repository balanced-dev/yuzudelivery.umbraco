using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core.ViewModelBuilder;
using System.Web;
using System.Configuration;
using System.IO;

namespace YuzuDelivery.Umbraco.Core
{
    public class DefaultUmbracoVmBuilderConfig : YuzuViewmodelsBuilderConfig
    {
        public DefaultUmbracoVmBuilderConfig(IFactory factory)
            : base(factory.GetAllInstances<IUpdateableVmBuilderConfig>())
        {
            var Server = HttpContext.Current.Server;

            EnableViewmodelsBuilder = ConfigurationManager.AppSettings["YuzuViewmodelBuilderActive"] == "true";

            var acceptUnsafe = ConfigurationManager.AppSettings["YuzuViewmodelBuilderAcceptUnsafeDirectory"] == "true";
            var root = Server.MapPath("~/");

            GeneratedViewmodelsOutputFolder = ConfigurationManager.AppSettings["YuzuViewmodelBuilderDirectory"];
            if (string.IsNullOrEmpty(GeneratedViewmodelsOutputFolder))
                GeneratedViewmodelsOutputFolder = Server.MapPath("/App_Data/ViewModels");
            else
                GeneratedViewmodelsOutputFolder = GetModelsDirectory(root, GeneratedViewmodelsOutputFolder, acceptUnsafe);

            AddNamespacesAtGeneration.Add("using YuzuDelivery.UmbracoModels;");
        }

        private string GetModelsDirectory(string root, string config, bool acceptUnsafe)
        {
            // making sure it is safe, ie under the website root,
            // unless AcceptUnsafeModelsDirectory and then everything is OK.

            if (!Path.IsPathRooted(root))
                throw new ConfigurationErrorsException($"Root is not rooted \"{root}\".");

            if (config.StartsWith("~/"))
            {
                var dir = Path.Combine(root, config.TrimStart("~/"));

                // sanitize - GetFullPath will take care of any relative
                // segments in path, eg '../../foo.tmp' - it may throw a SecurityException
                // if the combined path reaches illegal parts of the filesystem
                dir = Path.GetFullPath(dir);
                root = Path.GetFullPath(root);

                if (!dir.StartsWith(root) && !acceptUnsafe)
                    throw new ConfigurationErrorsException($"Invalid models directory \"{config}\".");

                return dir;
            }

            if (acceptUnsafe)
                return Path.GetFullPath(config);

            throw new ConfigurationErrorsException($"Invalid models directory \"{config}\".");
        }

    }
}
