using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Service;
using AHP.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AHP.Web.Api.Controllers
{
    public class PasswordResetController : ApiController
    {

        #region -- Members --

        private readonly IAuthenticationService _authenticationService;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        #endregion

        #region -- Constructors --

        public PasswordResetController(IAuthenticationService authenticationService,IActiveAnalyticsLogger logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _logMessages = new StringBuilder();            
        }

        #endregion

        #region -- Api Methods --
        [System.Web.Mvc.HttpPost]
        public BOPWReset ResetPasswordAndRedirect(BOPasswordReset passwordReset)
        {
            BOPWReset retValue = new BOPWReset { Message = string.Empty };
            try
            {
                _logMessages.AppendFormat("Performing password reset for user {0}.",passwordReset.Username);
                retValue.Message = _authenticationService.ResetPasswordAndRedirect(passwordReset);
            }
            catch (Exception ex)
            {
                _logMessages.AppendFormat("An Error occurred during password reset Exception information {0}.", ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessages.ToString());
            return retValue;
        } 
        #endregion

    }
}
