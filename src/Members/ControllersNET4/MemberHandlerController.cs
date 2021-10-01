#if NET472_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Controllers;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using System.Web.Mvc;

namespace YuzuDelivery.Umbraco.Members
{
    public class MemberHandlerController : SurfaceController
    {
        protected IMemberService memberService;
        protected IYuzuDeliveryMembersConfig config;

        public MemberHandlerController(IMemberService memberService, IYuzuDeliveryMembersConfig config)
        {
            this.memberService = memberService;
            this.config = config;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUmbracoFormRouteString]
        public ActionResult HandleForgottenPassword([Bind(Prefix = "forgottenPasswordVm")]ForgottenPasswordVm model)
        {
            if (ModelState.IsValid)
            {
                var member = memberService.GetByEmail(model.Email);
                if (member != null)
                {
                    member.SetValue("forgottenPasswordExpiry", DateTime.UtcNow.AddHours(3));
                    memberService.Save(member);

                    var changePasswordLink = string.Format("http://{0}{1}?id={2}", Request.ServerVariables["Server_Name"], config.ChangePasswordUrl, member.Key);

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
        public ActionResult HandleChangePassword([Bind(Prefix = "changePasswordVm")]ChangePasswordVm model)
        {
            IMember member = null;
            var memberModel = Members.GetCurrentMember();

            if (memberModel == null)
            {
                member = memberService.GetByKey(Guid.Parse(model.UserKey));
                if (member != null)
                {
                    memberModel = Members.GetById(member.Id);
                    if (memberModel == null || memberModel.Value<DateTime>("forgottenPasswordExpiry") < DateTime.UtcNow)
                        member = null;
                }
            }
            else
            {
                member = memberService.GetById(memberModel.Id);
            }

            if (member != null && ModelState.IsValid)
            {
                var newPassword = model.Password;

                memberService.SavePassword(member, newPassword);
                memberService.Save(member);

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
        public ActionResult HandleUpdate([Bind(Prefix = "changeMemberVm")]CreateMemberVm model)
        {
            if (ModelState.IsValid)
            {
                var memberModel = Members.GetCurrentMember();
                var member = memberService.GetById(memberModel.Id);

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

                    memberService.Save(member);

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
#endif