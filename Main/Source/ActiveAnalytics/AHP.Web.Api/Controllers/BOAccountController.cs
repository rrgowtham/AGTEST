using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Service;
using AHP.Web.Api.Models;
using System.Configuration;
using System.Text;
using System.Web.Http;

namespace AHP.Web.Api.Controllers
{
    public class BOAccountController : ApiBaseController
    {
        #region -- Members --

        private readonly IAuthenticationService _authenticationService;
        private readonly IActiveAnalyticsLogger _logger;
        private readonly StringBuilder _errorMessageBuilder;

        #endregion

        #region -- Constructors --
        public BOAccountController(IAuthenticationService authenticationService,IActiveAnalyticsLogger logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _errorMessageBuilder = new StringBuilder();
        }
        #endregion

        #region -- Api Methods --                

        [System.Web.Http.HttpPost]
        public BOUser AuthenticateWithSAPAndGetToken(UserCredential userCredential)
        {

            string baseUrl = ConfigurationManager.AppSettings["BOBaseUrl"];
            _errorMessageBuilder.AppendFormat("User Login Attempt with BO Server {0}.",baseUrl);
            string userAuth = (userCredential.IsInternalUser) ? ConfigurationManager.AppSettings["BOInternalAuth"] : ConfigurationManager.AppSettings["BOExternalAuth"];
            _errorMessageBuilder.AppendFormat("User Login Type {0}. Login details {1}.",userCredential.IsInternalUser ? "Internal":"External",userAuth);
            BOAuthentication boAuthmodel = new BOAuthentication()
            {
                UserName = userCredential.Username,
                Password = userCredential.Password,
                URI = baseUrl,
                UserAuth = userAuth
            };


            try
            {
                _errorMessageBuilder.Append("Invoking Authentication Service.");
                var authResult = _authenticationService.AuthenticateUserAndGetToken(boAuthmodel);
                if (authResult.StatusCode == 0)
                {
                    _errorMessageBuilder.Append("User successfully logged in.");
                    BOUser retValue = new BOUser
                    {
                        LoginToken = authResult.LogonToken,
                        MustChangePassword = authResult.MustChangePassword,
                        BOSessionId = authResult.BOSesssionID,
                        BOSerializedSessionId = authResult.BOSerializedSessionId
                    };
                    _errorMessageBuilder.AppendFormat("User :{0} Successfully logged into BO System as external user.",userCredential.Username);
                    _logger.Info(_errorMessageBuilder.ToString());
                    return retValue;
                }
                _errorMessageBuilder.Append("Successfully invoked authentication service. But did not receive valid output. Status Code : "+ authResult.StatusCode);
            }
            catch (System.Exception ex)
            {
                _errorMessageBuilder.Append("An error occurred calling authentication service. Exception details "+ ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_errorMessageBuilder.ToString());
            //TODO: Check if null is handled or return expected return type. Just have one return.
            return null;
        }

        [System.Web.Http.HttpGet]
        public bool GetBOUserLogoff(string authToken, string sessionId)
        {
            string baseUrl = ConfigurationManager.AppSettings["BOBaseUrl"];
            string userAuth = ConfigurationManager.AppSettings["BOExternalAuth"];
            _errorMessageBuilder.AppendFormat("Logging user out of system. BO Base Url {0} and External Auth type {1}",baseUrl,userAuth);
            bool response = false;
            BOAuthentication boAuthmodel = new BOAuthentication()
            {
                UserName = string.Empty,
                Password = string.Empty,
                URI = baseUrl,
                UserAuth = userAuth,
                LogonToken = authToken,
                BOSesssionID = sessionId
            };
            try
            {
                _errorMessageBuilder.Append("Invoking logout in authentication service");
                var authResult = _authenticationService.Logoff(boAuthmodel);
                response = authResult;
            }
            catch (System.Exception ex)
            {
                _errorMessageBuilder.AppendFormat("An error occurred. But could be ignored if system says token expired. Exception details {0}",ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                response = false;
            }
            _logger.Info(_errorMessageBuilder.ToString());
            return response;
        }                

        #endregion
    }
}
