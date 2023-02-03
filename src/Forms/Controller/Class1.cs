// Decompiled with JetBrains decompiler
// Type: Umbraco.Forms.Web.Controllers.UmbracoFormsController
// Assembly: Umbraco.Forms.Web, Version=10.1.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 0FD466A0-B8F6-4278-9456-7FA192A5EC85
// Assembly location: C:\FrontendProjects\Yuzu\backend\BaseProject\delivery.src\Yuzu.Base.Web\bin\Debug\net6.0\Umbraco.Forms.Web.dll

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.DependencyInjection;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;
using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.Services;


#nullable enable
namespace Umbraco.Forms.Web.Controllers
{
    public class YuzuUmbracoFormsController : SurfaceController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRecordService _recordService;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IFieldPreValueSourceService _fieldPreValueSourceService;
        private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IMemberManager _memberManager;
        private readonly IPlaceholderParsingService _placeholderParsingService;
        private readonly IFormRenderingService _formRenderingService;
        private readonly IEventMessagesFactory _eventMessagesFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly PackageOptionSettings _packageOptionSettings;
        private readonly SecuritySettings _securitySettings;
        private const string FormsSubmittedKey = "UmbracoFormSubmitted";
        internal const string FormsSubmittedFromCurrentPageKey = "UmbracoFormSubmittedFromCurrentPage";
        internal const string FormsSubmittedQuerystringKey = "formSubmitted";

        [Obsolete("Please use the constructor taking all parameters. This constructor will be removed in a future version.")]
        public YuzuUmbracoFormsController(
          IUmbracoContextAccessor umbracoContextAccessor,
          IUmbracoDatabaseFactory databaseFactory,
          ServiceContext services,
          AppCaches appCaches,
          IProfilingLogger profilingLogger,
          IPublishedUrlProvider publishedUrlProvider,
          IHostingEnvironment hostingEnvironment,
          IRecordService recordService,
          IFieldTypeStorage fieldTypeStorage,
          IFieldPreValueSourceService fieldPreValueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
          UmbracoHelper umbracoHelper,
          IMemberManager memberManager,
          IPlaceholderParsingService placeholderParsingService,
          IFormRenderingService formViewModelFactory,
          IEventMessagesFactory eventMessagesFactory,
          IEventAggregator eventAggregator,
          IOptions<PackageOptionSettings> packageOptionSettings)
          : this(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider, hostingEnvironment, recordService, fieldTypeStorage, fieldPreValueSourceService, fieldPreValueSourceTypeService, umbracoHelper, memberManager, placeholderParsingService, formViewModelFactory, eventMessagesFactory, eventAggregator, packageOptionSettings, StaticServiceProvider.Instance.GetRequiredService<IOptions<SecuritySettings>>())
        {
        }

