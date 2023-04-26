using Microsoft.AspNetCore.Builder;
using YuzuDelivery.Umbraco.Core.Middleware;

namespace YuzuDelivery.Umbraco.Core;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseYuzuJsonMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<YuzuRenderJsonMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseYuzuSchemaMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<YuzuLoadedSchemaMiddleware>();
        return app;
    }
}
