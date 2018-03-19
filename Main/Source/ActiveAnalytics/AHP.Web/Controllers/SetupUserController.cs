using AHP.Core.Logger;
using AHP.Web.Helpers;
using AHP.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AHP.Web.Controllers
{
    [Authorize]
    public class SetupUserController : BaseController
    {

        #region -- Members --

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        private readonly IServerDataRestClient _restClient;

        private readonly IOwinAuthenticationManager _authManager;

        #endregion

        #region -- Constructors --

        public SetupUserController(IActiveAnalyticsLogger logger,
            IServerDataRestClient restClient,
            IOwinAuthenticationManager authManager)
        {
            _logger = logger;
            _logMessages = new StringBuilder();
            _restClient = restClient;
            _authManager = authManager;
        }

        #endregion

        #region -- Action Methods --

        // GET: SetupUser
        public ActionResult Index()
        {
            return RedirectToAction("Home", "Customer");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Identity.MustChangePassword || Identity.PasswordExpired)
            {
                ViewModel.PasswordResetViewModel model = new ViewModel.PasswordResetViewModel();
                return View(model);
            }
            else
            {
                 return RedirectToAction("Home", "Customer");
            }            
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(ViewModel.PasswordResetViewModel updatePwdInfo)
        {
            GenericAjaxResponse<bool> changePwdResponse = new GenericAjaxResponse<bool>();
            _logMessages.AppendFormat("Password change requested for user {0}.", Identity.UserName);
            try
            {
                if (ModelState.IsValid)
                {
                    _logMessages.Append("Model validation successfull. Sending password details to service to change password.");
                    changePwdResponse = _restClient.ChangePassword(Identity.UserName, updatePwdInfo.OldPassword, updatePwdInfo.NewPassword);
                }
            }
            catch (Exception ex)
            {
                _logMessages.AppendFormat("Change password ended with error. Exception message is {0}", ex.Message);
            }
            _logger.Info(_logMessages.ToString());
            if (changePwdResponse.Success)
            {
                //Update the claim value first
                Dictionary<string, string> claimValues = new Dictionary<string, string>();
                claimValues.Add(AHP.Core.ClaimTypes.MustChangePassword, bool.FalseString);
                claimValues.Add(AHP.Core.ClaimTypes.PasswordExpired, bool.FalseString);
                _authManager.UpdateClaim(Request, claimValues);
                if (Identity.MustSelectSecurityQuestions)
                {
                    //send to select security questions
                    return RedirectToAction("SelectQuestions", "SetupUser");
                }
                else
                {
                    //redirect to customer home page
                    return RedirectToAction("Home", "Customer");
                }
            }
            else
            {
                //send back view with the error
                foreach (string errMsg in changePwdResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, errMsg);
                }
                if (updatePwdInfo != null)
                {
                    updatePwdInfo.NewPassword = updatePwdInfo.OldPassword = updatePwdInfo.ConfirmPassword = string.Empty;
                }
                return View(updatePwdInfo);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SelectQuestions(ViewModel.SecurityQuestionsViewModel usrQuestionInfo)
        {
            try
            {
                List<string> securityQuestionList = new List<string>();
                _logMessages.AppendFormat("Setting up user security questions for user {0}.", Identity.UserName);

                //getting list of security questions
                _logMessages.Append("Getting security question list.");
                securityQuestionList = _restClient.GetSecurityQuestionList();

                usrQuestionInfo.SecurityQuestions = securityQuestionList;
                if (ModelState.IsValid)
                {
                    List<string> selectedSecQuestions = new List<string>();
                    selectedSecQuestions.Add(usrQuestionInfo.PrimarySelectedQuestion.Trim().ToLower());
                    selectedSecQuestions.Add(usrQuestionInfo.SecondarySelectedQuestion.Trim().ToLower());
                    selectedSecQuestions.Add(usrQuestionInfo.ThirdSelectedQuestion.Trim().ToLower());

                    //do a distinct on the selected questions and check if there are 3 unique questions
                    if(selectedSecQuestions.Distinct().Count() != 3)
                    {
                        ModelState.AddModelError(string.Empty, "Please select unique security questions.");
                        return View(usrQuestionInfo);
                    }

                    if (string.IsNullOrEmpty(usrQuestionInfo.PrimaryProvidedAnswer)
                        || string.IsNullOrEmpty(usrQuestionInfo.SecondaryProvidedAnswer) 
                        || string.IsNullOrEmpty(usrQuestionInfo.ThirdProvidedAnswer))
                    {
                        ModelState.AddModelError(string.Empty, "Please provide answers to your security questions");
                        return View(usrQuestionInfo);
                    }

                    List<AHP.Core.DTO.UserSecurityOption> selectedQuestions = new List<Core.DTO.UserSecurityOption>();
                    selectedQuestions.Add(new Core.DTO.UserSecurityOption()
                    {
                        Answer = usrQuestionInfo.PrimaryProvidedAnswer,
                        Question = usrQuestionInfo.PrimarySelectedQuestion
                    });
                    selectedQuestions.Add(new Core.DTO.UserSecurityOption()
                    {
                        Answer = usrQuestionInfo.SecondaryProvidedAnswer,
                        Question = usrQuestionInfo.SecondarySelectedQuestion
                    });
                    selectedQuestions.Add(new Core.DTO.UserSecurityOption()
                    {
                        Answer = usrQuestionInfo.ThirdProvidedAnswer,
                        Question = usrQuestionInfo.ThirdSelectedQuestion
                    });

                    GenericAjaxResponse<bool> response = _restClient.SetSecurityQuestionsForUser(Identity.UserName, selectedQuestions);

                    if (response.Success && response.Data)
                    {
                        Dictionary<string, string> claimValues = new Dictionary<string, string>();
                        claimValues.Add(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, bool.FalseString);
                        //Update the claim value
                        _authManager.UpdateClaim(Request, claimValues);

                        //redirect to customer home pae
                        return RedirectToAction("Home", "Customer");
                    }
                    ModelState.AddModelError(string.Empty, response.Errors[0]);
                    return View(usrQuestionInfo);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                _logMessages.AppendFormat("Exception occurred updating security questions. Exception info {0}.", ex.Message);
            }
            _logger.Info(_logMessages.ToString());
            //show the view if it has come till here
            return View(usrQuestionInfo);
        }

        [HttpGet]
        public ActionResult SelectQuestions()
        {
            ViewModel.SecurityQuestionsViewModel usrQuestionInfo = new ViewModel.SecurityQuestionsViewModel();
            try
            {
                if (!Identity.MustSelectSecurityQuestions)
                {
                    return RedirectToAction("Home", "Customer");
                }
                List<string> securityQuestionList = new List<string>();
                _logMessages.AppendFormat("Setting up security questions for user");
                usrQuestionInfo.SecurityQuestions = _restClient.GetSecurityQuestionList();
                return View(usrQuestionInfo);
            }
            catch (Exception ex)
            {
                return View(usrQuestionInfo);
            }
        }


        #endregion

    }
}