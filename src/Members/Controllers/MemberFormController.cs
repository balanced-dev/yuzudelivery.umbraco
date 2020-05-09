using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Mvc;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using YuzuDelivery.Umbraco.Forms;
using YuzuDelivery.Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Controllers;

namespace YuzuDelivery.Umbraco.Members
{
    public class MemberFormController : SurfaceController
    {
        private readonly IMemberService memberService;
        private readonly IYuzuDeliveryMembersConfig config;
        private readonly YuzuFormViewModelFactory viewmodelFactory;

        public MemberFormController(IMemberService memberService, IYuzuDeliveryMembersConfig config, YuzuFormViewModelFactory viewmodelFactory)
        {
            this.memberService = memberService;
            this.config = config;
            this.viewmodelFactory = viewmodelFactory;
        }

        public ActionResult Login()
        {
            if (!Umbraco.MemberIsLoggedOn())
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
            if (!Umbraco.MemberIsLoggedOn())
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
            if (!Umbraco.MemberIsLoggedOn())
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

        public ActionResult ChangeMember()
        {
            if (Umbraco.MemberIsLoggedOn())
            {
                var memberModel = Members.GetCurrentMember();
                var member = memberService.GetById(memberModel.Id);

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

        public ActionResult ChangePassword()
        {
            var model = new YuzuFormViewModel();
            IMember member = null;
            IPublishedContent memberModel = null;
            var userKey = Request.QueryString["id"];

            if (!string.IsNullOrEmpty(userKey))
            {
                member = memberService.GetByKey(Guid.Parse(userKey));
                memberModel = Members.GetById(member.Id);
            }

            if (Umbraco.MemberIsLoggedOn() || member != null)
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model.Form = viewmodelFactory.Success("Change Password", "Your password has been changed");
                }
                else if (member != null && memberModel.Value<DateTime>("ForgottenPasswordExpiry") < DateTime.UtcNow)
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
