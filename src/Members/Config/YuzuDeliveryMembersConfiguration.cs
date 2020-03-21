using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models;

namespace YuzuDelivery.Umbraco.Members
{
    public class YuzuDeliveryMembersConfiguration : IYuzuDeliveryMembersConfig
    {
        public YuzuDeliveryMembersConfiguration()
        {
            ForgottenPasswordLabel = "Forgotten Password?";
            RegisterLabel = "Register";
            BackLabel = "Back";
            CancelLabel = "Cancel";

            EmailNotFoundErrorMessage = "Email address not found";
            MemberNotFoundErrorMessage = "Member not found";

            ForgottenPasswordEmailAction = (string email, string name, string changePasswordLink) => { };
        }

        public string HomeUrl { get; set; }
        public string MembersHomeUrl { get; set; }
        public string ForgottenPasswordUrl { get; set; }
        public string LoginUrl { get; set; }
        public string RegisterUrl { get; set; }
        public string ChangePasswordUrl { get; set; }

        public string ForgottenPasswordLabel { get; set; }
        public string RegisterLabel { get; set; }
        public string CancelLabel { get; set; }
        public string BackLabel { get; set; }

        public string EmailNotFoundErrorMessage { get; set; }
        public string MemberNotFoundErrorMessage { get; set; }

        public Action<string, string, string> ForgottenPasswordEmailAction { get; set; }
    }
}
