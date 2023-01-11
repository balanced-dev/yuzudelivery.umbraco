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
using YuzuDelivery.Core.Mapping;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Import.Settings;

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

            builder.Services.Configure<YuzuConfiguration>(cfg =>
            {
                cfg.MappingAssemblies.Add(GetType().Assembly);
            });

            builder.Services.Configure<ViewModelGenerationSettings>(settings =>
            {
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_Form>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_DataForm>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_DataFormBuilder>();
                settings.ExcludeViewModelsAtGeneration.Add<vmSub_DataFormBuilderFieldset>();
                settings.ExcludeViewModelsAtGeneration.Add<vmSub_DataFormBuilderValidation>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormButton>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormBlank>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormCheckboxRadio>();
                settings.ExcludeViewModelsAtGeneration.Add<vmSub_FormCheckboxRadioDataValue>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormCheckboxRadioList>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormHidden>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormFileInput>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormSelect>();
                settings.ExcludeViewModelsAtGeneration.Add<vmSub_FormSelectOption>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormTextArea>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_FormTextInput>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_Recaptcha>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_Recaptcha3>();
                settings.ExcludeViewModelsAtGeneration.Add<vmBlock_TitleAndDescription>();

                settings.AddNamespacesAtGeneration.Add("YuzuDelivery.Umbraco.Forms");
            });

            //MUST be transient lifetimes
            builder.Services.AddOptions<ImportSettings>()
                   .Configure<IVmPropertyFinder>((settings, propertyFinder) =>
                   {
                       settings.IgnoreViewmodels.Add<vmBlock_DataForm>();
                       settings.IgnoreViewmodels.Add<vmBlock_DataFormBuilder>();
                       settings.IgnoreViewmodels.Add<vmBlock_Form>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormButton>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormCheckboxRadio>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormCheckboxRadioList>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormHidden>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormFileInput>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormSelect>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormTextArea>();
                       settings.IgnoreViewmodels.Add<vmBlock_FormTextInput>();
                       settings.IgnoreViewmodels.Add<vmBlock_Recaptcha>();
                       settings.IgnoreViewmodels.Add<vmBlock_Recaptcha3>();
                       settings.IgnoreViewmodels.Add<vmBlock_TitleAndDescription>();

                       settings.DataStructureProperties.Add("Forms", propertyFinder.GetProperties(typeof(vmBlock_DataForm)));
                   });

            builder.Services.AddTransient<YuzuMappingConfig, FormMappingConfig>();

            builder.Services.AddTransient(typeof(YuzuMappingConfig), typeof(FormAutoMapping));


            // Register custom FieldTypes
            builder.WithCollectionBuilder<FieldCollectionBuilder>()
                .Add<ColumnBlank>();
        }
    }
}
