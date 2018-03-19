using AHP.Web.Helpers;
using AHP.Web.ViewModel;
using System.Web.Mvc;
using System;
using AHP.Core.Logger;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using AHP.Web.Models;
using System.Collections.Generic;

namespace AHP.Web.Controllers
{
    [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, NoStore = true)]
    public class CustomerController : BaseController
    {

        #region -- Members --

        private readonly IServerDataRestClient _restClient;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessage;

        private readonly IOwinAuthenticationManager _authManager;

        #endregion

        #region -- Constructors --

        public CustomerController(IActiveAnalyticsLogger logger,
            IServerDataRestClient restClient,
            IOwinAuthenticationManager authManager)
        {
            _restClient = restClient;
            _logger = logger;
            _logMessage = new StringBuilder();
            _authManager = authManager;
        }

        #endregion

        #region -- Action Methods --

        public ActionResult Home()
        {
            ReportViewModel reportVM = new ReportViewModel();
            try
            {
                _logMessage.Append("Getting reports list using sap token Customer Controller Home Method.");
                ViewBag.IsInternalUser = Identity.IsInternalUser;

                if (Identity != null && !string.IsNullOrEmpty(Identity.SapToken))
                {
                    reportVM.Category = _restClient.GetReportList(Identity.SapToken);
                    _logMessage.Append("Identity and Sap token are present. Sap Token passed is " + Identity.SapToken + ".");
                }
                else
                {
                    _logMessage.Append("Identity or Sap Token is missing");
                }

                if (Identity != null)
                {
                    //get all tableau views
                    GenericAjaxResponse<List<AHP.Core.DTO.TableauViewInfo>> apiResponse = _restClient.GetUsersViews(Identity.UserName, Identity.IsInternalUser ? "INTERNAL" : "EXTERNAL");
                    if (apiResponse.Success && apiResponse.Data != null)
                    {
                        List<Web.ViewModel.TableauWorkbookViewModel> uiResponse = apiResponse.Data.Select(dto => new Web.ViewModel.TableauWorkbookViewModel()
                        {
                            Description = dto.Description,
                            ShortDescription = dto.Description.Substring(0, Math.Min(136, dto.Description.Length)),
                            Disabled = dto.Disabled,
                            IsDashboard = dto.IsDashboard,
                            ShortName = dto.ViewName.Substring(0, Math.Min(51, dto.ViewName.Length)),
                            ViewId = dto.ViewId,
                            ViewName = dto.ViewName,
                            ViewUrl = dto.ViewUrl
                        }).ToList();

                        ViewBag.TableauViews = uiResponse.
                            Where(vw => vw.IsDashboard.Equals("N", StringComparison.OrdinalIgnoreCase) && vw.Disabled.Equals("N", StringComparison.OrdinalIgnoreCase)).
                            ToList();
                        ViewBag.TableauDashboards = uiResponse.Where(vw => vw.IsDashboard.Equals("Y", StringComparison.OrdinalIgnoreCase) && vw.Disabled.Equals("N", StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                _logMessage.Append("An error occurred getting reports. Exception information " + ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessage.ToString());
            return View(reportVM);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewModel.PasswordResetViewModel model = new ViewModel.PasswordResetViewModel();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(ViewModel.PasswordResetViewModel updatePwdInfo)
        {
            GenericAjaxResponse<bool> changePwdResponse = new GenericAjaxResponse<bool>();
            _logMessage.AppendFormat("Password change requested for user {0}.", Identity.UserName);
            try
            {
                if (ModelState.IsValid)
                {
                    _logMessage.Append("Model validation successfull. Sending password details to service to change password.");
                    changePwdResponse = _restClient.ChangePassword(Identity.UserName, updatePwdInfo.OldPassword, updatePwdInfo.NewPassword);
                }
            }
            catch (Exception ex)
            {
                _logMessage.AppendFormat("Change password ended with error. Exception message is {0}", ex.Message);
            }
            _logger.Info(_logMessage.ToString());
            if (changePwdResponse.Success)
            {
                //Update the claim value first
                Dictionary<string, string> claimValues = new Dictionary<string, string>();
                claimValues.Add(AHP.Core.ClaimTypes.MustChangePassword, bool.FalseString);
                claimValues.Add(AHP.Core.ClaimTypes.PasswordExpired, bool.FalseString);
                //redirect to customer home page
                return RedirectToAction("Home", "Customer");
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


        #endregion

    }
}