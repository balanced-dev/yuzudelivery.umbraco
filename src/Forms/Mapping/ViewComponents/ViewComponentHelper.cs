using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Text.Encodings.Web;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;

namespace YuzuDelivery.Umbraco.Forms
{
    public class ViewComponentHelper
    {
        public async Task<string> RenderToString(string viewComponent, object args, ViewContext context, HttpContext httpContext)
        {
            var sp = httpContext.RequestServices;

            var helper = new DefaultViewComponentHelper(
                sp.GetRequiredService<IViewComponentDescriptorCollectionProvider>(),
                HtmlEncoder.Default,
                sp.GetRequiredService<IViewComponentSelector>(),
                sp.GetRequiredService<IViewComponentInvokerFactory>(),
                sp.GetRequiredService<IViewBufferScope>());

            // Take a copy so that view components which mutate ViewData do not leak
            var viewContextCopy = new ViewContext(context, context.View, new ViewDataDictionary(context.ViewData), context.Writer);

            using (var writer = new StringWriter())
            {
                helper.Contextualize(viewContextCopy);
                var result = await helper.InvokeAsync(viewComponent, args);
                result.WriteTo(writer, HtmlEncoder.Default);
                await writer.FlushAsync();
                return writer.ToString();
            }
        }
    }
}