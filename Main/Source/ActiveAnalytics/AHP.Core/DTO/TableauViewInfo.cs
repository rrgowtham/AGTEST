using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.DTO
{
    public class TableauViewInfo
    {
        public string ViewId { get; set; }

        public string IsDashboard { get; set; }

        public string ViewName { get; set; }

        public string ViewUrl { get; set; }

        public string Disabled { get; set; }

        public string Description { get; set; }

    }
}
