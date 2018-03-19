using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    /// <summary>
    /// Stores an Authenticated LDAP User's information.
    /// Once this object is constructed, it cannot be modified.
    /// </summary>
    public class LDAPUser
    {
        public string AccountName { get; set; }
        public LDAPAccessStatus LDAPAccessStatus { get; set; }
        public string LDAPAccessStatusMessage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        
    }
}
