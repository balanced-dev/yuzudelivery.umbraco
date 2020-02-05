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

namespace YuzuDelivery.Umbraco.Members
{
    public class MemberController : SurfaceController
    {
        protected IMemberService memberService;

        public MemberController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        public ActionResult Login()
        {
            vmBlock_AccountForm model = null;

            if (!Umbraco.MemberIsLoggedOn())
            {
                model = new vmBlock_AccountForm()
                {
                    Title = "Login",
                    Fields = new List<object>()
                    {
                        new vmBlock_FormTextInput() { Name = "loginModel.Username", Type = "text", Placeholder = "Email Address", IsRequired = true },
                        new vmBlock_FormTextInput() { Name = "loginModel.Password", Type = "password", Placeholder = "Password", IsRequired = true },
                    },
                    SubmitButtonText = "Login",
                    ActionLinks = new List<vmBlock_DataLink>()
                    {
                        new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.ForgottenPasswordLabel, Href = YuzuDeliveryMembers.Configuration.ForgottenPasswordUrl },
                        new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.RegisterLabel, Href = YuzuDeliveryMembers.Configuration.RegisterUrl },
                    }
                };
            }
            else
                Response.Redirect(YuzuDeliveryMembers.Configuration.HomeUrl);

            return PartialView("login", model);
        }

        public ActionResult ForgottenPassword()
        {
            vmBlock_AccountForm model = null;

            if (!Umbraco.MemberIsLoggedOn())
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Forgotten Password",
                        IsSubmitted = true,
                        SuccessMessage = "We have emailed a link to change your password",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.BackLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Forgotten Password",
                        Fields = new List<object>()
                        {
                            new vmBlock_FormTextInput() { Name = "forgottenPasswordVm.Email", Type = "text", Placeholder = "Email Address", IsRequired = true },
                        },
                        SubmitButtonText = "Submit",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.CancelLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(YuzuDeliveryMembers.Configuration.HomeUrl);

            return PartialView("ForgottenPassword", model);
        }

        public ActionResult Register()
        {
            vmBlock_AccountForm model = null;

            if (!Umbraco.MemberIsLoggedOn())
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Register",
                        IsSubmitted = true,
                        SuccessMessage = "Account created successfully.",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.BackLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Register",
                        Fields = new List<object>()
                        {
                            new vmBlock_FormTextInput() { Name = "registerModel.Name", Type = "text", Placeholder = "Name", IsRequired = true },
                            new vmBlock_FormTextInput() { Name = "registerModel.Email", Type = "text", Placeholder = "Email Address", IsRequired = true },
                            new vmBlock_FormTextInput() { Name = "registerModel.Password", Type = "password", Placeholder = "Password", IsRequired = true }
                        },
                        SubmitButtonText = "Submit",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.CancelLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(YuzuDeliveryMembers.Configuration.HomeUrl);

            return PartialView("Register", model);
        }

        public ActionResult ChangeMember()
        {
            vmBlock_AccountForm model = null;

            if (Umbraco.MemberIsLoggedOn())
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Change Member",
                        IsSubmitted = true,
                        SuccessMessage = "User details have been updated",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.BackLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Change Member",
                        Fields = new List<object>()
                        {
                            new vmBlock_FormTextInput() { Name = "changeMemberVm.Name", Type = "text", Placeholder = "Name", IsRequired = true },
                            new vmBlock_FormTextInput() { Name = "changeMemberVm.Email", Type = "text", Placeholder = "Email Address", IsRequired = true }
                        },
                        SubmitButtonText = "Change",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.CancelLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(YuzuDeliveryMembers.Configuration.HomeUrl);

            return PartialView("ChangeMember", model);
        }

        public ActionResult ChangePassword()
        {
            vmBlock_AccountForm model = null;
            IMember member = null;
            IPublishedContent memberModel = null;

            if (Request.QueryString["id"] != null)
            {
                member = memberService.GetByKey(Guid.Parse(Request.QueryString["id"]));
                memberModel = Members.GetById(member.Id);
            }

            if (Umbraco.MemberIsLoggedOn() || member != null)
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Change Password",
                        IsSubmitted = true,
                        SuccessMessage = "Password has been changed",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.BackLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
                else if (member != null && memberModel.Value<DateTime>("ForgottenPasswordExpiry") < DateTime.Now)
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Change Password",
                        IsSubmitted = true,
                        SuccessMessage = "Password change link has timed out, please try again",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.ForgottenPasswordLabel, Href = YuzuDeliveryMembers.Configuration.ForgottenPasswordUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_AccountForm()
                    {
                        Title = "Change Password",
                        Fields = new List<object>()
                        {
                            new vmBlock_FormTextInput() { Name = "changePasswordVm.Password", Type = "password", Placeholder = "Password", IsRequired = true },
                            new vmBlock_FormTextInput() { Name = "changePasswordVm.ConfirmPassword", Type = "password", Placeholder = "Confirm Password", IsRequired = true }
                        },
                        SubmitButtonText = "Change",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = YuzuDeliveryMembers.Configuration.CancelLabel, Href = YuzuDeliveryMembers.Configuration.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(YuzuDeliveryMembers.Configuration.HomeUrl);

            return PartialView("ChangePassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUmbracoFormRouteString]
        public ActionResult HandleForgottenPassword([Bind(Prefix = "forgottenPasswordVm")]ForgottenPasswordVm model)
        {
            if(ModelState.IsValid)
            {
                var member = memberService.GetByEmail(model.Email);
                if (member != null)
                {
                    member.SetValue("forgottenPasswordExpiry", DateTime.Now.AddHours(3));
                    memberService.Save(member);

                    var changePasswordLink = string.Format("http://{0}{1}?id={2}", Request.ServerVariables["Server_Name"], YuzuDeliveryMembers.Configuration.ChangePasswordUrl, member.Key);

                    YuzuDeliveryMembers.Configuration.ForgottenPasswordEmailAction(model.Email, member.Name, changePasswordLink);

                    TempData["FormSuccess"] = true;
                    return CurrentUmbracoPage();
                }
                else
                {
                    ModelState.AddModelError("passwordReminderVm", YuzuDeliveryMembers.Configuration.EmailNotFoundErrorMessage);
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
                if(member != null)
                {
                    memberModel = Members.GetById(member.Id);
                    if (memberModel == null || memberModel.Value<DateTime>("forgottenPasswordExpiry") < DateTime.Now)
                        member = null;
                }
            }
            else {
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
                    ModelState.AddModelError("changePasswordVm", YuzuDeliveryMembers.Configuration.MemberNotFoundErrorMessage);
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
                    ModelState.AddModelError("changeMemberVm", YuzuDeliveryMembers.Configuration.MemberNotFoundErrorMessage);
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
