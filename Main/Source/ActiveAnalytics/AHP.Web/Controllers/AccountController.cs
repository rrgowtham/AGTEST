using System;
using System.Web.Mvc;
using AHP.Web.Models;
using System.Configuration;
using AHP.Web.Helpers;
using AHP.Core.Model;
using AHP.Web.Api.Models;
using AHP.Core.Logger;
using System.Text;
using System.Web;
using System.Security.Claims;
using AHP.Web.ViewModel;
using System.Collections.Generic;

namespace AHP.Web.Controllers
{
    [AllowAnonymous]
    /// <summary>
    /// Account Controller: Takes care of login/logout and Identity settings.
    /// Needs to inherit directly from Controller class.
    /// </summary>
    public class AccountController : BaseController
    {

        #region -- Members --

        private readonly IServerDataRestClient _restClient;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        private readonly IOwinAuthenticationManager _authManager;

        private readonly ITableauRestConnector _tableauClient;

        #endregion

        #region -- Constructors --

        public AccountController(IActiveAnalyticsLogger logger,
            IServerDataRestClient restClient,
            IOwinAuthenticationManager authManager,
            ITableauRestConnector tableauClient)
        {
            _restClient = restClient;
            _logger = logger;
            _logMessages = new StringBuilder();
            _authManager = authManager;
            _tableauClient = tableauClient;
        }

        #endregion

