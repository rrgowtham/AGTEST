using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AHP.Web.ViewModel
{
    public class PersonalInfoViewModel
    {
        [Required(ErrorMessage = "** Username is required")]
        [Display(Name = "What is your username for Active Health?")]
        public string UserName { get; set; }

        
        [Required(ErrorMessage = "** Enter your DOB Month and year")]
        [Display(Name = "What is your birth month and year?")]
        
        public string MonthYear { get; set; }

        [Required(ErrorMessage = "** ZipCode is required")]
        [Display(Name = "What is your zip code?")]
        [MaxLength(5)]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "** Enter your favorite teacher")]
        [Display(Name = "Who was your favorite teacher?")]
        [MaxLength(25)]
        public string FavTeacher { get; set; }

        [Required(ErrorMessage = "** Enter your favorite place")]
        [Display(Name = "What was your favorite place to visit as a child?")]
        [MaxLength(25)]
        public string FavPlaceAsChild { get; set; }
    }
}