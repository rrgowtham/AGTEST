using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Service;
using System;
using System.Configuration;
using System.Text;
using System.Web.Http;

namespace AHP.Web.Api.Controllers
{
    public class LDAPAccountController : ApiBaseController
    {
        #region -- Members --

        private readonly ILDAPAuthenticationService _ldapAuthenticationService;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        #endregion

        #region -- Constructors --

        public LDAPAccountController(ILDAPAuthenticationService ldapAuthenticationService,IActiveAnalyticsLogger logger)
        {
            _ldapAuthenticationService = ldapAuthenticationService;
            _logger = logger;
            _logMessages = new StringBuilder();
        }

        #endregion

        #region  -- Api Methods --
        
        /// <summary>
        /// Returns "true" for success and an error message if it is not success.
        /// </summary>
        /// <param name="userCredential"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        //TODO: Rename method accc to what it does
        public LDAPUser AuthenticateUserWithLdap(UserCredential userCredential)
        {
            
            var ldapAuthentication = new LDAPAuthentication()
            {
                DomainName = ConfigurationManager.AppSettings["Domain"],
                UserName = userCredential.Username,
                Password = userCredential.Password
            };
            _logMessages.AppendFormat("Performing LDAP logon for user {0} with domain {1}",ldapAuthentication.UserName,ldapAuthentication.DomainName);
            LDAPUser response = null;
            try
            {
                response = _ldapAuthenticationService.IsAuthenticated(ldapAuthentication);
                _logMessages.Append("Successfully invoked ldap service. Response received");
            }
            catch (Exception ex)
            {
                _logMessages.AppendFormat("An Error occurred invoking ldap authentication. Exception details {0}",ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessages.ToString());
            return response;
        } 
        #endregion
    }
}
