using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AHP.Core.Repository;
using AHP.Core.Model;
using AHP.Repository;
using AHP.Core.Logger;
using System.Text;
using AHP.Core.Service;

namespace AHP.Web.Api.Controllers
{
    public class PersonalInfoController : ApiBaseController
    {

        #region -- Members --

        private readonly IActiveAnalyticsLogger _logger;

        private readonly IAuthenticationService _authenticationService;

        private readonly IUserinfoManager _userInfoManager;

        private readonly IEmailSenderService _emailDeliveryService;

        private readonly StringBuilder _logMessages;

        #endregion     

        #region -- Constructors --

        public PersonalInfoController(IActiveAnalyticsLogger logger,
            IAuthenticationService authenticationService,
            IUserinfoManager userInfoManager,
            IEmailSenderService emailDeliveryService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userInfoManager = userInfoManager;
            _emailDeliveryService = emailDeliveryService;
            _logMessages = new StringBuilder();
        }

        #endregion

        #region -- Api Methods --        

        [System.Web.Http.HttpGet]
        public bool IsLoggedOn(string sessionId)
        {
            return _authenticationService.IsLoggedOn(sessionId);
        }

        //[ActionName("ActivateEmail")]
        //[System.Web.Http.HttpPost]
        //public bool ActivateEmailForUser(string username,string email)
        //{
        //    bool response = false;            
        //    try
        //    {
        //        _logMessages.AppendFormat("Setting up email for user with username {0} whose has email {1}.",username,email);
        //        string fromEmail = System.Configuration.ConfigurationManager.AppSettings["EmailFROM"];
        //        _logMessages.AppendFormat("Welcome email will be sent from {0}.", fromEmail);
        //        string subject = System.Configuration.ConfigurationManager.AppSettings["EmailFROM"];
        //        _logMessages.AppendFormat("Subject of the email is {0}.", subject);
        //        string portalUrl = System.Configuration.ConfigurationManager.AppSettings["LogonUrl"];
        //        _logMessages.AppendFormat("Logon url for the user is {0}.", portalUrl);               

        //        string welcomeEmailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/EmailTemplates/WelcomeEmail.html"));
        //        welcomeEmailBody = welcomeEmailBody.Replace("{ServerUrl}",portalUrl);
        //        welcomeEmailBody = welcomeEmailBody.Replace("{Username}", username);
        //        welcomeEmailBody = welcomeEmailBody.Replace("{CopyrightYear}", DateTime.Today.Year.ToString());                
                
        //        if(_emailDeliveryService.SendEmail(fromEmail,new []{email},new string[] { },subject,welcomeEmailBody))
        //        {
        //            response = true;
        //            _logMessages.AppendFormat("Welcome email has been sent successfully to user {0} with email {1}.", username,email);
        //            response = response && _userInfoManager.UpsertEmailAddress(username, email, true);
        //        }
        //        else
        //        {
        //            response = false;
        //            _logMessages.AppendFormat("Error occurred sending email to user {0} with email {1}.", username,email);
        //            response = response &&_userInfoManager.UpsertEmailAddress(username, email, false);
        //        }                
        //    }
        //    catch (Exception ex)
        //    {
        //        _logMessages.AppendFormat("Oops error occurred activating user email. Error info {0}",ex.Message);
        //        response = false;
        //    }
        //    _logger.Info(_logMessages.ToString());
        //    return response;
        //}


        //[System.Web.Http.HttpPost]
        //[ActionName("UpsertSecurityQuestions")]
        //public bool UpdateSecurityQandAnswers(PersonalInfoQuestion question)
        //{
        //    bool response = false;
        //    try
        //    {                
        //        _logMessages.AppendFormat("Setting up security question for user {0},Month Year:{1},ZipCode:{2},Fav Teacher:{3},Fav Kid:{4}.",question.UserName,question.MonthYear,question.ZipCode,question.FavTeacher,question.FavPlaceAsChild);
        //        response = _userInfoManager.SetupSecurityAnswers(question.UserName, question.MonthYear, question.ZipCode, question.FavTeacher, question.FavPlaceAsChild);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logMessages.AppendFormat("An error occurred setting up security question and answer.Error {0}", ex.Message);
        //        response = false;
        //    }
        //    _logger.Info(_logMessages.ToString());
        //    return response;
        //}

        #endregion

    }
}
