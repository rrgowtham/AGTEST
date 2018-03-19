using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
   public class BOPasswordReset
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string SAPLoginToken { get; set; }

        public bool AccountLocked { get; set; }
    }
}
