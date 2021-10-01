#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Umbraco.Forms;
using YuzuDelivery.Umbraco.Core;
using Umbraco.Extensions;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common.Models;
using Umbraco.Cms.Web.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace YuzuDelivery.Umbraco.Members
{
    public class MemberFormController : SurfaceController
    {
        protected IMemberSignInManager signInManager;
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IYuzuDeliveryMembersConfig config;
        private readonly YuzuFormViewModelFactory viewmodelFactory;

        public MemberFormController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider, IMemberService memberService, IYuzuDeliveryMembersConfig config, YuzuFormViewModelFactory viewmodelFactory)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberService = memberService;
            this.config = config;
            this.viewmodelFactory = viewmodelFactory;
        }

        public ActionResult Login()
        {
            if (!_memberManager.IsLoggedIn())
            {
                var formFields = new List<object>()
                    {
                        new vmBlock_FormTextInput() { Name = "loginModel.Username", Type = "text", Placeholder = "Email Address", IsRequired = true },
                        new vmBlock_FormTextInput() { Name = "loginModel.Password", Type = "password", Placeholder = "Password", IsRequired = true },
                    };

                var model = viewmodelFactory.Create(this, formFields, x => false);
                model.AddHandler<UmbLoginController>(x => x.HandleLogin(null));
                model.AddAntiForgeryToken = false;

                return PartialView("YuzuForms", model);
            }
            else
            {
                Response.Redirect(config.HomeUrl);
                return null;
            }
        }

        public ActionResult ForgottenPassword()
        {
            if (!_memberManager.IsLoggedIn())
            {
                var formFields = new List<object>()
                    {
                        new vmBlock_FormTextInput() { Name = "forgottenPasswordVm.Email", Type = "text", Placeholder = "Email Address", IsRequired = true },
                    };

                var model = viewmodelFactory.Create(this, formFields, x => x.HasTempData("FormSuccess"));
                model.AddHandler<MemberHandlerController>(x => x.HandleForgottenPassword(null));

                return PartialView("YuzuForms", model);
            }
            else
            {
                Response.Redirect(config.HomeUrl);
                return null;
            }
        }

        public ActionResult Register()
        {
            if (!_memberManager.IsLoggedIn())
            {
                var formFields = new List<object>()
                    {
                        new vmBlock_FormTextInput() { Name = "registerModel.Name", Type = "text", Placeholder = "Name", IsRequired = true },
                        new vmBlock_FormTextInput() { Name = "registerModel.Email", Type = "text", Placeholder = "Email Address", IsRequired = true },
                        new vmBlock_FormTextInput() { Name = "registerModel.Password", Type = "password", Placeholder = "Password", IsRequired = true }
                    };


                var model = viewmodelFactory.Create(this, formFields, x => x.HasTempData("FormSuccess"));
                model.AddHandler<UmbRegisterController>(x => x.HandleRegisterMember(null));
                model.AddAntiForgeryToken = false;

                return PartialView("YuzuForms", model);
            }
            else
            {
                Response.Redirect(config.HomeUrl);
                return null;
            }
        }

        public async Task<IActionResult> ChangeMember()
        {
            if (!_memberManager.IsLoggedIn())
            {
                var memberModel = await _memberManager.GetCurrentMemberAsync();
                var member = _memberService.GetByKey(memberModel.Key);

                var formFields = new List<object>()
                {
                    new vmBlock_FormTextInput() { Name = "changeMemberVm.Name", Type = "text", Label = "Full Name", Value = member.Name, Placeholder = "Full Name", IsRequired = true },
                    new vmBlock_FormTextInput() { Name = "changeMemberVm.Email", Type = "text", Label = "Email Address", Value = member.Email, Placeholder = "Email Address", IsRequired = true },
                };

                var model = viewmodelFactory.Create(this, formFields, x => x.HasTempData("FormSuccess"), x => x.ChangeMember());
                model.AddHandler<MemberHandlerController>(x => x.HandleUpdate(null));

                return PartialView("YuzuForms", model);
            }
            else
            {
                Response.Redirect(config.HomeUrl);
                return null;
            }
        }

        public async Task<ActionResult> ChangePassword()
        {
            var model = new YuzuFormViewModel();
            IMember member = null;
            MemberIdentityUser memberModel = null;
            var userKey = Request.Query["id"];

            if (!string.IsNullOrEmpty(userKey))
            {
                memberModel = await _memberManager.GetCurrentMemberAsync();
                member = _memberService.GetByKey(memberModel.Key);
            }

            if (_memberManager.IsLoggedIn() || member != null)
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model.Form = viewmodelFactory.Success("Change Password", "Your password has been changed");
                }
                else if (member != null && member.GetValue<DateTime>("ForgottenPasswordExpiry") < DateTime.UtcNow)
                {
                    model.Form = viewmodelFactory.Success("Change Password", "Password change link has timed out, please try again");
                    model.Form.ActionLinks = new List<vmBlock_DataLink>()
                    {
                        new vmBlock_DataLink() { Label = config.ForgottenPasswordLabel, Href = config.ForgottenPasswordUrl }
                    };
                }
                else
                {
                    var formFields = new List<object>()
                    {
                        new vmBlock_FormTextInput() { Name = "changePasswordVm.Password", Type = "password", Label = "Password", Placeholder = "Password", IsRequired = true },
                        new vmBlock_FormTextInput() { Name = "changePasswordVm.ConfirmPassword", Type = "password", Label = "Confirm Password", Placeholder = "Confirm Password", IsRequired = true },
                        new vmBlock_FormHidden() { Name = "changePasswordVm.UserKey", Value = userKey }
                    };
                    model.Form = viewmodelFactory.Form("Change Password", "Change Password", formFields);
                    model.AddHandler<MemberHandlerController>(x => x.HandleChangePassword(null));
                }
            }
            else
                Response.Redirect(config.HomeUrl);

            return PartialView("YuzuForms", model);
        }

    }
}
#endif
