using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Service;
using AHP.Repository;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AHP.Web.Api.Controllers
{
    [RoutePrefix("api/UserInfo")]
    public class UserinfoController : ApiController
    {

        #region -- Members --

        private readonly IUserinfoManager _userInfoManager;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        private readonly IEmailSenderService _emailDeliveryService;

        private readonly IAuthenticationService _authService;

        #endregion

        #region -- Constructors --
        public UserinfoController(IUserinfoManager userInfoManager,
            IActiveAnalyticsLogger logger,
            IEmailSenderService emailDeliveryService
            ,IAuthenticationService authService)
        {
            _userInfoManager = userInfoManager;
            _logger = logger;
            _logMessages = new StringBuilder();
            _emailDeliveryService = emailDeliveryService;
            _authService = authService;
        }
        #endregion

        #region -- Action Methods --

        [System.Web.Http.HttpGet]
        [Route("ListUsers")]
        public GenericResponse<List<AHP.Core.DTO.ExternalUserInfo>> GetAllUsers()
        {
            GenericResponse<List<AHP.Core.DTO.ExternalUserInfo>> response = new GenericResponse<List<AHP.Core.DTO.ExternalUserInfo>>();
            try
            {
                _logMessages.Append("Retrieving all users in the data source.");
                response = _userInfoManager.ListUsers();
                if (response.Success)
                {
                    _logMessages.AppendFormat("There are {0} users in the data source.", response.Data.Count);
                }
                else
                {
                    _logMessages.Append("Error occurred getting users list.");
                }              
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Failed to get user list. Please try again");
                _logMessages.AppendFormat("An Error occurred getting users list. Exception info " + ex.Message);
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("CreateUser")]
        public AHP.Core.Model.GenericResponse<bool> CreateUserAccount(AHP.Core.DTO.ExternalUserInfo userInfo)
        {
            AHP.Core.Model.GenericResponse<bool> response = new Core.Model.GenericResponse<bool>();
            try
            {
                if (userInfo == null)
                {
                    response.Success = false;
                    response.Data = false;
                    response.Errors.Add("User information provided is empty");
                    return response;
                }
                response = _userInfoManager.CreateUser(userInfo);

                //send welcome email to admin
                if (response.Success && response.Data)
                {
                    string[] toEmail = System.Configuration.ConfigurationManager.AppSettings["WelcomeEmailTo"].Split(',');
                    //send user details email. To User
                    _logMessages.AppendFormat("Sending email to admin {0} with user details.",string.Join(";",toEmail));
                    string fromEmail = System.Configuration.ConfigurationManager.AppSettings["EmailFROM"];
                    _logMessages.AppendFormat("Welcome email will be sent from {0}.", fromEmail);
                    string subject = System.Configuration.ConfigurationManager.AppSettings["NewUserSubject"];
                    _logMessages.AppendFormat("Subject of the email is {0}.", subject);                    

                    string welcomeEmailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/EmailTemplates/NewUserEmail.html"));

                    welcomeEmailBody = welcomeEmailBody.Replace("{Firstname}",userInfo.Firstname);
                    welcomeEmailBody = welcomeEmailBody.Replace("{Lastname}", userInfo.Lastname);
                    welcomeEmailBody = welcomeEmailBody.Replace("{Email}", userInfo.Email);
                    welcomeEmailBody = welcomeEmailBody.Replace("{Username}", userInfo.Username);
                    welcomeEmailBody = welcomeEmailBody.Replace("{SupplierId}", userInfo.SupplierId);
                    welcomeEmailBody = welcomeEmailBody.Replace("{Role}", userInfo.Role);
                    welcomeEmailBody = welcomeEmailBody.Replace("{Company}", userInfo.Company);
                    //welcomeEmailBody = welcomeEmailBody.Replace("{Birthyear}", userInfo.BirthYear.ToString());
                    //welcomeEmailBody = welcomeEmailBody.Replace("{Birthmonth}", userInfo.BirthMonth.ToString());
                    //welcomeEmailBody = welcomeEmailBody.Replace("{Zipcode}", userInfo.ZipCode);

                    if (_emailDeliveryService.SendEmail(fromEmail, toEmail, new string[] { }, subject, welcomeEmailBody))
                    {
                        response.Success = true;
                        response.Data = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = false;
                        response.Errors.Add("User successfully created. Failed to send email to admin with user information.");                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred creating new user account. Error info - ",ex);
                response.Success = false;
                response.Errors.Add("Error occurred creating new user account. Please try again.");
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("UserDetails")]
        public AHP.Core.Model.GenericResponse<Core.DTO.ExternalUserInfo> GetUserDetails(string username)
        {
            AHP.Core.Model.GenericResponse<Core.DTO.ExternalUserInfo> response = new GenericResponse<Core.DTO.ExternalUserInfo>();
            try
            {
                response = _userInfoManager.GetUserDetails(username);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred getting user details",ex);
                response.Success = false;
                response.Errors.Add("Error occurred getting user details.");
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("UnlockUser")]
        public AHP.Core.Model.GenericResponse<bool> UnlockUserAccount(string username,string email)
        {
            AHP.Core.Model.GenericResponse<bool> response = new Core.Model.GenericResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
                {
                    response.Success = false;
                    response.Data = false;
                    response.Errors.Add("Please provide username and email to activate user account");
                    return response;
                }
                response = _userInfoManager.UnlockUserAccount(username,email);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred activating user account. Error info - ", ex);
                response.Success = false;
                response.Errors.Add("Error occurred activating user account. Please try again.");
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("LockUser")]
        public AHP.Core.Model.GenericResponse<bool> LockUserAccount(string username, string email)
        {
            AHP.Core.Model.GenericResponse<bool> response = new Core.Model.GenericResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
                {
                    response.Success = false;
                    response.Data = false;
                    response.Errors.Add("Please provide username and email to deactivate user account");
                    return response;
                }
                response = _userInfoManager.LockUserAccount(username, email);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred locking user account. Error info - ", ex);
                response.Success = false;
                response.Errors.Add("Error occurred locking user account. Please try again.");
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("ActivateEmail")]
        public AHP.Core.Model.GenericResponse<bool> ActivateEmail(string username, string email)
        {
            AHP.Core.Model.GenericResponse<bool> apiResponse = new Core.Model.GenericResponse<bool>();
            AHP.Core.Model.GenericResponse<bool> dbResponse = new Core.Model.GenericResponse<bool>();
            try
            {
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
                {
                    //send welcome email. If successfully emailed then we will activate their email address on File
                    _logMessages.AppendFormat("Setting up email for user with username {0} whose has email {1}.", username, email);
                    string fromEmail = System.Configuration.ConfigurationManager.AppSettings["EmailFROM"];
                    _logMessages.AppendFormat("Welcome email will be sent from {0}.", fromEmail);
                    string subject = System.Configuration.ConfigurationManager.AppSettings["WelcomeSubject"];
                    _logMessages.AppendFormat("Subject of the email is {0}.", subject);
                    string portalUrl = System.Configuration.ConfigurationManager.AppSettings["LogonUrl"];
                    _logMessages.AppendFormat("Logon url for the user is {0}.", portalUrl);

                    string welcomeEmailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/EmailTemplates/WelcomeEmail.html"));
                    welcomeEmailBody = welcomeEmailBody.Replace("{ServerUrl}", portalUrl);
                    welcomeEmailBody = welcomeEmailBody.Replace("{Username}", username);
                    welcomeEmailBody = welcomeEmailBody.Replace("{CopyrightYear}", DateTime.Today.Year.ToString());

                    if (_emailDeliveryService.SendEmail(fromEmail, new[] { email }, new string[] { }, subject, welcomeEmailBody))
                    {
                        apiResponse.Success = true;                   
                        _logMessages.AppendFormat("Welcome email has been sent successfully to user {0} with email {1}.", username, email);
                        dbResponse =_userInfoManager.ActivateEmail(username, email, true);
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Errors.Add("Failed to deliver welcome email. Please correct and try again.");
                        _logMessages.AppendFormat("Error occurred sending email to user {0} with email {1}.", username, email);
                        dbResponse = _userInfoManager.ActivateEmail(username, email, false);
                    }

                    //operation succeeded only if email and db operation succeeded
                    apiResponse.Success = apiResponse.Success && dbResponse.Success;
                    apiResponse.Errors = dbResponse.Errors;
                    apiResponse.Data = dbResponse.Data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                    apiResponse.Errors.Add("Please provide username and email to activate user account");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred activating user account. Error info - ", ex);
                apiResponse.Success = false;
                apiResponse.Errors.Add("Error occurred activating user account. Please try again.");
            }
            return apiResponse;
        }

        [System.Web.Http.HttpPost]
        [Route("ResetPassword")]
        public AHP.Core.Model.GenericResponse<bool> ResetPassword(string username, string email,bool changePwdOnLogon)
        {
            AHP.Core.Model.GenericResponse<bool> apiResponse = new Core.Model.GenericResponse<bool>();
            AHP.Core.Model.GenericResponse<string> dbResponse = new Core.Model.GenericResponse<string>();
            try
            {
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
                {

                    //Reset user pwd only if email is activated
                    dbResponse = _userInfoManager.ResetPassword(username,email, changePwdOnLogon);

                    if(dbResponse.Success)
                    {
                        if (string.IsNullOrEmpty(dbResponse.Data))
                        {
                            apiResponse.Errors.Add("Password could not be reset");
                            apiResponse.Success = false;
                            return apiResponse;
                        }
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Errors = dbResponse.Errors;
                        return apiResponse;
                    }

                    //send the new password to the requested email address

                    //send welcome email. If successfully emailed then we will activate their email address on File
                    _logMessages.AppendFormat("Resetting the password for user {0} with email {1}.", username, email);
                    string fromEmail = System.Configuration.ConfigurationManager.AppSettings["EmailFROM"];
                    _logMessages.AppendFormat("Password reset email will be sent from {0}.", fromEmail);
                    string subject = System.Configuration.ConfigurationManager.AppSettings["ResetPasswordSubject"];
                    _logMessages.AppendFormat("Subject of the email is {0}.", subject);
                    string portalUrl = System.Configuration.ConfigurationManager.AppSettings["LogonUrl"];
                    _logMessages.AppendFormat("Logon url for the user is {0}.", portalUrl);

                    string welcomeEmailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/EmailTemplates/ResetPassword.html"));
                    welcomeEmailBody = welcomeEmailBody.Replace("{ServerUrl}", portalUrl);
                    //welcomeEmailBody = welcomeEmailBody.Replace("{Username}", username);
                    welcomeEmailBody = welcomeEmailBody.Replace("{RandomPassword}", dbResponse.Data);
                    welcomeEmailBody = welcomeEmailBody.Replace("{CopyrightYear}", DateTime.Today.Year.ToString());

                    if (_emailDeliveryService.SendEmail(fromEmail, new[] { email }, new string[] { }, subject, welcomeEmailBody))
                    {
                        apiResponse.Success = true;
                        apiResponse.Data = true;
                        _logMessages.AppendFormat("Password reset email has been sent successfully to user {0} with email {1}.", username, email);                        
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Data = false;
                        apiResponse.Errors.Add("Failed to deliver password reset email. Please correct and try again.");
                        _logMessages.AppendFormat("Error occurred sending email to user {0} with email {1}.", username, email);
                    }

                    //operation succeeded only if email and db operation succeeded
                    apiResponse.Success = apiResponse.Success && dbResponse.Success;
                    apiResponse.Errors = dbResponse.Errors;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                    apiResponse.Errors.Add("Please provide username and email to activate user account");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred resetting user password. Error info - ", ex);
                apiResponse.Success = false;
                apiResponse.Errors.Add("Error occurred resetting user password. Please try again.");
            }
            return apiResponse;
        }

        [System.Web.Http.HttpPost]
        [Route("UpdateUser")]
        public AHP.Core.Model.GenericResponse<AHP.Core.DTO.ExternalUserInfo> UpdateUserAccount(AHP.Core.DTO.ExternalUserInfo userInfo)
        {
            AHP.Core.Model.GenericResponse<AHP.Core.DTO.ExternalUserInfo> response = new Core.Model.GenericResponse<AHP.Core.DTO.ExternalUserInfo>();
            try
            {
                if (userInfo == null)
                {
                    response.Success = false;
                    response.Errors.Add("User information provided is empty");
                    return response;
                }
                response = _userInfoManager.UpdateUser(userInfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred creating new user account. Error info - ", ex);
                response.Success = false;
                response.Errors.Add("Error occurred creating new user account. Please try again.");
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("ActivateUser")]
        public AHP.Core.Model.GenericResponse<bool> ActivateUserAccount(string username,string email)
        {
            AHP.Core.Model.GenericResponse<bool> response = new GenericResponse<bool>();
            response = _userInfoManager.ActivateUser(username, email);
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("DeactivateUser")]
        public AHP.Core.Model.GenericResponse<bool> DeactivateUserAccount(string username, string email)
        {
            AHP.Core.Model.GenericResponse<bool> response = new GenericResponse<bool>();
            response = _userInfoManager.DeactivateUser(username, email);
            return response;
        }


        [System.Web.Http.HttpPost]
        [Route("Login")]
        public GenericResponse<AHP.Core.DTO.ExternalUserInfo> Login(string username,string password)
        {
            GenericResponse<AHP.Core.DTO.ExternalUserInfo> response = new GenericResponse<AHP.Core.DTO.ExternalUserInfo>();
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    response.Success = false;
                    response.Errors.Add("Username is required");
                    return response;
                }

                if (string.IsNullOrEmpty(password))
                {
                    response.Success = false;
                    response.Errors.Add("Password is required");
                    return response;
                }

               response = _userInfoManager.Logon(username, password);
               _logMessages.Append(string.Join(",",response.Errors));
            }
            catch (Exception ex)
            {
                _logMessages.AppendFormat("Login failed for user {0}, Exception message is {1}",username, ex.Message);
            }
            _logger.Info(_logMessages.ToString());
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("GetUserSessionInfo")]
        public GenericResponse<AHP.Core.Model.BOUserSessionInfo> GetSessionInfo(string token)
        {
            GenericResponse<AHP.Core.Model.BOUserSessionInfo> response = new GenericResponse<BOUserSessionInfo>();
            response = _authService.GetUserSessionInfo(token);
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("GetQuestionsForUser")]
        public GenericResponse<List<AHP.Core.DTO.UserSecurityOption>> GetQuestionsForUser(string username)
        {
            GenericResponse<List<AHP.Core.DTO.UserSecurityOption>> response = new GenericResponse<List<Core.DTO.UserSecurityOption>>();
            response = _userInfoManager.GetSecurityOptionsForUser(username);            
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("ChangePassword")]
        public GenericResponse<bool> ChangePassword(string username,string oldPassword,string newPassword)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            response = _userInfoManager.ChangePassword(username,oldPassword,newPassword);
            return response;
        }

        [System.Web.Http.HttpPost]
        [Route("GetSecurityQuestion")]
        public List<string> GetSecurityQuestionList()
        {
            return _userInfoManager.GetSecurityQuestions();
        }

        [System.Web.Http.HttpPost]
        [Route("UpdateSecurityQuestions")]
        public GenericResponse<bool> UpdateSecurityQuestions(Core.DTO.UpdateSecurityQuestionsRequest securityQuestionAnswers)
        {
            return _userInfoManager.SetupQuestions(securityQuestionAnswers.Username, securityQuestionAnswers.SecurityQuestions);
        }

        [System.Web.Http.HttpPost]
        [Route("RemoveQuestions")]
        public GenericResponse<bool> RemoveSecurityQuestions(string username)
        {            
            return _userInfoManager.ClearSecurityAnswers(username);
        }

        
        [System.Web.Http.HttpPost]
        [Route("ValidateQuestions")]
        public GenericResponse<bool> ResetPassword(Core.DTO.UpdateSecurityQuestionsRequest securityQuestionAnswers)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            GenericResponse<AHP.Core.DTO.PasswordResetResponse> dbResponse = _userInfoManager.ResetPassword(securityQuestionAnswers.Username, securityQuestionAnswers.SecurityQuestions);            
            response.Success = dbResponse.Success;
            response.Errors = dbResponse.Errors;
            if (dbResponse.Success)
            {
                if (dbResponse.Data == null)
                {
                    response.Errors.Add("Random password could not be generated. Please try again.");
                    response.Success = false;
                }
                else
                {                    
                    //send welcome email. If successfully emailed then we will activate their email address on File
                    _logMessages.AppendFormat("Resetting the password for user {0} with email {1}.", dbResponse.Data.Username,dbResponse);
                    string fromEmail = System.Configuration.ConfigurationManager.AppSettings["EmailFROM"];
                    _logMessages.AppendFormat("Password reset email will be sent from {0}.", fromEmail);
                    string subject = System.Configuration.ConfigurationManager.AppSettings["ResetPasswordSubject"];
                    _logMessages.AppendFormat("Subject of the email is {0}.", subject);
                    string portalUrl = System.Configuration.ConfigurationManager.AppSettings["LogonUrl"];
                    _logMessages.AppendFormat("Logon url for the user is {0}.", portalUrl);

                    string welcomeEmailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/EmailTemplates/ResetPassword.html"));
                    welcomeEmailBody = welcomeEmailBody.Replace("{ServerUrl}", portalUrl);
                    //welcomeEmailBody = welcomeEmailBody.Replace("{Username}", securityQuestionAnswers.Username);
                    welcomeEmailBody = welcomeEmailBody.Replace("{RandomPassword}", dbResponse.Data.NewPassword);
                    welcomeEmailBody = welcomeEmailBody.Replace("{CopyrightYear}", DateTime.Today.Year.ToString());

                    if (_emailDeliveryService.SendEmail(fromEmail, new[] { dbResponse.Data.Email }, new string[] { }, subject, welcomeEmailBody))
                    {
                        response.Success = true;
                        response.Data = true;
                        _logMessages.AppendFormat("Password reset email has been sent successfully to user {0} with email {1}.", dbResponse.Data.Username, dbResponse.Data.Email);
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = false;
                        response.Errors.Add("We could not deliver random password to your email address on file.");
                        _logMessages.AppendFormat("Error occurred sending email to user {0} with email {1}.", dbResponse.Data.Username, dbResponse.Data.Email);
                    }

                    //operation succeeded only if email and db operation succeeded
                    response.Success = response.Success && dbResponse.Success;
                    response.Errors = dbResponse.Errors;
                }
            }
            return response;
        }


        [HttpPost]
        [Route("SAPLogon")]
        public GenericResponse<BOUserSessionInfo> Logon(string username)
        {
            GenericResponse<BOUserSessionInfo> response = new GenericResponse<BOUserSessionInfo>();

            try
            {
                RestClient boRestClient = new RestClient(ConfigurationManager.AppSettings["RestUrl"]);
                RestRequest logonRequest = new RestRequest("logon/trusted", Method.GET);
                logonRequest.AddHeader("Content-Type", "application/json");
                logonRequest.AddHeader("Accept", "application/json");
                logonRequest.AddHeader("X-SAP-TRUSTED-USER", username);
                IRestResponse<TrustedLogonResponse> trustedLogonResponse = boRestClient.Execute<TrustedLogonResponse>(logonRequest);
                if (trustedLogonResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (trustedLogonResponse == null
                        || trustedLogonResponse.Data == null
                        || !string.IsNullOrEmpty(trustedLogonResponse.Data.error_code)
                        || string.IsNullOrEmpty(trustedLogonResponse.Data.logonToken))
                    {
                        response.Success = false;
                        response.Errors.Add("Username does not exist on reporting server. Please contact AHM IT for assistance.");
                        return response;
                    }

                    GenericResponse<AHP.Core.Model.BOUserSessionInfo>  sesInfo = GetSessionInfo(trustedLogonResponse.Data.logonToken);
                    if (sesInfo.Success)
                    {
                        response.Success = sesInfo.Success;
                        response.Data = sesInfo.Data;
                        response.Errors = sesInfo.Errors;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add("Could not retrieve user session information. Please try again.");
                    }                                     
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add("Username does not exist on reporting server. Please contact AHM IT for assistance.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while login to web intelligence", ex);
                response.Success = false;
                response.Errors.Add("Could not retrieve user information from server. Please try again.");
            }

            return response;
        }

        [HttpPost]
        [Route("GetAllInternalUsers")]
        public GenericResponse<List<AHP.Core.DTO.InternalUserInfo>> GetAllInternalUsers()
        {
            GenericResponse<List<AHP.Core.DTO.InternalUserInfo>> response = new GenericResponse<List<AHP.Core.DTO.InternalUserInfo>>();
            try
            {
                _logMessages.Append("Retrieving all internal users in the data source.");
                response = _userInfoManager.ListAllInternalUsers();
                if (response.Success)
                {
                    _logMessages.AppendFormat("There are {0} users in the data source.", response.Data.Count);
                }
                else
                {
                    _logMessages.Append("Error occurred getting users list.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Failed to get user list. Please try again");
                _logMessages.AppendFormat("An Error occurred getting internal users list. Exception info " + ex.Message);
            }
            _logger.Info(_logMessages.ToString());
            return response;
        }

        [HttpPost]
        [Route("MapInternalUser")]
        public GenericResponse<bool> MapActivehealthIdToTableau(Core.DTO.InternalUserInfo userInfo)
        {
            AHP.Core.Model.GenericResponse<bool> response = new Core.Model.GenericResponse<bool>();
            try
            {
                if (userInfo == null)
                {
                    response.Success = false;
                    response.Data = false;
                    response.Errors.Add("User information provided is empty");
                    return response;
                }
                response = _userInfoManager.MapInternalUser(userInfo);                
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred creating new user account. Error info - ", ex);
                response.Success = false;
                response.Errors.Add("Could not process your request");
            }
            return response;
        }

        [HttpPost]
        [Route("UpdateInternalUser")]
        public GenericResponse<bool> UpdateInternalUserInfo(Core.DTO.InternalUserInfo userInfo)
        {
            AHP.Core.Model.GenericResponse<bool> response = new Core.Model.GenericResponse<bool>();
            try
            {
                if (userInfo == null)
                {
                    response.Success = false;
                    response.Data = false;
                    response.Errors.Add("User information provided is empty");
                    return response;
                }
                response = _userInfoManager.UpdateInternalUser(userInfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred creating new user account. Error info - ", ex);
                response.Success = false;
                response.Errors.Add("Could not process your request");
            }
            return response;
        }

        [HttpPost]
        [Route("GetAllTableauViews")]
        public GenericResponse<List<AHP.Core.DTO.TableauViewInfo>> GetAllTableauViews ()
        {
            GenericResponse<List<AHP.Core.DTO.TableauViewInfo>> response = new GenericResponse<List<AHP.Core.DTO.TableauViewInfo>>();
            try
            {
                _logMessages.Append("Retrieving all internal users in the data source.");
                response = _userInfoManager.ListAllTableauViews();
                if (response.Success)
                {
                    _logMessages.AppendFormat("There are {0} views in the data source.", response.Data.Count);
                }
                else
                {
                    _logMessages.Append("Error occurred getting tableau views list.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Failed to get tableau views list. Please try again");
                _logMessages.AppendFormat("An Error occurred getting tableau views list. Exception info " + ex.Message);
            }
            _logger.Info(_logMessages.ToString());
            return response;
        }

        [HttpPost]
        [Route("AddTableauInfo")]
        public GenericResponse<bool> AddTableauViewInfo(AHP.Core.DTO.TableauViewInfo tabInfo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                response = _userInfoManager.AddTableauInfo(tabInfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Error adding new tableau info",ex);
                response.Errors.Add("Could not process the request. Please try again");
                response.Success = false;
            }
            return response;
        }

        [HttpPost]
        [Route("UpdateTableauInfo")]
        public GenericResponse<bool> UpdateTableauViewInfo(AHP.Core.DTO.TableauViewInfo tabInfo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                response = _userInfoManager.UpdateTableauInfo(tabInfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Error adding new tableau info", ex);
                response.Errors.Add("Could not process the request. Please try again");
                response.Success = false;
            }
            return response;
        }

        [HttpPost]
        [Route("GetAllViewAssociation")]
        public GenericResponse<List<Core.DTO.TableauViewUserAssociation>> GetUsersForView(string viewId)
        {
            GenericResponse<List<Core.DTO.TableauViewUserAssociation>> response = new GenericResponse<List<Core.DTO.TableauViewUserAssociation>>();
            try
            {
                response = _userInfoManager.GetUsersForView(viewId);
            }
            catch (Exception ex)
            {
                _logger.Error("Error listing users for the view", ex);
                response.Errors.Add("Could not process the request");
                response.Success = false;
            }
            return response;
        }

        [HttpPost]
        [Route("UpdateViewAssociation")]
        public GenericResponse<bool> UpdateViewsassocitation(string viewId, List<Core.DTO.TableauViewUserAssociation> viewsUserAssociation)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                response = _userInfoManager.UpdateUsersForView(viewId,viewsUserAssociation);
            }
            catch (Exception ex)
            {
                _logger.Error("Error listing users for the view", ex);
                response.Errors.Add("Could not process the request");
                response.Success = response.Data = false;
            }
            return response;
        }

        [HttpPost]
        [Route("GetViewsForUser")]
        public GenericResponse<List<AHP.Core.DTO.TableauViewInfo>> GetTableauviewsForUser(string username,string usertype)
        {
            GenericResponse<List<AHP.Core.DTO.TableauViewInfo>> apiResponse = new GenericResponse<List<Core.DTO.TableauViewInfo>>();
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    apiResponse.Errors.Add("Username is required");
                }
                if (string.IsNullOrEmpty(usertype))
                {
                    apiResponse.Errors.Add("User type is required");
                }
                if (!usertype.Equals("External",StringComparison.OrdinalIgnoreCase) && !usertype.Equals("Internal", StringComparison.OrdinalIgnoreCase))
                {
                    apiResponse.Errors.Add("Only external or internal users are allowed");
                }
                if (apiResponse.Errors.Count > 0)
                {
                    return apiResponse;
                }
                apiResponse = _userInfoManager.GetViewsOnUser(username,usertype);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occurred retrieving tableau views for user", ex);
                apiResponse.Errors.Add("Could not process your request");
                apiResponse.Success = false;
            }
            return apiResponse;
        }

        #endregion

    }
}
