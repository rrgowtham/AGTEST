using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AHP.Web.Models
{
    public class User
    {
        [Required(ErrorMessage = "** UserName Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "** Password Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Internal User")]
        public bool IsInternalUser { get; set; }
        
    }
}