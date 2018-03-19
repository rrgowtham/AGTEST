using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{

    [Table("EVENTAUDIT")]
    public class EventAudit
    {
        [Key]
        [Column("EVENTID")]
        [MaxLength(36,ErrorMessage ="Maximum length for event id is 36")]
        public string EventId { get; set; }

        [Column("EVENTDATE")]
        public DateTime EventDate { get; set; }

        [Column("EVENTTYPE")]
        [MaxLength(100, ErrorMessage = "Maximum length for event type is 100")]
        public string EventType { get; set; }

        [Column("USERNAME")]
        [MaxLength(128, ErrorMessage = "Maximum length for username is 128")]
        public string Username { get; set; }

        [Column("DESCRIPTION")]
        [MaxLength(1000, ErrorMessage = "Maximum length for description is 1000")]
        public string Description { get; set; }

    }
}
