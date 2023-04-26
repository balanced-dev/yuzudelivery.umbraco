using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using YuzuDelivery.Core.Settings;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace YuzuDelivery.Umbraco.Core.Middleware;

public class YuzuLoadedSchemaMiddleware : IMiddleware
{
    private readonly IOptions<CoreSettings> _coreSettings;

    public YuzuLoadedSchemaMiddleware(IOptions<CoreSettings> coreSettings)
    {
        _coreSettings = coreSettings;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // ReSharper disable once StringLiteralTypo
        if (!ShouldHandle(context))
        {
            await next(context);
            return;
        }

        var fileProvider = _coreSettings.Value.SchemaFileProvider;

        var files = fileProvider.GetDirectoryContents(string.Empty)
            .Cast<IFileInfo>();

        await context.Response.WriteAsJsonAsync(
            files, 
            files.GetType(),
            new JsonSerializerOptions(),
            MediaTypeNames.Application.Json);
    }

    private bool ShouldHandle(HttpContext context)
    {
        if (context.Request.Query.ContainsKey("showJsonLoadedSchema"))
        {
            return true;
        }

        return false;
    }
}


