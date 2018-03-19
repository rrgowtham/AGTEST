using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AHP.Web.Helpers
{
    public class OwinAuthenticationManager : IOwinAuthenticationManager
    {
        public void SignIn(HttpRequestBase request, params ClaimsIdentity[] claims)
        {
            request.GetOwinContext().Authentication.SignIn(claims);
        }

        public void SignOut(HttpRequestBase request, params string[] authenticationTypes)
        {
            request.GetOwinContext().Authentication.SignOut(authenticationTypes);
        }

        public bool UpdateClaim(HttpRequestBase request, Dictionary<string,string> claimValues)
        {
            try
            {
                var AuthenticationManager = request.GetOwinContext().Authentication;
                var Identity = new ClaimsIdentity(((ClaimsPrincipal)request.RequestContext.HttpContext.User).Identity);

                foreach (var kValuePair in claimValues)
                {
                    //remove the claim
                    Identity.RemoveClaim(Identity.FindFirst(kValuePair.Key));
                    //add back with new value
                    Identity.AddClaim(new Claim(kValuePair.Key, kValuePair.Value));
                }                
                AuthenticationManager.AuthenticationResponseGrant = new Microsoft.Owin.Security.AuthenticationResponseGrant(new ClaimsPrincipal(Identity), new Microsoft.Owin.Security.AuthenticationProperties { IsPersistent = true });
                //logout and log back in quickly
                AuthenticationManager.SignOut(Identity.AuthenticationType);
                AuthenticationManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties() { IsPersistent = true }, Identity);
                return true;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                //eat any exceptions.
                return false;
            }
        }
    }
}