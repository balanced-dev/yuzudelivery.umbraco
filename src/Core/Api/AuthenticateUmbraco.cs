using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Security;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{

    public class AuthenticateUmbraco : IAuthoriseApi
    {

        public bool Authorise(string username, string password, string role)
        {
            var providerKey = "UsersMembershipProvider";
            var provider = Membership.Providers[providerKey];

            if (provider == null || !provider.ValidateUser(username, password))
                return false;
            var user = Current.Services.UserService.GetByUsername(username);

            if (!user.IsApproved || user.IsLockedOut || !user.AllowedSections.Contains(role))
                return false;

            return true;
        }

    }
}
