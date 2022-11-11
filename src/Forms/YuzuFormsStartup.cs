using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core.ViewModelBuilder;
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Forms.Core.Providers;

namespace YuzuDelivery.Umbraco.Forms
{

    public class YuzuFormsStartup : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IFormElementMapGetter, FormElementMapGetter>();
            builder.Services.RegisterAll<IFormFieldMappingsInternal>(typeof(YuzuFormsStartup).Assembly);

            builder.Services.AddTransient<FormBuilderTypeConverter>();
            builder.Services.AddTransient<FormTypeConvertor>();
            builder.Services.AddTransient(typeof(FormValueResolver<,>));

            builder.Services.AddTransient<ViewComponentHelper>();

            //MUST be transient lifetimes
            builder.Services.AddTransient(typeof(IUpdateableConfig), typeof(FormUmbracoConfig));
            builder.Services.AddTransient(typeof(IUpdateableVmBuilderConfig), typeof(FormVmBuilderConfig));
            builder.Services.AddTransient(typeof(IUpdateableImportConfiguration), typeof(FormImportConfig));

            builder.Services.AddTransient<YuzuMappingConfig, FormMappingConfig>();

            builder.Services.AddTransient(typeof(YuzuMappingConfig), typeof(FormAutoMapping));


            // Register custom FieldTypes
            builder.WithCollectionBuilder<FieldCollectionBuilder>()
                .Add<ColumnBlank>();
        }
    }

    public class FormUmbracoConfig : UpdateableConfig
    {
        public FormUmbracoConfig()
            : base()
        {
            MappingAssemblies.Add(typeof(YuzuFormsStartup).Assembly);
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
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormBlank>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormCheckboxRadio>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_FormCheckboxRadioDataValue>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormCheckboxRadioList>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormHidden>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormSelect>();
            ExcludeViewmodelsAtGeneration.Add<vmSub_FormSelectOption>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormTextArea>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_FormTextInput>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_Recaptcha>();
            ExcludeViewmodelsAtGeneration.Add<vmBlock_TitleAndDescription>();

            AddNamespacesAtGeneration.Add("YuzuDelivery.Umbraco.Forms");
        }
    }

    public class FormImportConfig : UpdateableImportConfiguration
    {
        public FormImportConfig(IVmPropertyFinder vmPropertyFinder)
            : base()
        {
            IgnoreViewmodels.Add<vmBlock_DataForm>();
            IgnoreViewmodels.Add<vmBlock_DataFormBuilder>();
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

            SpecialistProperties.Add("Forms", vmPropertyFinder.GetProperties(typeof(vmBlock_DataForm)));
        }
    }

}
