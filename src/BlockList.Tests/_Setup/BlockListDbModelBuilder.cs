using System;
using System.Collections.Generic;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Umbraco.Core;
using Newtonsoft.Json.Linq;
using Autofac;
using Rhino.Mocks;
using System.Linq.Expressions;

namespace YuzuDelivery.Umbraco.BlockList.Tests.Inline
{
    public partial class BlockListGridContentMapperTests
    {
        public class BlockListDbModelBuilder
        {
            private UmbracoApiTestMocks umb;
            private NamesHelper names;

            private GuidFactory guidFactory;

            public BlockListDbModel Expected { get; set; }

            public BlockListDbModel.LayoutItem CurrentLayoutItem { get; set; }
            public JObject CurrentContentData { get; set; }
            public JObject CurrentSettingsData { get; set; }

            public BlockListDbModelBuilder(UmbracoApiTestMocks umb, NamesHelper names, IContainer container)
            {
                this.umb = umb;
                this.names = names;

                Expected = new BlockListDbModel();

                guidFactory = container.Resolve<GuidFactory>();

                CurrentLayoutItem = new BlockListDbModel.LayoutItem();

            }

            public BlockListDbModelBuilder AddContentData<V>()
            {
                return AddContentData(names.VmToContentType<V>());
            }

            public BlockListDbModelBuilder AddContentData(string contentTypeName)
            {
                var content = CreateData("737353f6-1e0c-4534-bbf9-8993de48e380", contentTypeName);

                CurrentLayoutItem.ContentUdi = content["udi"].ToString();

                Expected.Layout.UmbracoBlockList.Add(CurrentLayoutItem);

                CurrentContentData = JObject.FromObject(content);
                Expected.ContentData.Add(CurrentContentData);

                return this;
            }

            public BlockListDbModelBuilder AddSettingsData<V>()
            {
                return AddSettingsData(names.VmToContentType<V>());
            }

            public BlockListDbModelBuilder AddSettingsData(string contentTypeName)
            {
                var settings = CreateData("4ff03677-4f42-46a8-889c-1e1c0c6cef96", contentTypeName);

                CurrentLayoutItem.SettingsUdi = settings["udi"].ToString();

                CurrentSettingsData = JObject.FromObject(settings);
                Expected.SettingsData.Add(CurrentSettingsData);

                return this;
            }

            public BlockListDbModelBuilder AddContentProperty<V>(Expression<Func<V, object>> property, object value)
            {
                AddContentProperty(names.AsAlias(property.GetMemberName()), value);
                return this;
            }

            public BlockListDbModelBuilder AddContentProperty(string name, object value)
            {
                if(value.GetType() == typeof(System.String))
                    CurrentContentData.Add(new JProperty(name, value));
                else
                    CurrentContentData.Add(new JProperty(name, JObject.FromObject(value)));
                return this;
            }

            public BlockListDbModelBuilder AddSettingsProperty<V>(Expression<Func<V, object>> property, object value)
            {
                AddSettingsProperty(names.AsAlias(property.GetMemberName()), value);
                return this;
            }

            public BlockListDbModelBuilder AddSettingsProperty(string name, object value)
            {
                if (value.GetType() == typeof(System.String))
                    CurrentSettingsData.Add(new JProperty(name, value));
                else
                    CurrentSettingsData.Add(new JProperty(name, JObject.FromObject(value)));
                return this;
            }





            private Dictionary<string, object> CreateData(string guid, string contentTypeName)
            {
                var contentTypeKey = umb.ContentType.Current.TypesByName[contentTypeName].Key;

                var udiGuid = Guid.Parse(guid);
                guidFactory.Stub(x => x.CreateNew(contentTypeKey)).Return(udiGuid);
                var elementUdi = Udi.Create("element", udiGuid).ToString();

                var output = new Dictionary<string, object>();
                output["contentTypeKey"] = contentTypeKey;
                output["udi"] = elementUdi;
                return output;
            }

        }

    }
}
