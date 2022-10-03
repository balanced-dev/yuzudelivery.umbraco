using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Cms.Core.Manifest;

namespace YuzuDelivery.Umbraco.Core
{

    public class YuzuManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest()
            {
                PackageName = "YuzuDeliveryViewModelsBuilder",
                BundleOptions = BundleOptions.None, 
                Scripts = new[]
                {
                    "/App_Plugins/YuzuDeliveryViewModelsBuilder/yuzuDashboardController.js",
                },
                Stylesheets = new[]
                {
                    "/_client/style/backoffice.css"
                }

            });
        }
    }

}
