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
        if (!context.Request.Query.ContainsKey("showjson"))
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
        var viewModel = _mapper.Map(currentPage, viewModelType, new Dictionary<string, object>
        {
            ["HtmlHelper"] = GetHtmlHelper(context) // Required for yuzu forms (ViewComponent -> string)
        });

        await context.Response.WriteAsJsonAsync(
            viewModel,
            viewModel.GetType(),
            new JsonSerializerOptions(),
            MediaTypeNames.Application.Json);
    }

    private IHtmlHelper GetHtmlHelper(HttpContext httpContext)
    {
        using var scope = _scopeFactory.CreateScope();

        var mmp = scope.ServiceProvider.GetRequiredService<IModelMetadataProvider>();
        var viewData = new ViewDataDictionary(mmp, new ModelStateDictionary());

        var tdp = scope.ServiceProvider.GetRequiredService<ITempDataProvider>();
        var tempData = new TempDataDictionary(httpContext, tdp);

        var viewContext = new ViewContext(
            new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor())),
            NullView.Instance,
            viewData,
            tempData,
            new StringWriter(),
            new HtmlHelperOptions());

        var html = scope.ServiceProvider.GetRequiredService<IHtmlHelper>();
        ((IViewContextAware)html).Contextualize(viewContext);
        return html;
    }

    private class NullView : IView
    {
        public static readonly NullView Instance = new();

        public string Path => string.Empty;

        public Task RenderAsync(ViewContext context) => Task.CompletedTask;
    }
}