        [ActivatorUtilitiesConstructor]
        public YuzuUmbracoFormsController(
          IUmbracoContextAccessor umbracoContextAccessor,
          IUmbracoDatabaseFactory databaseFactory,
          ServiceContext services,
          AppCaches appCaches,
          IProfilingLogger profilingLogger,
          IPublishedUrlProvider publishedUrlProvider,
          IHostingEnvironment hostingEnvironment,
          IRecordService recordService,
          IFieldTypeStorage fieldTypeStorage,
          IFieldPreValueSourceService fieldPreValueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
          UmbracoHelper umbracoHelper,
          IMemberManager memberManager,
          IPlaceholderParsingService placeholderParsingService,
          IFormRenderingService formViewModelFactory,
          IEventMessagesFactory eventMessagesFactory,
          IEventAggregator eventAggregator,
          IOptions<PackageOptionSettings> packageOptionSettings,
          IOptions<SecuritySettings> securitySettings)
          : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._recordService = recordService;
            this._fieldTypeStorage = fieldTypeStorage;
            this._fieldPreValueSourceService = fieldPreValueSourceService;
            this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            this._umbracoHelper = umbracoHelper;
            this._memberManager = memberManager;
            this._placeholderParsingService = placeholderParsingService;
            this._formRenderingService = formViewModelFactory;
            this._eventMessagesFactory = eventMessagesFactory;
            this._eventAggregator = eventAggregator;
            this._packageOptionSettings = packageOptionSettings.Value;
            this._securitySettings = securitySettings.Value;
        }

        [HttpPost]
        [ValidateFormsAntiForgeryToken]
        public IActionResult HandleForm(FormViewModel model)
        {
            Form form = this._formRenderingService.GetForm(model.FormId);
            if (form == null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
                interpolatedStringHandler.AppendLiteral("Could not load form with id ");
                interpolatedStringHandler.AppendFormatted<Guid>(model.FormId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            model.Build(form, this._fieldTypeStorage, this._fieldPreValueSourceService, this._fieldPreValueSourceTypeService, this.AppCaches, this._hostingEnvironment, this._placeholderParsingService, this._packageOptionSettings, this._securitySettings);
            if (this.HoneyPotIsEmpty(model))
            {
                this._formRenderingService.PrePopulateForm(form, this.HttpContext, model);
                model.FormState = this._formRenderingService.GetFormState(form, model, this.HttpContext);
                this._formRenderingService.StoreFormState(model.FormState, model);
                this._formRenderingService.ResumeFormState(model, model.FormState);
                if (this.NavigatingToPreviousPage(model))
                {
                    this.GoBackward(model);
                }
                else
                {
                    this.ValidateFormState(model, form);
                    if (this.ModelState.IsValid)
                        this.GoForward(form, model);
                }
                model.IsFirstPage = model.FormStep == 0;
                model.IsLastPage = model.FormStep == form.Pages.Count - 1;
            }
            else
                model.SubmitHandled = true;
            this.OnFormHandled(form, model);
            this._formRenderingService.StoreFormModel(this.TempData, model);
            if (!model.SubmitHandled)
                return (IActionResult)this.CurrentUmbracoPage();
            this._formRenderingService.ClearFormModel(this.TempData);
            this._formRenderingService.ClearFormState(model);
            this.TempData["UmbracoFormSubmitted"] = (object)model.FormId;
            if (this.HttpContext.Items.ContainsKey((object)"FormsRedirectAfterFormSubmitUrl"))
                return (IActionResult)this.Redirect(this.HttpContext.Items[(object)"FormsRedirectAfterFormSubmitUrl"].ToString());
            QueryString queryString = QueryString.Create("formSubmitted", model.FormId.ToString());
            IPublishedContent publishedContent = this._umbracoHelper.Content(form.GoToPageOnSubmit);
            if (publishedContent != null)
                return this._packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission ? (IActionResult)this.RedirectToUmbracoPage(publishedContent.Key, queryString) : (IActionResult)this.RedirectToUmbracoPage(publishedContent.Key);
            this.TempData["UmbracoFormSubmittedFromCurrentPage"] = (object)model.FormId;
            return this._packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission ? (IActionResult)this.RedirectToCurrentUmbracoPage(queryString) : (IActionResult)this.RedirectToCurrentUmbracoPage();
        }

        private bool NavigatingToPreviousPage(FormViewModel model)
        {
            if (model.FormStep == 0 || !this.Request.HasFormContentType)
                return false;
            return !string.IsNullOrEmpty((string)this.Request.Form["__prev"]) || !string.IsNullOrEmpty((string)this.Request.Form["PreviousClicked"]);
        }

        protected virtual void OnFormHandled(Form form, FormViewModel model)
        {
        }

        private void GoBackward(FormViewModel model)
        {
            --model.FormStep;
            if (model.FormStep >= 0)
                return;
            model.FormStep = 0;
        }

        private void GoForward(Form form, FormViewModel model)
        {
            ++model.FormStep;
            if (model.FormStep != form.Pages.Count<Page>())
                return;
            this.SubmitForm(form, model);
        }

        private void SubmitForm(Form form, FormViewModel model)
        {
            IProfilingLogger profilingLogger = this.ProfilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
            interpolatedStringHandler.AppendLiteral("Umbraco Forms: Submitting Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<UmbracoFormsController>(stringAndClear))
            {
                model.SubmitHandled = true;
                Record record1 = new Record();
                if (model.RecordId != Guid.Empty)
                {
                    Record record2 = this._formRenderingService.GetRecord(model.RecordId, form);
                    if (record2 != null)
                        record1 = record2;
                }
                record1.Form = form.Id;
                record1.State = FormState.Submitted;
                Record record3 = record1;
                IPublishedContent currentPage = this.CurrentPage;
                int num = currentPage != null ? currentPage.Id : 0;
                record3.UmbracoPageId = num;
                record1.IP = this.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                record1.Culture = Thread.CurrentThread.CurrentCulture.Name;
                bool flag = false;
                string str = (string)null;
                if (this.HttpContext != null && this.HttpContext.User != null && this.HttpContext.User.Identity != null)
                {
                    flag = this.HttpContext.User.Identity.IsAuthenticated;
                    str = this.HttpContext.User.Identity.Name;
                }
                if (flag && !string.IsNullOrEmpty(str) && this.HttpContext?.User != null)
                {
                    MemberIdentityUser result = this._memberManager.GetUserAsync(this.HttpContext.User).GetAwaiter().GetResult();
                    if (result != null)
                        record1.MemberKey = result.Key.ToString();
                }
                Dictionary<Guid, string> valuesForConditions = FieldConditionEvaluation.GetFormFieldValuesForConditions(model.FormState, form.AllFields, this._fieldTypeStorage, this.ControllerContext.HttpContext);
                foreach (FieldSet allFieldSet in form.AllFieldSets)
                {
                    foreach (Field allField in allFieldSet.AllFields)
                    {
                        object[] valueToStore = allField.GetValueToStore(model.FormState, this._fieldTypeStorage, this.ControllerContext.HttpContext, true);
                        RecordField recordField1 = record1.GetRecordField(allField.Id);
                        if (recordField1 != null)
                        {
                            recordField1.Values.Clear();
                            this.SetRecordFieldValues(recordField1, form, allFieldSet, allField, (IDictionary<Guid, string>)valuesForConditions, valueToStore);
                        }
                        else
                        {
                            RecordField recordField2 = new RecordField(allField);
                            this.SetRecordFieldValues(recordField2, form, allFieldSet, allField, (IDictionary<Guid, string>)valuesForConditions, valueToStore);
                            record1.RecordFields.Add(allField.Id, recordField2);
                        }
                    }
                }
                this._formRenderingService.ClearFormState(model);
                this._recordService.Submit(record1, form);
                this.AddRecordIdToTempData(record1);
            }
        }

        private void SetRecordFieldValues(
          RecordField recordField,
          Form form,
          FieldSet fieldSet,
          Field field,
          IDictionary<Guid, string> formFieldValues,
          object[] fieldValues)
        {
            if (!this.IsFormElementValidForConditions(form, (IConditioned)fieldSet, formFieldValues) || !this.IsFormElementValidForConditions(form, (IConditioned)field, formFieldValues))
                return;
            recordField.Values.AddRange((IEnumerable<object>)fieldValues);
        }

        private bool IsFormElementValidForConditions(
          Form form,
          IConditioned formElement,
          IDictionary<Guid, string> formFieldValues)
        {
            return formElement.Condition == null || formElement.Condition.IsCircular(form) || formElement.Condition.IsVisible(form, formFieldValues, this._placeholderParsingService);
        }

        private void AddRecordIdToTempData(Record record)
        {
            if (this.TempData.ContainsKey("Forms_Current_Record_id"))
                ((IDictionary<string, object>)this.TempData).Remove("Forms_Current_Record_id");
            this.TempData.Add("Forms_Current_Record_id", (object)record.UniqueId);
        }

        private void ValidateFormState(FormViewModel model, Form form)
        {
            if (!this.Request.HasFormContentType)
                return;
            YuzuUmbracoFormsController.PopulateFieldValues(model, form);
            Dictionary<Guid, string> dictionary = form.AllFields.ToDictionary<Field, Guid, string>((Func<Field, Guid>)(f => f.Id), (Func<Field, string>)(f => YuzuUmbracoFormsController.GetFieldValueForFormStateValidation(f)));
            foreach (FieldSet fieldSet in form.Pages[model.FormStep].FieldSets)
            {
                if (fieldSet.Condition == null || !fieldSet.Condition.Enabled || fieldSet.Condition.IsVisible(form, (IDictionary<Guid, string>)dictionary, this._placeholderParsingService))
                {
                    foreach (Field field in fieldSet.Containers.SelectMany<FieldsetContainer, Field>((Func<FieldsetContainer, IEnumerable<Field>>)(c => (IEnumerable<Field>)c.Fields)))
                    {
                        FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(field);
                        IFormCollection form1 = this.Request.Form;
                        Guid id = field.Id;
                        string key1 = id.ToString();
                        StringValues array = form1[key1];
                        if (fieldTypeByField.SupportsUploadTypes)
                        {
                            Dictionary<string, object[]> formState = model.FormState;
                            id = field.Id;
                            string key2 = id.ToString();
                            array = (StringValues)((IEnumerable<object>)formState[key2]).Select<object, string>((Func<object, string>)(x => ((IEnumerable<string>)x.ToString().Split(new string[1]
                         {
                "***|***"
                         }, StringSplitOptions.None)).Last<string>())).ToArray<string>();
                        }
                        foreach (string err in (IEnumerable<string>)((object)fieldTypeByField.ValidateField(form, field, (IEnumerable<object>)array, this.HttpContext, this._placeholderParsingService) ?? (object)new string[0]))
                        {
                            string validationErrorMessage = this._placeholderParsingService.ParsePlaceholdersForValidationErrorMessage(form, field, err);
                            ModelStateDictionary modelState = this.ModelState;
                            id = field.Id;
                            string key3 = id.ToString();
                            string errorMessage = validationErrorMessage;
                            modelState.AddModelError(key3, errorMessage);
                        }
                    }
                }
            }
            EventMessages messages = this._eventMessagesFactory.Get();
            this._eventAggregator.Publish<FormValidateNotification>(new FormValidateNotification(form, messages, this.HttpContext, this.ModelState));
        }

        private static string GetFieldValueForFormStateValidation(Field field)
        {
            string lower = true.ToString().ToLower();
            if (field.FieldTypeId == Guid.Parse("D5C0C390-AE9A-11DE-A69E-666455D89593") && field.Values.Count > 1 && field.Values.First<object>().ToString() == lower)
                field.Values = new List<object>() { (object)lower };
            return string.Join<object>(", ", (IEnumerable<object>)(field.Values ?? new List<object>()));
        }

        private static void PopulateFieldValues(FormViewModel model, Form form)
        {
            foreach (Field allField in form.AllFields)
            {
                object[] source;
                model.FormState.TryGetValue(allField.Id.ToString(), out source);
                allField.Values = source != null ? ((IEnumerable<object>)source).ToList<object>() : new List<object>();
            }
        }

        private bool HoneyPotIsEmpty(FormViewModel model) => !this.Request.HasFormContentType || string.IsNullOrEmpty((string)this.Request.Form[model.FormId.ToString().Replace("-", string.Empty)]);
    }

    internal static class PlaceholderParsingServiceExtensions
    {
        internal static string ParsePlaceholdersForValidationErrorMessage(
          this IPlaceholderParsingService placeholderParsingService,
          Form form,
          Field field,
          string err)
        {
            string validationErrorMessage = err;
            if (string.IsNullOrWhiteSpace(err))
                validationErrorMessage = string.Format(placeholderParsingService.ParsePlaceHolders(form.InvalidErrorMessage ?? string.Empty, form: form), (object)field.Caption);
            return validationErrorMessage;
        }
    }

    internal static class FieldExtensions
    {
        public static void PopulateDefaultValue(
          this Field field,
          IPlaceholderParsingService placeholderParsingService)
        {
            string str;
            if (field.Values != null && field.Values.Count > 0 && field.Values.All<object>((Func<object, bool>)(x => x != null)) || !field.Settings.TryGetValue("DefaultValue", out str) || string.IsNullOrEmpty(str))
                return;
            field.Values = new List<object>()
      {
        (object) placeholderParsingService.ParsePlaceHolders(str)
      };
        }

        public static object[] GetValueToStore(
          this Field field,
          Dictionary<string, object[]> formState,
          IFieldTypeStorage fieldTypeStorage,
          HttpContext httpContext,
          bool includeNonIdempotentFieldTypes)
        {
            object[] postedValues = new object[0];
            if (formState != null)
            {
                Dictionary<string, object[]> dictionary1 = formState;
                Guid id = field.Id;
                string key1 = id.ToString();
                if (dictionary1.ContainsKey(key1))
                {
                    Dictionary<string, object[]> dictionary2 = formState;
                    id = field.Id;
                    string key2 = id.ToString();
                    postedValues = dictionary2[key2];
                }
            }
            FieldType fieldTypeByField = fieldTypeStorage.GetFieldTypeByField(field);
            if (includeNonIdempotentFieldTypes || !fieldTypeByField.SupportsUploadTypes)
                postedValues = fieldTypeByField.ConvertToRecord(field, (IEnumerable<object>)postedValues, httpContext).ToArray<object>();
            return postedValues;
        }
    }
}
