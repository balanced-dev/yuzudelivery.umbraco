using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace YuzuDelivery.Umbraco.Members
{
    public class ChangePasswordVm
    {
        public string UserKey { get; set; }

        [Required(ErrorMessage = "Password is a required field")]
        [MinLength(10, ErrorMessage = "The password must be at least 10 characters")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and confirmation must match")]
        [Required(ErrorMessage = "Password confirmation is a required field")]
        public string ConfirmPassword { get; set; }
    }
}
