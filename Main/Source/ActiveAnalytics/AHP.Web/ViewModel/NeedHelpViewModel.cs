using System;
using System.ComponentModel.DataAnnotations;

namespace AHP.Web.ViewModel
{
    public class CustomerViewModel
    {
        [Required(ErrorMessage = "**Issue is Required!")]
        public int SelectedIssueId { get; set; }
        [Required(ErrorMessage = " ")]
        [EmailAddress(ErrorMessage = " Please enter valid Email!")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "** First Name is Required!")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "** Last Name is Required!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "** Company Name is Required!")]
        [Display(Name = "Company")]
        public string Company { get; set; }

        [DataType(DataType.PhoneNumber)]        
        [Required(ErrorMessage=" ")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "** Invalid Phone Number")]        
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        //private string _issueDescription = "Please describe your issue!";
        [Required(ErrorMessage = " ")]
        [MaxLength(2000, ErrorMessage = "Max length allowed is 2000 characters")]
        public string IssueDescription { get; set; }
    }
}