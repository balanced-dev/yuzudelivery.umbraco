using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Cms.Core.Manifest;

namespace YuzuDelivery.Umbraco.BlockList
{

    public class DocTypeGridEditorManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest()
            {
                Scripts = new[]
                {
                    "/App_Plugins/YuzuBlockList/GridContentColumnsSettingsController.js",
                    "/App_Plugins/YuzuBlockList/GridContentItem.js",
                    "/App_Plugins/YuzuBlockList/GridContentSection.js"
                },
                Stylesheets = new[]
                {
                    "/App_Plugins/YuzuBlockList/GridSection.css"
                }

            });
        }
    }

}
