using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AHP.Web.ViewModel
{
    public class PasswordResetViewModel
    {

        [Required(ErrorMessage = "Please enter your current password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{10,}$",ErrorMessage = "Password complexity requirements not met")]      
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Please enter your new password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{10,}$", ErrorMessage = "Password complexity requirements not met")]
        public string NewPassword { get; set; }

        
        [Compare("NewPassword", ErrorMessage ="The new password and confirm password do not match")]
        [Required(ErrorMessage = "Please confirm your new password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{10,}$", ErrorMessage = "Password complexity requirements not met")]
        public string ConfirmPassword { get; set; }
        
    }
}