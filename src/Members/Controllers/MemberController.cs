﻿using System;
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
        protected IYuzuDeliveryMembersConfiguration config;

        public MemberController(IMemberService memberService, IYuzuDeliveryMembersConfiguration config)
        {
            this.memberService = memberService;
            this.config = config;
        }

        public ActionResult Login()
        {
            vmBlock_DataFormBuilder model = null;

            if (!Umbraco.MemberIsLoggedOn())
            {
                model = new vmBlock_DataFormBuilder()
                {
                    Title = "Login",
                    Fieldsets = new List<vmSub_DataFormBuilderFieldset>()
                    {
                        new vmSub_DataFormBuilderFieldset()
                        {
                            Fields = new List<object>()
                            {
                                new vmBlock_FormTextInput() { Name = "loginModel.Username", Type = "text", Placeholder = "Email Address", IsRequired = true },
                                new vmBlock_FormTextInput() { Name = "loginModel.Password", Type = "password", Placeholder = "Password", IsRequired = true },
                            }
                        }
                    },
                    SubmitButtonText = "Login",
                    ActionLinks = new List<vmBlock_DataLink>()
                    {
                        new vmBlock_DataLink() { Label = config.ForgottenPasswordLabel, Href = config.ForgottenPasswordUrl },
                        new vmBlock_DataLink() { Label = config.RegisterLabel, Href = config.RegisterUrl },
                    }
                };
            }
            else
                Response.Redirect(config.HomeUrl);

            return PartialView("login", model);
        }

        public ActionResult ForgottenPassword()
        {
            vmBlock_DataFormBuilder model = null;

            if (!Umbraco.MemberIsLoggedOn())
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Forgotten Password",
                        IsSuccess = true,
                        SuccessMessage = "We have emailed a link to change your password",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.BackLabel, Href = config.HomeUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Forgotten Password",
                        Fieldsets = new List<vmSub_DataFormBuilderFieldset>()
                        {
                            new vmSub_DataFormBuilderFieldset()
                            {
                                Fields = new List<object>()
                                {
                                    new vmBlock_FormTextInput() { Name = "forgottenPasswordVm.Email", Type = "text", Placeholder = "Email Address", IsRequired = true },
                                },
                            }
                        },
                        SubmitButtonText = "Submit",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.CancelLabel, Href = config.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(config.HomeUrl);

            return PartialView("ForgottenPassword", model);
        }

        public ActionResult Register()
        {
            vmBlock_DataFormBuilder model = null;

            if (!Umbraco.MemberIsLoggedOn())
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Register",
                        IsSuccess = true,
                        SuccessMessage = "Account created successfully.",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.BackLabel, Href = config.HomeUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Register",
                        Fieldsets = new List<vmSub_DataFormBuilderFieldset>()
                        {
                            new vmSub_DataFormBuilderFieldset()
                            {
                                Fields = new List<object>()
                                {
                                    new vmBlock_FormTextInput() { Name = "registerModel.Name", Type = "text", Placeholder = "Name", IsRequired = true },
                                    new vmBlock_FormTextInput() { Name = "registerModel.Email", Type = "text", Placeholder = "Email Address", IsRequired = true },
                                    new vmBlock_FormTextInput() { Name = "registerModel.Password", Type = "password", Placeholder = "Password", IsRequired = true }
                                },
                            }
                        },
                        SubmitButtonText = "Submit",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.CancelLabel, Href = config.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(config.HomeUrl);

            return PartialView("Register", model);
        }

        public ActionResult ChangeMember()
        {
            vmBlock_DataFormBuilder model = null;

            if (Umbraco.MemberIsLoggedOn())
            {
                if (TempData["FormSuccess"] != null && TempData["FormSuccess"].ToString() == "True")
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Change Member",
                        IsSuccess = true,
                        SuccessMessage = "User details have been updated",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.BackLabel, Href = config.HomeUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Change Member",
                        Fieldsets = new List<vmSub_DataFormBuilderFieldset>()
                        {
                            new vmSub_DataFormBuilderFieldset()
                            {
                                Fields = new List<object>()
                                {
                                    new vmBlock_FormTextInput() { Name = "changeMemberVm.Name", Type = "text", Placeholder = "Name", IsRequired = true },
                                    new vmBlock_FormTextInput() { Name = "changeMemberVm.Email", Type = "text", Placeholder = "Email Address", IsRequired = true }
                                },
                            }
                        },
                        SubmitButtonText = "Change",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.CancelLabel, Href = config.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(config.HomeUrl);

            return PartialView("ChangeMember", model);
        }

        public ActionResult ChangePassword()
        {
            vmBlock_DataFormBuilder model = null;
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
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Change Password",
                        IsSuccess = true,
                        SuccessMessage = "Password has been changed",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.BackLabel, Href = config.HomeUrl }
                        }
                    };
                }
                else if (member != null && memberModel.Value<DateTime>("ForgottenPasswordExpiry") < DateTime.Now)
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Change Password",
                        IsSuccess = true,
                        SuccessMessage = "Password change link has timed out, please try again",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.ForgottenPasswordLabel, Href = config.ForgottenPasswordUrl }
                        }
                    };
                }
                else
                {
                    model = new vmBlock_DataFormBuilder()
                    {
                        Title = "Change Password",
                        Fieldsets = new List<vmSub_DataFormBuilderFieldset>()
                        {
                            new vmSub_DataFormBuilderFieldset()
                            {
                                Fields = new List<object>()
                                {
                                    new vmBlock_FormTextInput() { Name = "changePasswordVm.Password", Type = "password", Placeholder = "Password", IsRequired = true },
                                    new vmBlock_FormTextInput() { Name = "changePasswordVm.ConfirmPassword", Type = "password", Placeholder = "Confirm Password", IsRequired = true }
                                },
                            }
                        },
                        SubmitButtonText = "Change",
                        ActionLinks = new List<vmBlock_DataLink>()
                        {
                            new vmBlock_DataLink() { Label = config.CancelLabel, Href = config.HomeUrl }
                        }
                    };
                }
            }
            else
                Response.Redirect(config.HomeUrl);

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