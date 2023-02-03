using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Threading.Tasks;

namespace YuzuDelivery.Umbraco.Forms
{
    public class ViewContextFactory
    {
        public ViewContext Create(HttpContext httpContext)
        {
            var mmp = httpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
            var viewData = new ViewDataDictionary(mmp, new ModelStateDictionary());

            var factory = httpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = factory.GetTempData(httpContext);

            return new ViewContext(
                new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor())),
                NullView.Instance,
                viewData,
                tempData,
                new StringWriter(),
                new HtmlHelperOptions());
        }

        private class NullView : IView
        {
            public static readonly NullView Instance = new();

            public string Path => string.Empty;

            public Task RenderAsync(ViewContext context) => Task.CompletedTask;
        }
    }
}
