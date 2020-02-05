using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace YuzuDelivery.Umbraco.Members
{
    public interface IYuzuDeliveryMembersConfiguration
    {
        string HomeUrl { get; set; }
        string ForgottenPasswordUrl { get; set; }
        string LoginUrl { get; set; }
        string RegisterUrl { get; set; }
        string ChangePasswordUrl { get; set; }

        string ForgottenPasswordLabel { get; set; }
        string RegisterLabel { get; set; }
        string CancelLabel { get; set; }
        string BackLabel { get; set; }

        string EmailNotFoundErrorMessage { get; set; }
        string MemberNotFoundErrorMessage { get; set; }

        Action<string, string, string> ForgottenPasswordEmailAction { get; set; }
    }
}