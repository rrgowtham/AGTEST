using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AHP.Web.ViewModel
{
    public class UserQuestionsViewmodel
    {

        [Required(AllowEmptyStrings = false,ErrorMessage = "Please select your primary security question")]
        public string PrimarySelectedQuestion { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "This is a required field.")]
        public string PrimaryProvidedAnswer { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "Please select your secondary security question")]
        public string SecondarySelectedQuestion { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "This is a required field.")]
        public string SecondaryProvidedAnswer { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select your third security question")]
        public string ThirdSelectedQuestion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "This is a required field.")]
        public string ThirdProvidedAnswer { get; set; }

        public List<string> SecurityQuestions { get; set; }  

    }
}