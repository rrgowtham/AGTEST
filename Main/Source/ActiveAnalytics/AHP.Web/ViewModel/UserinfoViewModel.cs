using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AHP.Web.ViewModel
{
    public class UserinfoViewModel
    {

        [MaxLength(128,ErrorMessage = "Username must be within 128 characters")]
        [Required(AllowEmptyStrings =false,ErrorMessage = "Username is required")]
        [RegularExpression(@"^\w+$",ErrorMessage = "Only alpha-numeric characters and underscores are allowed for username")]
        public string Username { get; set; }

        [MaxLength(128, ErrorMessage = "First name must be within 128 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+", ErrorMessage = "Only alphabets and whitespace are allowed in first name")]
        public string Firstname { get; set; }

        [MaxLength(128, ErrorMessage = "Last name must be within 128 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+", ErrorMessage = "Only alphabets and whitespace are allowed in last name")]
        public string Lastname { get; set; }

        [MaxLength(1000, ErrorMessage = "Supplier id must be within 1000 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Supplier id is required")]
        [RegularExpression(@"^[0-9]{2,6}(,[0-9]{2,6})*$", ErrorMessage = "Supplier id must be 2 to 6 digit comma separated")]
        public string SupplierId { get; set; }

        [MaxLength(128, ErrorMessage = "Email must be within 1000 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [MaxLength(50,ErrorMessage = "Role cannot be bigger than 50 characters")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "Role is required")]        
        public string Role { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "Company is required")]
        [MaxLength(100,ErrorMessage = "Company must be within 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+", ErrorMessage = "Only alphabets and whitespace are allowed in company name")]
        public string Company { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "Birth month is required")]
        [RegularExpression(@"^(0?[1-9]|1[012])$", ErrorMessage = "Enter birth month in format MM")]        
        public string BirthMonth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Birth year is required")]        
        [RegularExpression(@"^(\d){4}$", ErrorMessage = "Enter birth year in format YYYY")]
        public string BirthYear { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Zipcode is required")]
        [RegularExpression(@"^\b\d{5}\b(?:[- ]{1}\d{4})?$", ErrorMessage = "Enter zip code in format ##### or Zip+4")]
        public string ZipCode { get; set; }

        public bool ChangePasswordOnLogon { get; set; }

        public bool IsLocked { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailActive { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public string LastLogonDate { get; set; }

    }
}