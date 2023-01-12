using Umbraco.Cms.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.TemplateEngines.Handlebars;

namespace Yuzu.Acceptance
{
    public class YuzuComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddYuzuCore();
            builder.Services.AddYuzuHandlebars();
        }
    }
}
