using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    public class ReportCategory
    {
        public string InfoStoreId { get; set; }
        public string Cuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<ReportCategory> ParentCategories { get; set; }
        public List<ReportCategory> SubCategories { get; set; }
        public List<Report> Reports { get; set; }

        public ReportCategory()
        {
            this.ParentCategories = new List<ReportCategory>();
            this.SubCategories = new List<ReportCategory>();
            this.Reports = new List<Report>();
        }
    }
    
}
