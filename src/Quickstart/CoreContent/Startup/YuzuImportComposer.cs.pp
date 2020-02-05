using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Grid;
using YuzuDelivery.Umbraco.Forms;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.ViewModels;
using YuzuDelivery.UmbracoModels;

namespace $rootnamespace$
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeBefore(typeof(YuzuStartup))]
    [ComposeBefore(typeof(YuzuGridStartup))]
    [ComposeBefore(typeof(YuzuFormsStartup))]
    public class YuzuImportsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var Server = HttpContext.Current.Server;
            var localAssembly = Assembly.GetAssembly(typeof(YuzuImportsComposer));
            var gridAssembly = Assembly.GetAssembly(typeof(YuzuGridStartup));

            var config = new YuzuDeliveryImportConfiguration()
            {
                IsActive = ConfigurationManager.AppSettings["YuzuImportActive"] == "true",
                DocumentTypeAssemblies = new Assembly[] { localAssembly },
                ViewModelQualifiedTypeName = "YuzuDelivery.ViewModels.{0}, $rootnamespace$",
                UmbracoModelsQualifiedTypeName = "YuzuDelivery.UmbracoModels.{0}, $rootnamespace$",
                DataTypeFolder = new DataTypeFolder()
                {
                    Name = "$rootnamespace$"
                },
                DataLocations = new List<IDataLocation>()
                {
                    new DataLocation()
                    {
                        Name = "Main",
                        Path = Server.MapPath(ConfigurationManager.AppSettings["YuzuData"])
                    }
                },
                ImageLocations = new List<IDataLocation>()
                {
                    new DataLocation()
                    {
                        Name = "Main",
                        Path = Server.MapPath(ConfigurationManager.AppSettings["YuzuImages"])
                    }
                },
                CustomConfigFileLocation = Server.MapPath(ConfigurationManager.AppSettings["YuzuImportCustomConfig"])
            };

            YuzuDeliveryImport.Initialize(config);
        }
    }
}
