#region -- File Headers --

/*
 * Author: James Deepak
 * Purpose: Validate Security Questions and Reset user password
 * Date: June 1 2017
 * 
 */

#endregion

#region -- Using Directives --
using AHP.Core.Logger;
using AHP.Web.Helpers;
using AHP.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
#endregion

namespace AHP.Web.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [AllowAnonymous]
    public class AccountRecoveryController : BaseController
    {
        #region -- Members --

        private readonly IServerDataRestClient _restClient;

        private readonly StringBuilder _loggerMessages;

        private readonly IActiveAnalyticsLogger _logger;

        #endregion

        #region -- Constructors --
        public AccountRecoveryController(IServerDataRestClient restClient,
    IActiveAnalyticsLogger logger)
        {
            _restClient = restClient;
            _logger = logger;
            _loggerMessages = new StringBuilder();
        }
        #endregion

        #region -- Action Methods --
        public ActionResult Index()
        {
            return RedirectToAction("ResetPassword", "AccountRecovery");
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            _logger.Info("User landed on password reset screen.");
            AHP.Web.ViewModel.AccountRecoveryInfoViewModel pwdResetViewmodel = new ViewModel.AccountRecoveryInfoViewModel();
            return View(pwdResetViewmodel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ValidateUsername(ViewModel.AccountRecoveryInfoViewModel accountInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("Username", "Username is required");
                    _logger.Info("User submitted password reset form. But username does not exist in form value. Showing validation message.");
                    return View("~/Views/AccountRecovery/ResetPassword.cshtml", accountInfo);
                }

                //Check user account disabled or not
                GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo> userResponse = _restClient.GetUserDetails(accountInfo.Username);
                if (!userResponse.Success || userResponse.Data == null)
                {
                    ModelState.AddModelError("Username","Account information does not exist");
                    return View("~/Views/AccountRecovery/ResetPassword.cshtml", accountInfo);
                }

                if (!userResponse.Data.IsActive)
                {
                    ModelState.AddModelError("Username", "Your account has been disabled. Please contact your account manager.");
                    return View("~/Views/AccountRecovery/ResetPassword.cshtml", accountInfo);
                }

                //Get security questions for the user.
                GenericAjaxResponse<List<AHP.Core.DTO.UserSecurityOption>> apiResponse = _restClient.GetSecurityQuestionsForUser(accountInfo.Username);

                //only two questions need to be present and user should also be present
                if (apiResponse.Success && apiResponse.Data != null && apiResponse.Data.Count == 3)
                {
                    ViewModel.UserQuestionsViewmodel usrQuestionInfo = new ViewModel.UserQuestionsViewmodel()
                    {
                        SecurityQuestions = new List<string>()
                    };

                    //Pre fill primary and secondary questions that the user had selected
                    usrQuestionInfo.PrimarySelectedQuestion = apiResponse.Data[0].Question;
                    usrQuestionInfo.SecondarySelectedQuestion = apiResponse.Data[1].Question;
                    usrQuestionInfo.ThirdSelectedQuestion = apiResponse.Data[2].Question;
                    ViewBag.Username = accountInfo.Username;

                    _logger.Info("User details exists. Redirecting to answer security question page.");
                    return View("~/Views/AccountRecovery/AnswerSecurityQuestions.cshtml", usrQuestionInfo);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account information does not exist or you haven't setup your security questions in the system yet.");
                    return View("~/Views/AccountRecovery/ResetPassword.cshtml", accountInfo);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,"We are sorry. Could not process your request at this time.");
                _logger.Error("Error occurred validating username", ex);
                return View("~/Views/AccountRecovery/ResetPassword.cshtml", accountInfo);
            }
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AnswerSecurityQuestions(ViewModel.UserQuestionsViewmodel securityQuestions,string username)
        {

            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("ResetPassword","AccountRecovery",routeValues: new { id = "user-does-not-exist" });
                }

                ViewBag.Username = username;

                if (!ModelState.IsValid)
                {
                    if (securityQuestions == null)
                    {                        
                        return RedirectToAction("ResetPassword", "AccountRecovery", routeValues: new { id = "invalid-user-input" });
                    }
                    else
                    {
                        securityQuestions.SecurityQuestions = new List<string>();
                        //Get security questions for the user.
                        GenericAjaxResponse<List<AHP.Core.DTO.UserSecurityOption>> apiResponse = _restClient.GetSecurityQuestionsForUser(username);

                        //only two questions need to be present and user should also be present
                        if (apiResponse.Success && apiResponse.Data != null && apiResponse.Data.Count == 3)
                        {
                            securityQuestions.SecurityQuestions.AddRange(apiResponse.Data.Select(ques => ques.Question));
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Errors[0]);
                        }
                        return View("~/Views/AccountRecovery/AnswerSecurityQuestions.cshtml", securityQuestions);
                    }                    
                }

                List<AHP.Core.DTO.UserSecurityOption> usrSecurityQuestions = new List<Core.DTO.UserSecurityOption>();
                usrSecurityQuestions.Add(new Core.DTO.UserSecurityOption()
                {
                    Answer = securityQuestions.PrimaryProvidedAnswer,
                    Question = securityQuestions.PrimarySelectedQuestion
                });
                usrSecurityQuestions.Add(new Core.DTO.UserSecurityOption()
                {
                    Answer = securityQuestions.SecondaryProvidedAnswer,
                    Question = securityQuestions.SecondarySelectedQuestion
                });
                usrSecurityQuestions.Add(new Core.DTO.UserSecurityOption()
                {
                    Answer = securityQuestions.ThirdProvidedAnswer,
                    Question = securityQuestions.ThirdSelectedQuestion
                });
                GenericAjaxResponse<bool> resetPwdResponse = _restClient.ResetPassword(username, usrSecurityQuestions);
                if (resetPwdResponse.Success && resetPwdResponse.Data)
                {
                    return View("~/Views/AccountRecovery/PasswordResetSuccess.cshtml");
                }
                string errMessage = resetPwdResponse.Errors[0];
                if (!string.IsNullOrEmpty(errMessage))
                {
                    errMessage = errMessage.Replace("<<click here>>", "<a href='" + Url.Action("ResetPassword", "AccountRecovery") + "' title='reset password'>click here</a>") + " to try resetting your password again. <br/> <strong>Note:</strong> If you do not remember the answers to your security questions please contact ActiveHealth Management support at (800) 491 - 3464.";
                }
                ModelState.AddModelError(string.Empty, errMessage);
                securityQuestions.SecurityQuestions = new List<string>();
                return View("~/Views/AccountRecovery/AnswerSecurityQuestions.cshtml", securityQuestions);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred validating answers to security questions.",ex);
                ModelState.AddModelError(string.Empty, "An error occurred validating security answers");
            }
            return View("~/Views/AccountRecovery/AnswerSecurityQuestions.cshtml", securityQuestions);
        }

        #endregion
    }
}