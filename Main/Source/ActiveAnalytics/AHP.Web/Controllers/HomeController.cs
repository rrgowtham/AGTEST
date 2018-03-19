using AHP.Web.Helpers;
using System.Web.Mvc;

namespace AHP.Web.Controllers
{
    public class HomeController : BaseController
    {
        /// <summary>
        /// Checks for User Type and Redirects to respective landing pages.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //if (Identity.IsInternalUser)
            //    return RedirectToAction("Home", "Admin");
            //else
            if(Identity != null)
                return RedirectToAction("Home", "Customer");
            return RedirectToAction("Login", "Default");
        }
    }
}