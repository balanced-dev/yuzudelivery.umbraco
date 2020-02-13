using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core.ViewModelBuilder;

namespace YuzuDelivery.Umbraco.Forms
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuFormsStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            AddFormStrategies(composition);

            composition.Register<IFormElementMapGetter, FormElementMapGetter>(Lifetime.Singleton);

            //MUST be tranient lifetime
            composition.Register(typeof(IUpdateableVmBuilderConfig), typeof(FormVmBuilderConfig), Lifetime.Transient);

            //MUST be tranient lifetime
            composition.Register(typeof(IUpdateableImportConfiguration), typeof(FormImportConfig), Lifetime.Transient);

        }

        private void AddFormStrategies(Composition composition)
        {
            composition.Register<IYuzuDeliveryFormsConfiguration, YuzuDeliveryFormsConfiguration>();

            composition.Register<IFormFieldMappings[]>((factory) =>
            {
                var config = factory.GetInstance<IYuzuConfiguration>();
                var configForm = factory.GetInstance<IYuzuDeliveryFormsConfiguration>();
                var formElementAssemblies = config.ViewModelAssemblies;
                var items = new List<IFormFieldMappings>();

                if (configForm != null && configForm.FormElementAssemblies.Any())
                    formElementAssemblies = configForm.FormElementAssemblies;

                foreach (var assembly in formElementAssemblies)
                {
                    var formElementMappings = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(IFormFieldMappings)));
                    foreach (var f in formElementMappings)
                    {
                        var o = Activator.CreateInstance(f) as IFormFieldMappings;
                        items.Add(o);
                    }
                }

                return items.ToArray();
            });


            composition.Register<IFormFieldPostProcessor[]>((factory) =>
            {
                var config = factory.GetInstance<IYuzuConfiguration>();
                var configForm = factory.GetInstance<IYuzuDeliveryFormsConfiguration>();
                var formElementAssemblies = config.ViewModelAssemblies;
                var items = new List<IFormFieldPostProcessor>();

                if (configForm != null && configForm.FormElementAssemblies.Any())
                    formElementAssemblies = configForm.FormElementAssemblies;

                foreach (var assembly in formElementAssemblies)
                {
                    var formElementMappings = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(IFormFieldPostProcessor)));
                    foreach (var f in formElementMappings)
                    {
                        var o = Activator.CreateInstance(f) as IFormFieldPostProcessor;
                        items.Add(o);
                    }
                }

                return items.ToArray();
            });

            var types = typeof(YuzuFormsStartup).Assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(IFormFieldMappingsInternal)));
            foreach (var f in types)
            {
                composition.Register(typeof(IFormFieldMappingsInternal), f);
            }
        }
    }

    public class FormImportConfig : UpdateableImportConfiguration
    {
        public FormImportConfig()
            : base()
        {
            IgnoreProperties.Add("Form");
            IgnoreProperties.Add("FormEndpoint");

            IgnoreViewmodels.Add<vmBlock_Form>();
            IgnoreViewmodels.Add<vmBlock_FormButton>();
            IgnoreViewmodels.Add<vmBlock_FormCheckboxRadio>();
            IgnoreViewmodels.Add<vmBlock_FormCheckboxRadioList>();
            IgnoreViewmodels.Add<vmBlock_FormHidden>();
            IgnoreViewmodels.Add<vmBlock_FormSelect>();
            IgnoreViewmodels.Add<vmBlock_FormTextArea>();
            IgnoreViewmodels.Add<vmBlock_FormTextInput>();
            IgnoreViewmodels.Add<vmBlock_Recaptcha>();
            IgnoreViewmodels.Add<vmBlock_TitleAndDescription>();
        }
    }

    public class FormVmBuilderConfig : UpdateableVmBuilderConfig
    {
        public FormVmBuilderConfig()
            : base()
        {
            ExcludeViewmodelsAtGeneration.Add<vmBlock_Form>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataForm>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_DataFormBuilder>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataFormBuilderFieldset>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_DataFormBuilderValidation>();

            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormButton>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormCheckboxRadio>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormCheckboxRadioList>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormHidden>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormSelect>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_FormSelectOption>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormTextArea>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormTextInput>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_Recaptcha>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_TitleAndDescription>();

            AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.Forms;");
        }
    }

}