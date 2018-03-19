using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    [Table("PERSONALINFOQUESTIONS")]
    public class UserSecurityAnswer
    {
        [Key]
        [Column("ID")]
        [MaxLength(36,ErrorMessage = "ID cannot be larger than 48 character")]
        public string Guid { get; set; }

        [Column("USERNAME")]
        [MaxLength(128,ErrorMessage = "Username cannot be larger than 128 characters")]
        public string Username { get; set; }

        [Column("SECURITYQUESTION")]
        [MaxLength(1000,ErrorMessage = "Security question cannot be larger than 1000 characters")]
        public string SecurityQuestion { get; set; }

        [Column("ANSWER")]
        [MaxLength(1000,ErrorMessage = "Answer to security question cannot exceed 1000 characters")]
        public string Answer { get; set; }

        [Column("SALT")]
        [MaxLength(256, ErrorMessage = "Salt for answer cannot exceed 256 characters")]
        public string Salt { get; set; }

    }
}
