using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping;

internal class YuzuUmbracoMappingIndex : IYuzuMappingIndex
{
    private readonly IDictionary<string, Type> _map;

    public YuzuUmbracoMappingIndex(IOptions<YuzuConfiguration> config)
    {
        _map = new Dictionary<string, Type>();

        foreach (var viewModel in config.Value.ViewModels)
        {
            if (viewModel.GetCustomAttribute(typeof(YuzuMapAttribute)) is not YuzuMapAttribute yuzuMapAttribute)
            {
                continue;
            }

            _map[yuzuMapAttribute.SourceTypeName] = viewModel;
        }
    }

    public Type GetViewModelType(Type cmsType)
    {
        if (!_map.TryGetValue(cmsType.Name, out var viewModelType))
        {
            throw new InvalidOperationException($"Unknown type {cmsType.FullName}");
        }

        return viewModelType;
    }
}
