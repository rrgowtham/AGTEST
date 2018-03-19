using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AHP.Web.Helpers;
using System.Security.Claims;
using NWebsec.Mvc.HttpHeaders;

namespace AHP.Web.Controllers
{
    [XXssProtection(BlockMode = true, Policy = XXssProtectionPolicy.FilterEnabled)]
    [XFrameOptions(Policy = XFrameOptionsPolicy.SameOrigin)]
    public abstract class BaseController : Controller
    {
        private Identity _identity;
        public Identity Identity
        {
            get
            {
                //processing claims
                if (User != null && ((ClaimsIdentity)User.Identity) != null && User.Identity.IsAuthenticated)
                {
                    ClaimsIdentity userInfo = User.Identity as ClaimsIdentity;
                    if (userInfo.Claims.Any())
                    {
                        Claim userClaim;
                        _identity = new Identity(string.Empty, false);

                        //internal user
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.IsInternalUser, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.IsInternalUser = userClaim.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
                        }

                        //username
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(ClaimTypes.Name, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.UserName = userClaim.Value;
                        }

                        //firstname
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(ClaimTypes.GivenName, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.FirstName = userClaim.Value;
                        }

                        //lastname
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(ClaimTypes.Surname, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.LastName = userClaim.Value;
                        }

                        //displayname
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.DisplayName, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.DisplayName = userClaim.Value;
                        }

                        //logon token
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.LogonToken, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.SapToken = userClaim.Value;
                        }

                        //bo session id
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.BOSessionId, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.BOSessionId = userClaim.Value;
                        }

                        //bo serialized session id
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.BOSerializedSession, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.BOSerializedSessionId = userClaim.Value;
                        }

                        //must change password
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.MustChangePassword, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.MustChangePassword = userClaim.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
                        }

                        //must change security question
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.MustSelectSecurityQuestions = userClaim.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
                        }

                        //company
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.Company, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.Company = userClaim.Value;
                        }

                        //email
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.Email = userClaim.Value;
                        }

                        //role
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.Role = userClaim.Value;
                        }

                        //last logon date
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.LastLogonDate, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.LastLogonDate = userClaim.Value;
                        }

                        //tableau auth ticket
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.TableauAuthTicket, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.TableauAuthTicket = userClaim.Value;
                        }

                        //password expired indicator
                        userClaim = userInfo.Claims.FirstOrDefault(clm => clm.Type.Equals(AHP.Core.ClaimTypes.PasswordExpired, StringComparison.OrdinalIgnoreCase));
                        if (userClaim != null)
                        {
                            _identity.PasswordExpired = userClaim.Value == bool.TrueString;
                        }
                    }
                }
                return _identity;
            }
        }

        protected BaseController()
        {

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = filterContext.ActionDescriptor.ActionName;
                string allowedControllers = "Help,PasswordReset,Error,Account,SetupUser";

                //enforce password reset
                if (Identity != null)
                {
                    if ((Identity.MustChangePassword || Identity.PasswordExpired )&& allowedControllers.IndexOf(controllerName, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        filterContext.Result = RedirectToAction("ChangePassword", "SetupUser");
                    }
                    else if (Identity.MustSelectSecurityQuestions && allowedControllers.IndexOf(controllerName, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        filterContext.Result = RedirectToAction("SelectQuestions","SetupUser");
                    }
                    
                }
                else
                {
                    if ((controllerName == "Default" || controllerName == "Account") && (actionName == "Login"))
                    {
                        return;
                    }

                    if (Identity == null)
                    {
                        if ((controllerName == "PersonalInfo" || controllerName == "Help" || controllerName == "PasswordReset" || controllerName == "Error" || controllerName == "AccountRecovery"))
                        {
                            return;
                        }
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.HttpContext.Response.StatusCode = 401;
                        }
                        else
                        {
                            filterContext.Result = RedirectToAction("Login", "Default");
                        }
                    }
                }

                base.OnActionExecuting(filterContext);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
        }

    }
}