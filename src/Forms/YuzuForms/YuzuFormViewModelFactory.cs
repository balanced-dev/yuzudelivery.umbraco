using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Forms;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Skybrud.Umbraco.GridData.Models;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Forms
{
    public class YuzuFormViewModelFactory
    {
        protected readonly IMapper mapper;
        protected readonly UmbracoContext umbracoContext;

        protected string CompletedTitle = "completedTitle";
        protected string CompletedBodyText = "completedBodyText";
        protected string Title = "title";
        protected string SubmitButtonText = "submitButtonText";
        protected const string ActionLinks = "actionLinks";
        protected const string CompletedActionLinks = "completedActionLinks";

        public YuzuFormViewModelFactory(UmbracoContext umbracoContext, IMapper mapper)
        {
            this.umbracoContext = umbracoContext;
            this.mapper = mapper;
        }

        public vmBlock_DataFormBuilder Success(string title, string message)
        {
            return new vmBlock_DataFormBuilder()
            {
                Title = title,
                IsSuccess = true,
                SuccessMessage = message
            };
        }

        public vmBlock_DataFormBuilder Form(string title, string submitButton, List<object> formFields = null)
        {
            if (formFields == null) formFields = new List<object>();

            return new vmBlock_DataFormBuilder()
            {
                Title = title,
                Fieldsets = new List<vmSub_DataFormBuilderFieldset>()
                    {
                        new vmSub_DataFormBuilderFieldset()
                        {
                            //Fields = formFields
                        }
                    },
                SubmitButtonText = submitButton
            };
        }

        public YuzuFormViewModel Create<C>(C c, List<object> formFields, Expression<Func<C, bool>> isSuccess, Expression<Func<C, Task<IActionResult>>> handler = null, string template = null)
            where C : Controller
        {
            var model = new YuzuFormViewModel();
            var content = umbracoContext.PublishedRequest.PublishedContent;

            if (isSuccess.Compile()(c))
            {
                model.Form = Success(content.Value<string>(CompletedTitle), content.Value<string>(CompletedBodyText));
                model.Form.ActionLinks = mapper.Map<List<vmBlock_DataLink>>(content.Value<IEnumerable<Link>>(CompletedActionLinks));
            }
            else
            {
                model.Form = Form(content.Value<string>(Title), content.Value<string>(SubmitButtonText), formFields);
                model.Form.ActionLinks = mapper.Map<List<vmBlock_DataLink>>(content.Value<IEnumerable<Link>>(ActionLinks));
            }

            if (handler != null)
                model.AddHandler(handler);
            if (!string.IsNullOrEmpty(template))
                model.Template = template;

            return model;
        }
    }
}
