#if NETCOREAPP
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;

namespace Standalone
{
    public class YuzuExampleComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<GenerateController>();
            builder.Services.AddTransient<YuzuContentImportController>();
            builder.Services.AddTransient<Umbraco.Cms.Web.BackOffice.ModelsBuilder.ModelsBuilderDashboardController>();
        }
    }
}
#endif