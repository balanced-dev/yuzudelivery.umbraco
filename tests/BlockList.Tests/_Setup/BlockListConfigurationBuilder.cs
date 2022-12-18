using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.PropertyEditors;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;

namespace YuzuDelivery.Umbraco.BlockList.Tests
{
    public class BlockListConfigurationBuilder
    {
        private UmbracoApiTestMocks umb;

        public BlockListConfiguration Expected { get; }

        public BlockListConfigurationBuilder(UmbracoApiTestMocks umb)
        {
            this.umb = umb;

            Expected = new BlockListConfiguration();
            Expected.ValidationLimit = new BlockListConfiguration.NumberRange();
            Expected.Blocks = new List<BlockListConfiguration.BlockConfiguration>().ToArray();
        }

        public void AddBlock(string label, string contentTypeName, string settingsTypeName = null, bool forceHideContentEditor = false, string customView = null, string thumbnail = null)
        {
            var contentTypeKey = umb.ContentType.Current.TypesByName[contentTypeName].Key;
            Guid? settingsTypeKey = null;
            if(!string.IsNullOrWhiteSpace(settingsTypeName)) settingsTypeKey = umb.ContentType.Current.TypesByName[settingsTypeName].Key;

            var current = Expected.Blocks.ToList();
            current.Add(new BlockListConfiguration.BlockConfiguration()
            {
                View = customView == null ? BlockListDataTypeFactory.DefaultCustomView : customView,
                EditorSize = "medium",
                ContentElementTypeKey = contentTypeKey,
                SettingsElementTypeKey = settingsTypeKey,
                Label = label,
                Thumbnail = thumbnail,
                ForceHideContentEditorInOverlay = forceHideContentEditor
            });
            Expected.Blocks = current.ToArray();
        }

    }
}
