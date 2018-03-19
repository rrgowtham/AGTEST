using AHP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AHP.Web.Controllers
{
    [AllowAnonymous]
    public class DefaultController : BaseController
    {
        #region -- Members --
        private Core.Logger.IActiveAnalyticsLogger _logger;
        private readonly StringBuilder _logInfoBuilder;
        #endregion

        public DefaultController(Core.Logger.IActiveAnalyticsLogger logger)
        {
            _logger = logger;
            _logInfoBuilder = new StringBuilder();
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string id = "")
        {
            try
            {
                _logInfoBuilder.Append("Checking if user is logged on in default controller.");
                if (User.Identity.IsAuthenticated)
                {
                    _logInfoBuilder.Append("Session information exists. Redirecting user to proper page.");
                    RedirectToRouteResult response;
                    //Identity identity = Session["Identity"] as Identity;
                    if (Identity.IsInternalUser)
                    {
                        _logInfoBuilder.Append("User is internal user redirecting to Customer controller , Home Method.");
                    }
                    else
                    {
                        _logInfoBuilder.Append("User is External user redirecting to Customer controller , Home Method.");                       
                    }
                    response = RedirectToAction("Home", "Customer");
                    _logger.Info(_logInfoBuilder.ToString());
                    return response;
                }
                _logInfoBuilder.Append("User information not present in session. Go to login page");
            }
            catch (Exception ex)
            {
                _logInfoBuilder.Append("An error occurred Default Controller, Login method "+ ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logInfoBuilder.ToString());
            ViewBag.Message = HttpUtility.HtmlEncode(id);
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "Logged Out";
            }
            else
            {
                ViewBag.Title = "Active Analytics - Login";
            }
            //If we have come this far, session must be cleared too
            //Session.Clear();
            return View();
        }
    }
}