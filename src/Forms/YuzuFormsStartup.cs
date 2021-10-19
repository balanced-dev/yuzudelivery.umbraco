using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Core.ViewModelBuilder;

#if NETCOREAPP 
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace YuzuDelivery.Umbraco.Forms
{
#if NETCOREAPP
    public class YuzuFormsStartup : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IViewRenderService, ViewRenderService>();

            builder.Services.AddSingleton<IFormElementMapGetter, FormElementMapGetter>();
            builder.RegisterAll<IFormFieldMappingsInternal>(typeof(YuzuFormsStartup).Assembly);

            builder.Services.AddTransient<FormBuilderTypeConverter>();
            builder.Services.AddTransient<FormTypeConvertor>();
            builder.Services.AddTransient(typeof(FormValueResolver<,>));

            //MUST be transient lifetimes
            builder.Services.AddTransient(typeof(IUpdateableConfig), typeof(FormUmbracoConfig));
            builder.Services.AddTransient(typeof(IUpdateableVmBuilderConfig), typeof(FormVmBuilderConfig));
            builder.Services.AddTransient(typeof(IUpdateableImportConfiguration), typeof(FormImportConfig));

            builder.Services.AddTransient<YuzuMappingConfig, FormMappingConfig>();

            builder.Services.AddTransient(typeof(YuzuMappingConfig), typeof(FormAutoMapping));
        }
    }
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuFormsStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IFormElementMapGetter, FormElementMapGetter>(Lifetime.Singleton);
            composition.RegisterAll<IFormFieldMappingsInternal>(typeof(YuzuFormsStartup).Assembly);

            composition.Register<FormBuilderTypeConverter>();
            composition.Register<FormTypeConvertor>();
            composition.Register(typeof(FormValueResolver<,>));

            //MUST be transient lifetimes
            composition.Register(typeof(IUpdateableConfig), typeof(FormUmbracoConfig), Lifetime.Transient);
            composition.Register(typeof(IUpdateableVmBuilderConfig), typeof(FormVmBuilderConfig), Lifetime.Transient);
            composition.Register(typeof(IUpdateableImportConfiguration), typeof(FormImportConfig), Lifetime.Transient);

            composition.Register<YuzuMappingConfig, FormMappingConfig>();

            composition.Register(typeof(YuzuMappingConfig), typeof(FormAutoMapping));
        }
    }
#endif

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