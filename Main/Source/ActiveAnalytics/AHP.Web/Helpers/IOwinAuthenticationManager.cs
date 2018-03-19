using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Web.Helpers
{
    public interface IOwinAuthenticationManager
    {
        void SignIn(System.Web.HttpRequestBase request, params System.Security.Claims.ClaimsIdentity[] claims);

        void SignOut(System.Web.HttpRequestBase request, params string[] authenticationTypes);

        /// <summary>
        /// Updates the claim and its values in the request
        /// </summary>
        /// <param name="request">Http request</param>
        /// <param name="claimValues">Dictionary of claim names and values</param>
        /// <returns>If the update succeeeded</returns>
        bool UpdateClaim(System.Web.HttpRequestBase request,Dictionary<string,string> claimValues);
    }
}
