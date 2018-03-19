using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AHP.Web.ViewModel
{
    public class InternalUserViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Active Health employee ID is required")]
        [MaxLength(50, ErrorMessage = "Only 50 characters allowed")]
        [RegularExpression("^[a-z-A-Z]+$", ErrorMessage = "Only alphabets allowed in active health id")]
        public string ActiveHealthId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tableau account ID is required")]
        [MaxLength(50,ErrorMessage = "Only 50 characters allowed")]
        [RegularExpression("^[a-z-A-Z0-9]+$", ErrorMessage = "Only alpha numeric allowed in tableau id")]
        public string TableauId { get; set; }

    }
}