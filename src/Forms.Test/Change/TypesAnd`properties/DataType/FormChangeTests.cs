using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.PropertyEditors;
using NUnit.Framework;
using Autofac;
using YuzuDelivery.Umbraco.Forms;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Umbraco.Forms.Core.PropertyEditors;

namespace Yuzu.Delivery.Forms.Tests.Integration.ChangeItem
{
    [Category("Change / Property and DataType")]
    public class FormChangeTests : BaseTestSetup
    {
        public class vmBlock_Test
        {
            public vmBlock_Form Form { get; set; }
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
                VmPropertyName = "Form",
                Config = new ContentPropertyConfig()
                {
                    TypeName = "vmBlock_DataForm"
                }
            };

            umb.ContentType.ForUpdating<vmBlock_Test>();
            umb.ImportConfig.IgnoreViewmodels.Add("vmBlock_DataForm");
        }



        [Test]
        public void can_create_form_picker_data_type()
        {
            umb.DataType.AddParameterEditor("UmbracoForms.FormPicker");

            umb.DataType.AddAndStubCreate(1, "Form Picker", "UmbracoForms.FormPicker")
                .PropertyType.ForCreating<vmBlock_Test>(x => x.Form, 1, 1);

            svc.ChangeProperty(map);

            Assert.IsInstanceOf<FormPickerConfiguration>(umb.DataType.Current.Configuration);
            Assert.AreEqual("Form Picker", umb.DataType.Current.Name);
        }

        [Test]
        public void can_create_form_picker_property()
        {

            umb.DataType.AddAndStubGet(1, "Form Picker")
                .PropertyType.ForCreating<vmBlock_Test>(x => x.Form, 1, 1);

            svc.ChangeProperty(map);

            umb.PropertyType.WasCreated<vmBlock_Test>(x => x.Form);
        }

    }
}
