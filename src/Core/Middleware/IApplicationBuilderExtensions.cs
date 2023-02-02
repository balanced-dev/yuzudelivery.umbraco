using Microsoft.AspNetCore.Builder;
using YuzuDelivery.Umbraco.Core.Middleware;

// ReSharper disable once CheckNamespace
namespace YuzuDelivery.Umbraco.Core;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseYuzuJsonMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<YuzuRenderJsonMiddleware>();
        return app;
    }
}
