using AHP.Core;
using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Web.Helpers;
using AHP.Web.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace AHP.Web.Controllers
{
    [AllowAnonymous]
    /// <summary>
    /// This controller is hit when user clicks Need Help link on footer.
    /// It should be accessible anonymously and hence inherits directly from Controller.
    /// </summary>
    public class HelpController : BaseController
    {

        #region -- Members --

        private readonly IServerDataRestClient _restClient;

        private readonly StringBuilder _logMessage;

        private readonly IActiveAnalyticsLogger _logger;

        #endregion

        #region -- Constructors --

        public HelpController(IActiveAnalyticsLogger logger,IServerDataRestClient restClient)
        {
            _restClient = restClient;
            _logger = logger;
            _logMessage = new StringBuilder();
        }

        #endregion

        #region -- Action methods --

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NeedHelp(CustomerViewModel customerVM)
        {
            bool success = false;
            string message = string.Empty;
            string homeUrl = string.Empty;

            try
            {
                _logMessage.Append("Reached the Need Help section.");
                if (!ModelState.IsValid)
                {
                    message = "Please enter all required data correctly!";
                    _logMessage.Append("All required fields werent provided");
                }
                else
                {
                    Customer customer = new Customer
                    {
                        SelectedIssue = customerVM.SelectedIssueId == 0 ? "Reset Password" : EnumHelper.GetDescription((AccountIssues)customerVM.SelectedIssueId),
                        Email = customerVM.Email,
                        FirstName = customerVM.FirstName,
                        LastName = customerVM.LastName,
                        Company = customerVM.Company,
                        PhoneNumber = customerVM.PhoneNumber,
                        IssueDescription = customerVM.IssueDescription
                    };

                    _logMessage.Append("Sending email with customer request details.");
                    bool wasMailSentSuccessfully = _restClient.PostNeedHelpMessage(customer);

                    if (wasMailSentSuccessfully)
                    {
                        _logMessage.Append("Mail has been sent successfully.");
                        success = true;
                        homeUrl = @Url.Action("Home", "Customer");
                        if (customerVM.SelectedIssueId == 0)
                            message = "Your message has been sent. Password reset requests will be handled by AHM Support within 24 business hours.";
                        else
                            message = "Your message has been sent.";
                    }
                    else
                    {
                        _logMessage.Append("Email could not be sent. An error occurred in PostNeedHelpMessage method.");
                        success = false;
                        message = "Message couldn't be sent! Please try again!";
                    }
                }
            }
            catch (Exception ex)
            {
                _logMessage.Append("Error occurred requesting help error message is "+ ex.Message + ".");
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessage.ToString());
            Models.AjaxResponse response = new Models.AjaxResponse();
            response.Success = success;
            response.Message = message;
            response.HomeUrl = homeUrl;
            return Json(response);
        }

        [HttpGet]
        public ActionResult NeedHelp(string id = "")
        {

            CustomerViewModel customerViewModel = new CustomerViewModel();
            IEnumerable<AccountIssues> issueTypes = EnumHelper.ToList<AccountIssues>();
            try
            {
                var ddlListItems = issueTypes.AsEnumerable().Select(x => new SelectListItem
                {
                    Text = EnumHelper.GetDescription(x),
                    Value = ((int)x).ToString()
                });

                if (!string.IsNullOrEmpty(id.Trim()))
                {
                    if (id == "1")
                    {
                        ViewBag.IssuesList = ddlListItems;
                        customerViewModel.SelectedIssueId = 6;
                    }
                }
                else
                {
                    ViewBag.IssuesList = ddlListItems.Where(x => x.Text != "Reset Password");
                }                
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            return View(customerViewModel);
        } 

        #endregion
    }
}