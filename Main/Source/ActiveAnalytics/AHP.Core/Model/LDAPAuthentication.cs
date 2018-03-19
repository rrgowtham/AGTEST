using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    public class LDAPAuthentication
    {
        public string DomainName { get; set; }
        public string LDAPUrlPath { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FilterAttribute { get; set; }
    }
}
