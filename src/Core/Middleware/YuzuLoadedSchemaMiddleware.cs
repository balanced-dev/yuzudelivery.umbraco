using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

        var report = new FileProviderMiddleWareReport(_coreSettings.Value.SchemaFileProvider);

        await context.Response.WriteAsJsonAsync(
            report, 
            report.GetType(),
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

    public class FileProviderMiddleWareReport
    {
        public FileProviderMiddleWareReport(IFileProvider rootFileProvider)
        {
            FileProviders = new List<string>();
            Files = new List<IFileInfo>();

            if (rootFileProvider is CompositeFileProvider)
            {
                var compositeFileProvider = (CompositeFileProvider)rootFileProvider;
                foreach (var f in compositeFileProvider.FileProviders)
                {
                    AddFileProviderToReport(f, this);
                }
            }
            else
            {
                AddFileProviderToReport(rootFileProvider, this);
            }

            try
            {
                Files = rootFileProvider.GetDirectoryContents(string.Empty)
                .Cast<IFileInfo>();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

        }

        public List<string> FileProviders { get; set; }
        public IEnumerable<IFileInfo> Files { get; set; }
        public Exception? Exception { get; set; }

        private void AddFileProviderToReport(IFileProvider f, FileProviderMiddleWareReport report)
        {
            if (f is EmbeddedFileProvider)
            {
                var embeddedFileProvider = (EmbeddedFileProvider)f;
                report.FileProviders.Add("embedded");
            }

            if (f is PhysicalFileProvider)
            {
                var physicalFileProvider = (PhysicalFileProvider)f;
                report.FileProviders.Add($"physical {physicalFileProvider.Root}");
            }
        }
    }
}


