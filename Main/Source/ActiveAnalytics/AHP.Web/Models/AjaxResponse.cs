using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Web.Models
{
    public class AjaxResponse
    {
        public bool Success { get; set; }

        public string HomeUrl { get; set; }

        public string Message { get; set; }
    }
}