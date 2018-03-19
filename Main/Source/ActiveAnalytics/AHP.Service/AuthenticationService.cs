using AHP.Core.Service;
using System.Net;
using System.Xml;
using System.IO;
using AHP.Core.Repository;
using AHP.Core.Model;
using AHP.Service.BOSession;
using System;
using AHP.Core.Logger;
using System.Text;

namespace AHP.Service
{

    /// <summary>
    /// Authenticates and provides users session information
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        #region -- Members --

        private readonly IBIRepository _biRepository;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        #endregion

        #region -- Constructors --

        public AuthenticationService(IBIRepository biRepository, IActiveAnalyticsLogger logger)
        {
            _biRepository = biRepository;
            _logger = logger;
            _logMessages = new StringBuilder();
        }

        #endregion

        #region -- IAuthenticationService Implementation --

        public bool Logoff(BOAuthentication authModel)
        {
            bool response = false;
            using (SessionPortClient serviceProxy = new SessionPortClient())
            {                
                try
                {
                    _logMessages.AppendFormat("Performing logout for user with session id {0}.", authModel.BOSesssionID);
                    serviceProxy.logout(authModel.BOSesssionID);
                    response = true;
                    _logMessages.Append("Successfully logged out");
                }
                catch (System.Exception ex)
                {
                    _logMessages.AppendFormat("Logged out exception occurred. Can ignore exception message {0}.", ex.Message);
                    response = false;
                    //eat up exception, UI really doesn't care for it
                }
            }
            _logger.Info(_logMessages.ToString());
            return response;
        }

        public bool IsLoggedOn(string sessionId)
        {
            bool response = false;
            using (SessionPortClient serviceProxy = new SessionPortClient())
            {
                try
                {
                    // dont need audit messages as this method is being hit every few minutes
                    SessionInfo sesInfo = serviceProxy.getSessionInfo(sessionId);
                    if (sesInfo != null)
                    {
                        //both sessions should match
                        response = sesInfo.SessionID.Equals(sessionId, StringComparison.OrdinalIgnoreCase);
                    }
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                    response = false;
                }
            }
            return response;
        }

        public BOAuthentication AuthenticateUserAndGetToken(BOAuthentication authModel)
        {

            EnterpriseCredential creds = new EnterpriseCredential();
            SessionInfo objInfo = new SessionInfo();
            using (SessionPortClient objClient = new SessionPortClient("BOSession"))
            {
                try
                {
                    creds.Login = authModel.UserName;
                    creds.Password = authModel.Password;
                    creds.AuthType = authModel.UserAuth;
                    _logMessages.AppendFormat("Performing logon with username {0} and authentication type {1}.", authModel.UserName, authModel.UserAuth);
                    objInfo = objClient.login(creds, string.Empty);
                    if (objInfo != null)
                    {
                        authModel.StatusCode = 0;
                        // concatenate with double quotes and store as member
                        authModel.LogonToken = "\"" + objInfo.DefaultToken + "\"";
                        authModel.MustChangePassword = objInfo.MustChangePassword;
                        authModel.BOSesssionID = objInfo.SessionID;
                        authModel.BOSerializedSessionId = objInfo.SerializedSession;
                        _logMessages.AppendFormat("Logon successfull for the user {0}.", authModel.UserName);
                    }
                    else
                    {
                        _logMessages.Append("Logon failed for an unknown reason.");
                        authModel.StatusCode = 4;
                    }
                    _logger.Info(_logMessages.ToString());
                    return authModel;
                }
                catch (System.Exception ex)
                {
                    _logMessages.AppendFormat("Error occurred during logon {0}.", ex.Message);
                    _logger.Info(_logMessages.ToString());
                    throw;
                }
            }
        }

        public string ResetPasswordAndRedirect(BOPasswordReset passwordReset)
        {

            SessionInfo objInfo = new SessionInfo();
            string errorMessage = "";
            try
            {
                using (SessionPortClient objClient = new SessionPortClient("BOSession"))
                {
                    _logMessages.AppendFormat("Performing password reset for user:{0}.", passwordReset.Username);
                    if (passwordReset.AccountLocked)
                    {
                        _logMessages.AppendFormat("Account locked.Performing change password for user:{0}.", passwordReset.Username);
                        objClient.changePassword(passwordReset.SAPLoginToken, passwordReset.OldPassword, passwordReset.NewPassword);
                    }
                    else
                    {
                        EnterpriseCredential creds = new EnterpriseCredential();
                        creds.Login = passwordReset.Username;
                        creds.Password = passwordReset.OldPassword;
                        creds.AuthType = "secEnterprise";

                        _logMessages.Append("Performing logon to the sap BO");

                        objInfo = objClient.login(creds, "");
                        if (objInfo != null)
                        {
                            objClient.changePassword(objInfo.SessionID, passwordReset.OldPassword, passwordReset.NewPassword);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                errorMessage = ex.Message;
                _logMessages.AppendFormat("Error occurred in password reset. Exception message {0}.", ex.Message);
                _logger.Info(_logMessages.ToString());
                return errorMessage;
            }
            _logger.Info(_logMessages.ToString());
            return errorMessage;
        }

        public GenericResponse<AHP.Core.Model.BOUserSessionInfo> GetUserSessionInfo(string token)
        {
            GenericResponse<AHP.Core.Model.BOUserSessionInfo> response = new GenericResponse<BOUserSessionInfo>();
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    response.Success = false;
                    response.Errors.Add("Please supply a logon token to retrieve user session info");
                    return response;
                }

                using (SessionPortClient objClient = new SessionPortClient("BOSession"))
                {
                    _logMessages.Append("Retrieving session information for the user on providing a token.");
                    SessionInfo usrSesInfo = objClient.loginWithToken(token, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                    if (usrSesInfo == null)
                    {
                        response.Success = false;
                        response.Errors.Add("Session information is not available.");
                    }
                    else
                    {
                        _logMessages.Append("Successfully retrieved user session information.");
                        response.Data = new BOUserSessionInfo()
                        {
                            DefaultToken = usrSesInfo.DefaultToken,
                            MustChangePassword = usrSesInfo.MustChangePassword,
                            SerializedSession = usrSesInfo.SerializedSession,
                            SessionId = usrSesInfo.SessionID,
                            UserCUID = usrSesInfo.UserCUID
                        };
                        response.Success = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Unable to retrieve session information for the user");                
                _logMessages.AppendFormat("Error occurred in getting session information. Exception message {0}.", ex.Message);
                _logger.Info(_logMessages.ToString());
            }
            _logger.Info(_logMessages.ToString());
            return response;
        }

        #endregion

    }
}
