using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Forms;

#if NETCOREAPP
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Skybrud.Umbraco.GridData.Models;
#else
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.Models;
using Skybrud.Umbraco.GridData;
#endif

namespace YuzuDelivery.Umbraco.Grid
{
    public class YuzuGridFormViewModelFactory : YuzuFormViewModelFactory
    {
        public YuzuGridFormViewModelFactory(UmbracoContext umbracoContext, IMapper mapper)
            :base(umbracoContext, mapper)
        {        }


#if NETCOREAPP
        public YuzuFormViewModel CreateForGridItem<C, M>(C c, List<object> formFields, Expression<Func<C, bool>> isSuccess, Expression<Func<C, Task<IActionResult>>> getContentFromProperty, Expression<Func<M, GridDataModel>> gridProperty, Expression<Func<C, Task<IActionResult>>> handler = null, string template = null)
#else
        public YuzuFormViewModel CreateForGridItem<C, M>(C c, List<object> formFields, Expression<Func<C, bool>> isSuccess, Expression<Func<C, ActionResult>> getContentFromProperty, Expression<Func<M, GridDataModel>> gridProperty, Expression<Func<C, ActionResult>> handler = null, string template = null)
#endif
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
