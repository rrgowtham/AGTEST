using AHP.Core.Model;
using AHP.Core.Repository;
using AHP.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Service
{
    public class LDAPAuthenticationService : ILDAPAuthenticationService
    {
        private readonly ILDAPAuthenticationRepository _ldapAuthenticationRepository;

        public LDAPAuthenticationService(ILDAPAuthenticationRepository ldapAuthenticationRepository)
        {
            _ldapAuthenticationRepository = ldapAuthenticationRepository;
        }

        public LDAPUser IsAuthenticated(LDAPAuthentication ldapAuth)
        {
            return _ldapAuthenticationRepository.AuthenticateUser(ldapAuth);
        }

        //public string GetGroups(LDAPAuthentication ldapAuth)
        //{
        //    return _ldapAuthenticationRepository.GetGroups(ldapAuth);
        //}
    }
}
