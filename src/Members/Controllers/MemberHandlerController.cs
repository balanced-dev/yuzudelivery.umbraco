using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;

namespace YuzuDelivery.Umbraco.Members
{
    public class MemberHandlerController : SurfaceController
    {
        protected IMemberSignInManager _signInManager;
        protected IYuzuDeliveryMembersConfig config;
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;

        public MemberHandlerController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IMemberSignInManager signInManager,
            IMemberManager memberManager,
            IMemberService memberService,
            IYuzuDeliveryMembersConfig config)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _signInManager = signInManager;
            _memberManager = memberManager;
            _memberService = memberService;
            this.config = config;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUmbracoFormRouteString]
        public IActionResult HandleForgottenPassword([Bind(Prefix = "forgottenPasswordVm")]ForgottenPasswordVm model)
        {
            if (ModelState.IsValid)
            {
                var member = _memberService.GetByEmail(model.Email);
                if (member != null)
                {
                    member.SetValue("forgottenPasswordExpiry", DateTime.UtcNow.AddHours(3));
                    _memberService.Save(member);

                    var changePasswordLink = string.Format("http://{0}{1}?id={2}",  this.HttpContext.GetServerVariable("Server_Name"), config.ChangePasswordUrl, member.Key);

                    config.ForgottenPasswordEmailAction(model.Email, member.Name, changePasswordLink);

                    TempData["FormSuccess"] = true;
                    return CurrentUmbracoPage();
                }
                else
                {
                    ModelState.AddModelError("passwordReminderVm", config.EmailNotFoundErrorMessage);
                }
            }
            return CurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUmbracoFormRouteString]
        public async Task<IActionResult> HandleChangePassword([Bind(Prefix = "changePasswordVm")]ChangePasswordVm model)
        {
            IMember member = null;
            var memberModel = await _memberManager.GetCurrentMemberAsync();

            if (memberModel == null)
            {
                member = _memberService.GetByKey(Guid.Parse(model.UserKey));
                if (member != null)
                {

                    if (memberModel == null || member.GetValue<DateTime>("forgottenPasswordExpiry") < DateTime.UtcNow)
                        member = null;
                }
            }
            else
            {
                member = _memberService.GetByKey(memberModel.Key);
            }

            if (member != null && ModelState.IsValid)
            {
                var newPassword = model.Password;

                //ToDo this probably won't work
                await _memberManager.ChangePasswordAsync(memberModel, memberModel.PasswordConfig, newPassword);
                _memberService.Save(member);

                TempData["FormSuccess"] = true;
                return CurrentUmbracoPage();
            }
            else
            {
                if (member == null)
                    ModelState.AddModelError("changePasswordVm", config.MemberNotFoundErrorMessage);
                return CurrentUmbracoPage();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUmbracoFormRouteString]
        public async Task<IActionResult> HandleUpdate([Bind(Prefix = "changeMemberVm")]CreateMemberVm model)
        {
            if (ModelState.IsValid)
            {
                var memberModel = await _memberManager.GetCurrentMemberAsync();
                var member = _memberService.GetByKey(memberModel.Key);

                if (member == null)
                {
                    ModelState.AddModelError("changeMemberVm", config.MemberNotFoundErrorMessage);
                    return CurrentUmbracoPage();
                }
                else
                {
                    member.Email = model.Email;
                    member.Name = model.Name;
                    member.Username = model.Email;

                    _memberService.Save(member);

                    TempData["FormSuccess"] = true;
                    return RedirectToCurrentUmbracoPage(Request.QueryString);
                }
            }
            else
            {
                return CurrentUmbracoPage();
            }

        }

    }
}
