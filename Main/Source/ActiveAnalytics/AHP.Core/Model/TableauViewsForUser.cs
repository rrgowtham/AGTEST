using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    [Table("VIEWSFORUSER")]
    public class TableauViewsForUser
    {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("USERNAME",Order = 1)]
        [MaxLength(128, ErrorMessage = "Username cannot be more than 128 characters")]
        public string Username { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("VIEWID", Order = 2)]
        [MaxLength(36, ErrorMessage = "View id cannot be more than 36 characters")]
        public string ViewId { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("USERTYPE",Order = 3)]
        [MaxLength(20,ErrorMessage = "User type cannot be more than 20 characters")]
        public string UserType { get; set; }

    }
}
