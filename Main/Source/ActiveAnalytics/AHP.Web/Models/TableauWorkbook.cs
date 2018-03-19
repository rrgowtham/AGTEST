using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Web.Models
{
    public class TableauWorkbook
    {
        public string Id { get; set; }

        public string WorkbookName { get; set; }

        public string ShowTabs { get; set; }

        public string Size { get; set; }

        public string CreatedAt { get; set; }

        public string UpdatedAt { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string OwnerId { get; set; }

        public List<string> Tags { get; set; }
    }
}