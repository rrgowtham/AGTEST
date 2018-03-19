using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Web.Models
{
    public class TableauSite
    {
       public string SiteId { get; set; }

        public string SiteName { get; set; }

        public string ContentUrl { get; set; }

        public string AdminMode { get; set; }

        public string UserQuota { get; set; }

        public string StorageQuota { get; set; }

        public string State { get; set; }

        public string StateReason { get; set; }
    }
}