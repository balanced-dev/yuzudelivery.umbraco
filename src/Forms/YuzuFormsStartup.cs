using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Forms
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuFormsStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            foreach(var assembly in YuzuDeliveryForms.Configuration.FormElementAssemblies)
            {
                var formElementMappings = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(IFormFieldMappings)));
                foreach (var f in formElementMappings)
                {
                    composition.Register(typeof(IFormFieldMappings), f);
                }

                var formElementPostProcessors = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(IFormFieldPostProcessor)));
                foreach (var f in formElementPostProcessors)
                {
                    composition.Register(typeof(IFormFieldPostProcessor), f);
                }
            }

            var types = typeof(YuzuFormsStartup).Assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(IFormFieldMappingsInternal)));
            foreach (var f in types)
            {
                composition.Register(typeof(IFormFieldMappingsInternal), f);
            }

            composition.Register<IFormElementMapGetter, FormElementMapGetter>(Lifetime.Singleton);

            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_Form>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_DataForm>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_DataFormBuilder>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmSub_DataFormBuilderFieldset>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmSub_DataFormBuilderValidation>();

            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_FormButton>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_FormCheckboxRadio>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_FormCheckboxRadioList>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_FormHidden>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_FormSelect>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmSub_FormSelectOption>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_FormTextArea>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_FormTextInput>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_Recaptcha>();
            Yuzu.Configuration.ExcludeViewmodelsAtGeneration.Add<vmBlock_TitleAndDescription>();

            Yuzu.Configuration.AddNamespacesAtGeneration.Add("using YuzuDelivery.Umbraco.Forms;");

            YuzuDeliveryImport.Configuration.IgnoreProperties.Add("Form");
            YuzuDeliveryImport.Configuration.IgnoreProperties.Add("FormEndpoint");

            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_Form>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_FormButton>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_FormCheckboxRadio>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_FormCheckboxRadioList>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_FormHidden>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_FormSelect>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_FormTextArea>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_FormTextInput>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_Recaptcha>();
            YuzuDeliveryImport.Configuration.IgnoreViewmodels.Add<vmBlock_TitleAndDescription>();

        }
    }

}