using AHP.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Repository
{
    public interface ILDAPAuthenticationRepository
    {
        LDAPUser AuthenticateUser(LDAPAuthentication ldapAuth);
        //string GetGroups(LDAPAuthentication ldapAuth);
    }
}
