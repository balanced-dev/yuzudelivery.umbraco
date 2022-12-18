using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.PropertyEditors;
using NUnit.Framework;
using Autofac;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;

namespace YuzuDelivery.Umbraco.Core.Tests
{
    [Category("Change / Property and DataType" +
        "" +
        "")]
    public class ImageChangeTests : BaseTestSetup
    {
        public class vmBlock_Test {
            public vmBlock_DataImage Image { get; set; }
        }

        public SchemaChangeController svc;
        public VmToContentPropertyMap map;

        [OneTimeSetUp]
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
                VmPropertyName = "Image",
                Config = new ContentPropertyConfig()
                {
                    TypeName = "vmBlock_DataImage"
                }
            };

            umb.ContentType.ForUpdating<vmBlock_Test>();

            umb.ImportConfig.IgnoreViewmodels.Add("vmBlock_DataImage");
        }

        [Test]
        public void can_create_image_property()
        {
            umb.DataType.AddAndStubGet(1, "Media Picker")
                .PropertyType.ForCreating<vmBlock_Test>(x => x.Image, 1, 1);

            svc.ChangeProperty(map);

            umb.PropertyType.WasCreated<vmBlock_Test>(x => x.Image, groupName: string.Empty);
        }

    }
}
