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
    public class PersonalInfoQuestion
    {
        [Column("USERNAME")]
        [Key]
        public string UserName { get; set; }

        [Column("MONTHYEAR")]
        [MaxLength(7)]
        public string MonthYear { get; set; }

        [Column("ZIPCODE")]
        [MaxLength(25)]
        public string ZipCode { get; set; }

        [Column("FAVTEACHER")]
        [MaxLength(25)]
        public string FavTeacher { get; set; }

        [Column("FAVPLACEASCHILD")]
        [MaxLength(25)]
        public string FavPlaceAsChild { get; set; }

    }
}
