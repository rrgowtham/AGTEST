using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AHP.Web.ViewModel
{
    public class SecurityQuestionsViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select your first security question")]
        public string PrimarySelectedQuestion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide answer to your first selected question.")]
        public string PrimaryProvidedAnswer { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select your second security question")]
        public string SecondarySelectedQuestion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide answer to your second selected question")]
        public string SecondaryProvidedAnswer { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select your third security question")]
        public string ThirdSelectedQuestion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide answer to your third selected question")]
        public string ThirdProvidedAnswer { get; set; }

        public List<string> SecurityQuestions { get; set; }        
    }
}