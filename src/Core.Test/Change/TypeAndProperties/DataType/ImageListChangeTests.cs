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
    public class ImageListChangeTests : BaseTestSetup
    {
        public class vmBlock_Test {
            public List<vmBlock_DataImage> Image { get; set; }
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
                VmPropertyName = "Image",
                Config = new ContentPropertyConfig()
                {
                    TypeName = "vmBlock_DataImage",
                    IsList = true
                }
            };

            umb.ContentType.ForUpdating<vmBlock_Test>();
            umb.ImportConfig.IgnoreViewmodels.Add("vmBlock_DataImage");
        }

        [Test]
        public void can_create_multi_image_property()
        {
            umb.DataType.AddAndStubGet(1, "Multiple Media Picker")
                .PropertyType.ForCreating<vmBlock_Test>(x => x.Image, 1, 1);

            svc.ChangeProperty(map);

            umb.PropertyType.WasCreated<vmBlock_Test>(x => x.Image);
        }

    }
}