        #region -- Action Methods --

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(User usrLogin)
        {
            _logMessages.Append("Initiating Login in AccountController.Login .");
            bool success = false;
            string tableauTicket = string.Empty;
            string serviceId = string.Empty;
            string defaultDomain = "";
            ClaimsIdentity userClaims = null;
            bool enableTableau = false;
            if (ModelState.IsValid)
            {
                try
                {
                    enableTableau = ConfigurationManager.AppSettings["enableTableau"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
                    defaultDomain = ConfigurationManager.AppSettings["DefaultDomain"].ToString();
                    if (usrLogin.IsInternalUser)
                    {
                        _logMessages.Append("Performing logon as internal user LDAP Authentication. Username " + usrLogin.UserName + ".");
                        LDAPUser ldapUser = _restClient.IsLDAPAuthenticated(usrLogin.UserName, usrLogin.Password);

                        if (ldapUser == null)
                        {
                            success = false;
                            ModelState.AddModelError(string.Empty,"An error occurred please try again.");
                        }
                        else
                        {
                            switch (ldapUser.LDAPAccessStatus)
                            {
                                case LDAPAccessStatus.UserLogonSuccessful:
                                    _logMessages.Append("Ldap Authentication successfull.");
                                    userClaims = new ClaimsIdentity(
                                        new[]
                                        {
                                            new Claim(ClaimTypes.Name, usrLogin.UserName),
                                            new Claim(ClaimTypes.GivenName, ldapUser.FirstName),
                                            new Claim(ClaimTypes.Surname, ldapUser.LastName),
                                            new Claim(AHP.Core.ClaimTypes.DisplayName, ldapUser.DisplayName),
                                            new Claim(AHP.Core.ClaimTypes.IsInternalUser, usrLogin.IsInternalUser.ToString())
                                        },
                                        Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

                                    success = true;

                                    _logMessages.AppendFormat("Retrieving tableau account name for internal user {0}.",usrLogin.UserName);

                                    //Authenticate with Tableau for trusted ticket
                                    GenericAjaxResponse<string> getAccnameResponse = _restClient.GetTableauAccountname(usrLogin.UserName);

                                    _logMessages.AppendFormat("Tableau account name mapped to user {0} is '{1}'.", usrLogin.UserName,getAccnameResponse.Data);

                                    if (getAccnameResponse.Success && !string.IsNullOrEmpty(getAccnameResponse.Data))
                                    {
                                        GenericAjaxResponse<string> tabSigninResponse = _tableauClient.SignIn(getAccnameResponse.Data);
                                        if (tabSigninResponse.Success)
                                        {
                                            tableauTicket = tabSigninResponse.Data;
                                        }
                                    }

                                    //add tableau ticket to claims
                                    userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.TableauAuthTicket, tableauTicket));

                                    //Authenticate with BO again to get the Token for Reports
                                    _logMessages.Append("Requesting BO Server for user information and token.");

                                    //get session information along wit user information
                                    GenericAjaxResponse<AHP.Core.Model.BOUserSessionInfo> sessionInfo = _restClient.LogonToWebIntelligence(usrLogin.UserName);
                                    if (sessionInfo.Success)
                                    {
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, bool.FalseString));
                                        //AD Users are always a User. They can't be admin
                                        userClaims.AddClaim(new Claim(ClaimTypes.Role, "User"));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.LogonToken, sessionInfo.Data.DefaultToken));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.BOSessionId, sessionInfo.Data.SessionId));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.BOSerializedSession, sessionInfo.Data.SerializedSession));
                                        //Internal users can't change pwd
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangePassword, bool.FalseString));
                                        //for internal users last login date is now
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.LastLogonDate, DateTime.Now.ToShortDateString()));
                                        //internal users don't have pwd expiry
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.PasswordExpired, bool.FalseString));
                                        success = true;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, sessionInfo.Errors[0]);
                                        success = false;
                                    }                                    
                                    break;
                                case LDAPAccessStatus.UserLogonUnsuccessful:
                                    success = false;
                                    ModelState.AddModelError(string.Empty, "The username and password combination you entered is incorrect. Please use the same username and password as your AHM computer and try again.");
                                    _logMessages.Append("Internal user authentication failed for user " + usrLogin.UserName + ".");
                                    break;
                                case LDAPAccessStatus.UserAccountLocked:
                                    success = false;
                                    ModelState.AddModelError(string.Empty, "Your user account is locked. Please contact AHM IT for further assistance");
                                    _logMessages.Append("Internal user account has been locked for user " + usrLogin.UserName + ".");
                                    break;
                                default:
                                    success = false;
                                    ModelState.AddModelError(string.Empty, "Unknown error has occurred. Please try again.");
                                    _logMessages.AppendFormat("User :{0}, got response from AD which is either not success and nor Account locked.", usrLogin.UserName);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        _logMessages.Append("Performing Logon as External user. Authenticating with BO System. Username " + usrLogin.UserName + ".");

                        GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo> apiResponse = _restClient.Login(usrLogin.UserName, usrLogin.Password);

                        if (apiResponse == null)
                        {
                            ModelState.AddModelError(string.Empty, "An error occurred. Please try again");
                            success = false;
                        }
                        else
                        {
                            if (!apiResponse.Success)
                            {
                                success = false;
                                if (apiResponse.Errors.Count >= 1)
                                {
                                    string errMessage = apiResponse.Errors[0];
                                    if (!string.IsNullOrEmpty(errMessage))
                                    {
                                        errMessage = errMessage.Replace("<<click here>>", "<a href='" + Url.Action("ResetPassword", "AccountRecovery") + "' title='reset password'>click here</a>");
                                    }
                                    ModelState.AddModelError(string.Empty, errMessage);
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "An error occurred. Please try again");
                                }
                            }
                            else
                            {
                                if (apiResponse.Data == null)
                                {
                                    success = false;
                                    ModelState.AddModelError(string.Empty, "An error occurred. Please try again");
                                }
                                else
                                {
                                    _logMessages.AppendFormat("Retrieving tableau account name for external user {0}.", usrLogin.UserName);
                                    
                                    //get session information along wit user information
                                    GenericAjaxResponse<AHP.Core.Model.BOUserSessionInfo> sessionInfo = _restClient.LogonToWebIntelligence(apiResponse.Data.Username);

                                    //get security question for the user, if nothing exists then ask user to setup his security questions
                                    GenericAjaxResponse<List<AHP.Core.DTO.UserSecurityOption>> usrQuestions = _restClient.GetSecurityQuestionsForUser(apiResponse.Data.Username);

                                    if (sessionInfo.Success)
                                    {
                                        userClaims = new ClaimsIdentity(new[]
                                        {
                                            new Claim(ClaimTypes.Name, usrLogin.UserName),
                                            new Claim(AHP.Core.ClaimTypes.IsInternalUser, usrLogin.IsInternalUser.ToString())
                                        }, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

                                        userClaims.AddClaim(new Claim(ClaimTypes.GivenName, apiResponse.Data.Firstname));
                                        userClaims.AddClaim(new Claim(ClaimTypes.Surname, apiResponse.Data.Lastname));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.DisplayName, string.Format("{0},{1}", apiResponse.Data.Lastname, apiResponse.Data.Firstname)));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.Company, apiResponse.Data.Company ?? string.Empty));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, usrQuestions.Success ? (usrQuestions.Data.Count != 3).ToString() : bool.FalseString));
                                        userClaims.AddClaim(new Claim(ClaimTypes.Email, apiResponse.Data.Email));
                                        userClaims.AddClaim(new Claim(ClaimTypes.Role, apiResponse.Data.Role));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.LogonToken, sessionInfo.Data.DefaultToken));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.BOSessionId, sessionInfo.Data.SessionId));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.BOSerializedSession, sessionInfo.Data.SerializedSession));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangePassword, apiResponse.Data.ChangePasswordOnLogon.ToString()));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.LastLogonDate, apiResponse.Data.LastLogonDate));
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.PasswordExpired, (apiResponse.Data.PasswordExpiresOn.Date - DateTime.Today).TotalDays <= 0 ? bool.TrueString:bool.FalseString));


                                        _logMessages.AppendFormat("Using Service SID for tableau account name for external user {0}.", usrLogin.UserName);

                                        serviceId = System.Configuration.ConfigurationManager.AppSettings["SID"];
                                        GenericAjaxResponse<string> tabSigninResponse = _tableauClient.SignIn(serviceId);
                                        if (tabSigninResponse.Success)
                                        {
                                            tableauTicket = tabSigninResponse.Data;
                                        }

                                        _logMessages.AppendFormat("Obtained ticket '{0} for external user {1} using SID'",tableauTicket,usrLogin.UserName);

                                        //add tableau ticket to claims
                                        userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.TableauAuthTicket, tableauTicket));

                                        success = true;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, sessionInfo.Errors[0]);
                                        success = false;
                                    }                                  
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logMessages.Append("An Error occurred Exception Message is " + ex.Message + ".");
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    ModelState.AddModelError(string.Empty, "Error occurred processing your request. Please try again");
                    success = false;
                }
            }
            else
            {
                _logMessages.Append("Model Validation Failed.");
                success = false;
            }
            _logger.Info(_logMessages.ToString());

            if (success)
            {
                _authManager.SignIn(Request, userClaims);
                //redirect to customer logon page
                return RedirectToAction("Home", "Customer");
            }
            else
            {
                usrLogin.UserName = string.Empty;
                usrLogin.Password = string.Empty;
                usrLogin.IsInternalUser = false;
                return View("~/Views/Default/login.cshtml", usrLogin);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpGet]
        public ActionResult Logout()
        {
            string message = string.Empty;
            try
            {
                message = "2";
                _logMessages.Append("User being logged out. Username " + Identity.UserName + ".");
                if (Identity != null)
                {
                    //log off of sap webi
                    if (!string.IsNullOrEmpty(Identity.SapToken) && !string.IsNullOrEmpty(Identity.BOSessionId))
                    {
                        bool response = _restClient.LogoffFromSAP(Identity.SapToken, Identity.BOSessionId);
                        if (response)
                        {
                            message = "2";
                        }
                        else
                        {
                            message = "3";
                        }
                    }

                    //Log off of tableau
                    if (!string.IsNullOrEmpty(Identity.TableauAuthTicket))
                    {
                        _tableauClient.SignOut(Identity.TableauAuthTicket);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                message = "3";
                _logMessages.Append("An Error occurred loggin out the user. Exception information " + ex.Message + ".");
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _authManager.SignOut(Request, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Default", new { id = message });
        }

        #endregion

    }
}