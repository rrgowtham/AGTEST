using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    [Table("INTERNALUSERS")]
    public class InternalUserTableauInfo
    {
        [Key]
        [Column("ACTIVEHEALTHID")]
        [MaxLength(50, ErrorMessage = "Active Health ID can only be 50 character long")]
        public string ActiveHealthUserId { get; set; }

        [Column("TABLEAUID")]
        [MaxLength(50,ErrorMessage = "Tableau ID can only be 50 character long")]
        public string TableauId { get; set; }

    }
}
