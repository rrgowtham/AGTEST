using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AHP.Web.ViewModel;
using AHP.Core.Model;
using AHP.Web.Helpers;
using AHP.Core.Logger;
using System.Text;

namespace AHP.Web.Controllers
{
    [AllowAnonymous]
    public class PersonalInfoController : BaseController
    {

        #region -- Members --

        private readonly IServerDataRestClient _restClient;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessage;

        #endregion

        #region -- Constructors --

        public PersonalInfoController(IActiveAnalyticsLogger logger,IServerDataRestClient restClient)
        {
            _restClient = restClient;
            _logger = logger;
            _logMessage = new StringBuilder();
        }

        #endregion

        #region -- Action Methods --
        [HttpGet]
        public ActionResult Home()
        {
            PersonalInfoViewModel personalInfoVM = new PersonalInfoViewModel();
            return View(personalInfoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Home(PersonalInfoViewModel personalInfoVM)
        {
            bool success = false;
            string message = "";
            string homeUrl = "";
            bool isValidateSuccess = false;

            try
            {
                _logMessage.Append("Requesting password reset for External user.");
                if (!ModelState.IsValid)
                {
                    message = "Please enter all required data correctly!";
                }
                else
                {
                    _logMessage.Append("Field validation successfull.");
                    PersonalInfoQuestion personalInfo = new PersonalInfoQuestion
                    {
                        UserName = personalInfoVM.UserName,
                        MonthYear = personalInfoVM.MonthYear,
                        ZipCode = personalInfoVM.ZipCode,
                        FavTeacher = personalInfoVM.FavTeacher,
                        FavPlaceAsChild = personalInfoVM.FavPlaceAsChild
                    };

                    _logMessage.Append("Invoking rest service.");
                    isValidateSuccess = _restClient.IsPersonalInfoValidated(personalInfo);

                    if (isValidateSuccess)
                    {
                        _logMessage.Append("Password reset successfull.");
                        success = true;
                        homeUrl = @Url.Action("NeedHelp", "Help", new { id = 1 });
                    }
                    else
                    {
                        _logMessage.Append("Password reset unsuccessfull");
                        success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logMessage.Append("Error occurred "+ ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessage.ToString());            
            return Json(new { Success = success, HomeUrl = homeUrl, Message = message });
        } 
        #endregion
    }
}