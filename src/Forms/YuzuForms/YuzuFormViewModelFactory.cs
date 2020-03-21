using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Umbraco.Forms;
using System.Linq.Expressions;
using System.Web.Mvc;
using Umbraco.Web;
using AutoMapper;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Grid;
using Umbraco.Web.Models;
using Skybrud.Umbraco.GridData;

namespace YuzuDelivery.Umbraco.Forms
{
    public class YuzuFormViewModelFactory
    {
        private readonly IMapper mapper;
        private readonly UmbracoContext umbracoContext;

        private const string CompletedTitle = "completedTitle";
        private const string CompletedBodyText = "completedBodyText";
        private const string Title = "title";
        private const string SubmitButtonText = "submitButtonText";
        private const string ActionLinks = "actionLinks";
        private const string CompletedActionLinks = "completedActionLinks";

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
                            Fields = formFields
                        }
                    },
                SubmitButtonText = submitButton
            };
        }

        public YuzuFormViewModel Create<C>(C c, List<object> formFields, Expression<Func<C, bool>> isSuccess, Expression<Func<C, ActionResult>> handler = null, string template = null)
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

        public YuzuFormViewModel CreateForGridItem<C, M>(C c, List<object> formFields, Expression<Func<C, bool>> isSuccess, Expression<Func<C, ActionResult>> getContentFromProperty, Expression<Func<M, GridDataModel>> gridProperty, Expression<Func<C, ActionResult>> handler = null, string template = null)
            where C : Controller
        {
            var model = new YuzuFormViewModel();
            var content = umbracoContext.GetGridFormForEndpoint(getContentFromProperty, gridProperty);

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

            if(handler != null)
                model.AddHandler(handler);
            if(!string.IsNullOrEmpty(template))
                model.Template = template;

            return model;
        }
    }
}
