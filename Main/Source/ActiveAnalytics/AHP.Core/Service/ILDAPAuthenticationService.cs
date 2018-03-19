using AHP.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Service
{
    public interface ILDAPAuthenticationService
    {
        LDAPUser IsAuthenticated(LDAPAuthentication ldapAuth);
        //string GetGroups(LDAPAuthentication ldapAuth);
    }
}
