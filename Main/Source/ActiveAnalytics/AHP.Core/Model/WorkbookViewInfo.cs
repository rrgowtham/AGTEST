using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    [Table("TABLEAUVIEWS")]
    public class WorkbookViewInfo
    {
        [Column("VIEWID")]
        [MaxLength(36,ErrorMessage = "View ID must be 36 character long")]
        [Key]
        public string ViewId { get; set; }

        [Column("ISDASHBOARD")]
        [DefaultValue("N")]
        public string IsDashboard { get; set; }

        [Column("VIEWNAME")]
        [MaxLength(100, ErrorMessage = "View ID must be 36 character long")]
        public string ViewName { get; set; }

        [Column("VIEWURL")]
        [MaxLength(400, ErrorMessage = "View ID must be 36 character long")]
        public string ViewUrl { get; set; }

        [Column("DISABLED")]
        [DefaultValue("N")]
        public string IsDisabled { get; set; }

        [Column("DESCRIPTION")]
        [MaxLength(400,ErrorMessage = "Description cannot be more than 400 characters")]
        public string Description { get; set; }
    }
}
