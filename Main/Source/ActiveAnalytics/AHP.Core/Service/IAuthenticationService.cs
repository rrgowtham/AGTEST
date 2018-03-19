using AHP.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Service
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authModel"></param>
        /// <returns></returns>
        bool Logoff(BOAuthentication authModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authModel"></param>
        /// <returns></returns>
        BOAuthentication AuthenticateUserAndGetToken(BOAuthentication authModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        bool IsLoggedOn(string sessionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordReset"></param>
        /// <returns></returns>
        string ResetPasswordAndRedirect(BOPasswordReset passwordReset);

        /// <summary>
        /// Gets the session information using Web services sdk from Web Intelligence server using the provided logon token
        /// </summary>
        /// <param name="token">Logon token obtained either using rest service sdk or web services sdk or .net SDK</param>
        /// <returns>Session information of the user</returns>
        GenericResponse<AHP.Core.Model.BOUserSessionInfo> GetUserSessionInfo(string token);

    }
}
