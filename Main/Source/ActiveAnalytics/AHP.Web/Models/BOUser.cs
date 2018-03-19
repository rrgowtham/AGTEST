using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Web.Models
{
    public class BOUser
    {
        public string LoginToken { get; set; }
        public bool MustChangePassword { get; set; }
        public string BOSessionId { get; set; }

        public string BOSerializedSessionId { get; set; }
    }
}
