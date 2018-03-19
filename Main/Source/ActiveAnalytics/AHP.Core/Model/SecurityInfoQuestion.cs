using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    [Table("INFOQUESTIONS")]
    public class SecurityInfoQuestion
    {

        [Key]
        [MaxLength(1000,ErrorMessage = "Security question cannot exceed 1000 characters")]
        [Column("SECURITYQUESTION")]
        public string SecurityQuestion { get; set; }
    }
}
