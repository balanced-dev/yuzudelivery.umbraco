using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Web.Common.Routing;
using YuzuDelivery.Core.Mapping;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace YuzuDelivery.Umbraco.Core.Middleware;

public class YuzuRenderJsonMiddleware : IMiddleware
{
    private readonly IYuzuMappingIndex _mappingIndex;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public YuzuRenderJsonMiddleware(IYuzuMappingIndex mappingIndex, IMapper mapper, IServiceScopeFactory scopeFactory)
    {
        _mappingIndex = mappingIndex;
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // ReSharper disable once StringLiteralTypo
        if (!ShouldHandle(context))
        {
            await next(context);
            return;
        }

        var routeValues = context.Features.Get<UmbracoRouteValues>();
        var currentPage = routeValues?.PublishedRequest.PublishedContent;

        if (currentPage == null)
        {
            await next(context);
            return;
        }

        var viewModelType = _mappingIndex.GetViewModelType(currentPage.GetType());
        var viewModel = _mapper.Map(currentPage, viewModelType);

        await context.Response.WriteAsJsonAsync(
            viewModel,
            viewModel.GetType(),
            new JsonSerializerOptions(),
            MediaTypeNames.Application.Json);
    }

    private bool ShouldHandle(HttpContext context)
    {
        if (context.Request.Query.ContainsKey("showjson"))
        {
            return true;
        }

        if (context.Request.Headers.TryGetValue("Accept", out var accept))
        {
            if (accept.ToString().Contains(MediaTypeNames.Application.Json))
            {
                return true;
            }
        }

        return false;
    }
}


