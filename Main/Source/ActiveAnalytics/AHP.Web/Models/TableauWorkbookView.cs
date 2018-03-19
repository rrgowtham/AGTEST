using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Web.Models
{
    public class TableauWorkbookView
    {
        public string ViewId { get; set; }

        public string ViewName { get; set; }

        public string ContentUrl { get; set; }

        public int TotalViewCount { get; set; }
    }
}