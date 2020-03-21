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
    public class MemberFormController : SurfaceController
    {
        protected IMemberService memberService;
        protected IYuzuDeliveryMembersConfig config;

        public MemberFormController(IMemberService memberService, IYuzuDeliveryMembersConfig config)
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

    }
}
