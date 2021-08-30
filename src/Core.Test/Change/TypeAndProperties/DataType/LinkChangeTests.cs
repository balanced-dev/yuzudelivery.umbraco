using System;
using System.Collections.Generic;
using Umbraco.Web.PropertyEditors;
using NUnit.Framework;
using Autofac;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;

namespace YuzuDelivery.Umbraco.Core.Tests
{
    [Category("Change / Property and DataType")]
    public class LinkChangeTests : BaseTestSetup
    {
        public class vmBlock_Test {
            public vmBlock_DataLink Link { get; set; }
        }

        public SchemaChangeController svc;
        public VmToContentPropertyMap map;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            BaseFixtureSetup();
        }

        [SetUp]
        public void Setup()
        {
            BaseSetup();

            svc = container.Resolve<SchemaChangeController>();

            umb.Config.AddViewModel<vmBlock_Test>();

            map = new VmToContentPropertyMap()
            {
                VmName = "vmBlock_Test",
                VmPropertyName = "Link",
                Config = new ContentPropertyConfig()
                {
                    TypeName = "vmBlock_DataLink"
                }
            };

            umb.ContentType.ForUpdating<vmBlock_Test>();
            umb.ImportConfig.IgnoreViewmodels.Add("vmBlock_DataLink");
        }

        [Test]
        public void can_create_single_link_picker_data_type()
        {
            umb.DataType.AddAndStubCreate(1, "Url Picker", "Umbraco.MultiUrlPicker")
                .PropertyType.ForCreating<vmBlock_Test>(x => x.Link, 1, 1);

            svc.ChangeProperty(map);

            Assert.IsInstanceOf<MultiUrlPickerConfiguration>(umb.DataType.Current.Configuration);
            Assert.AreEqual("Url Picker", umb.DataType.Current.Name);
        }

        [Test]
        public void can_create_link_property()
        {
            umb.DataType.AddAndStubGet(1, "Url Picker")
                .PropertyType.ForCreating<vmBlock_Test>(x => x.Link, 1, 1);

            svc.ChangeProperty(map);

            umb.PropertyType.WasCreated<vmBlock_Test>(x => x.Link);
        }

    }

}
