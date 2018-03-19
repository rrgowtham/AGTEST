using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AHP.Web.Models
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "** Email Required")]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}