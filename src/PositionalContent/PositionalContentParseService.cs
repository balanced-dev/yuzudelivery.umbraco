using System;
using System.Collections.Generic;
using System.Linq;
using Hifi.PositionalContent;
using YuzuDelivery.Core;
using Umbraco.Web.Models;
using Umbraco.Web;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public class PositionalContentParseService
    {

        protected IPosConContentItem[] posConContentItems;
        protected IPosConContentItemInternal[] posConContentItemsInternal;
        protected IPosConImageItem[] posConImageItems;
        protected IPosConImageItemInternal[] posConImageItemInternal;

        public PositionalContentParseService(
            IPosConContentItem[] posConContentItems, 
            IPosConContentItemInternal[] posConContentItemsInternal,
            IPosConImageItem[] posConImageItems,
            IPosConImageItemInternal[] posConImageItemInternal)
        {
            this.posConContentItems = posConContentItems;
            this.posConContentItemsInternal = posConContentItemsInternal;
            this.posConImageItems = posConImageItems;
            this.posConImageItemInternal = posConImageItemInternal;
        }

        public object ItemContent(PositionalContentModel model, PositionalContentItem item, IDictionary<string, object> contextItems, PositionalContentItemDimension dimension = null, bool showOverriddenOnly = true)
        {
            if (showOverriddenOnly && dimension != null && !dimension.OverrideContent && !dimension.OverrideSettings)
                return null;

            object output = null;

            IPublishedElement content = null;
            IPublishedElement settings = null;

            if (dimension != null)
            {
                if (dimension.HasContent && dimension.OverrideContent)
                    content = dimension.GetContent(model);
                if (dimension.HasSettings && dimension.OverrideSettings)
                    settings = dimension.GetSetting(model);
            }

            if (item.HasContent && content == null)
                content = item.GetContent(model);
            if (item.HasSettings && settings == null)
                settings = item.GetSetting(model);

            if (settings != null)
                contextItems.Add("PositionalContentSettings", settings);

            foreach (var i in posConContentItems)
            {
                if(i.IsValid(content))
                {
                    output = i.Apply(content, settings, contextItems);
                    AddRefProperty(output);
                    return output;
                }
            }

            foreach (var i in posConContentItemsInternal)
            {
                if (i.IsValid(content))
                {
                    output = i.Apply(content, settings, contextItems);
                    AddRefProperty(output);
                    return output;
                }
            }

            return output;
        }

        public object ImageContent(PositionalContentModel model, IDictionary<string, object> contextItems, PositionalContentBreakpoint breakpoint = null, bool showOverriddenOnly = true)
        {
            if (showOverriddenOnly && breakpoint != null && !breakpoint.OverrideContent && !breakpoint.OverrideSettings)
                return null;

            object output = null;

            IPublishedElement content = null;
            IPublishedElement settings = null;

            if (breakpoint != null)
            {
                if (breakpoint.HasContent && breakpoint.OverrideContent)
                    content = breakpoint.GetContent(model);
                if (breakpoint.HasSettings && breakpoint.OverrideSettings)
                    settings = breakpoint.GetSetting(model);
            }

            if (model.HasContent && content == null)
                content = model.GetContent();
            if (model.HasSettings && settings == null)
                settings = model.GetSetting();

            if (settings != null)
                contextItems.Add("PositionalContentSettings", settings);

            foreach (var i in posConImageItems)
            {
                if (i.IsValid(content))
                {
                    output = i.Apply(model, content, settings, contextItems);
                    AddRefProperty(output);
                    return output;
                }
            }

            foreach (var i in posConImageItemInternal)
            {
                if (i.IsValid(content))
                {
                    output = i.Apply(model, content, settings, contextItems);
                    AddRefProperty(output);
                    return output;
                }
            }

            return output;
        }

        public object AddRefProperty(object output)
        {
            var refProperty = output.GetType().GetProperty("_ref");
            if (refProperty != null)
                refProperty.SetValue(output, output.GetType().Name.Replace(YuzuConstants.Configuration.BlockPrefix, "par"));
            return output;
        }

        public vmSub_DataPositionalContentDimension Dimension(PositionalContentModel model, PositionalContentItem item, KeyValuePair<string, PositionalContentItemDimension> dimension, IDictionary<string, object> contextItems)
        {
            return new vmSub_DataPositionalContentDimension()
            {
                BreakpointName = dimension.Key,
                Styles = dimension.Value.PositionalStyles(),
                ContentOverride = ItemContent(model, item, contextItems, dimension.Value),
                IsHidden = dimension.Value.Hide
            };
        }

    }
}
