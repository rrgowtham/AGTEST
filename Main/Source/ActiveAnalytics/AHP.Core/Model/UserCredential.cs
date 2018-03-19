using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    public class UserCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsInternalUser { get; set; }

    }
}
