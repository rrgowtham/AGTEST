using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AHP.Web.ViewModel
{
    public class TableauWorkbookViewModel
    {

        [MaxLength(36,ErrorMessage = "View id cannot be greater than 36 characters")]
        public string ViewId { get; set; }
        
        [System.ComponentModel.DefaultValue("N")]
        public string IsDashboard { get; set; }

        [RegularExpression("^[a-zA-Z-0-9]+", ErrorMessage = "Only alphanumeric and - allowed in view name")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "View name is required")]
        [MaxLength(100, ErrorMessage = "View name cannot be more than 100 characters")]
        public string ViewName { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "View Url is required")]
        [MaxLength(400,ErrorMessage = "View Url cannot be more than 400 characters")]
        public string ViewUrl { get; set; }

        [System.ComponentModel.DefaultValue("N")]
        public string Disabled { get; set; }

        [MaxLength(400,ErrorMessage = "Description cannot be more than 400 characters")]
        public string Description { get; set; }

        public string ShortName { get; set; }

        public string ShortDescription { get; set; }

    }
}