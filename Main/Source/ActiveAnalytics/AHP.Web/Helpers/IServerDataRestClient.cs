using AHP.Core.Model;
using AHP.Web.Api.Models;
using AHP.Web.Models;
using System.Collections.Generic;
using AHP.Core.DTO;
using AHP.Web.ViewModel;

namespace AHP.Web.Helpers
{
    public interface IServerDataRestClient
    {
        bool PostNeedHelpMessage(Customer customer);
        LDAPUser IsLDAPAuthenticated(string userame, string password);
        BOUser AuthenticateWithSAPAndGetToken(string username, string password, bool isInternalUser);
        
        bool LogoffFromSAP(string sapToken,string sessionId);

        ReportCategory GetReportList(string sapToken);
        string GetReport(string sapToken, string sessionId, string reportId);

        bool IsPersonalInfoValidated(PersonalInfoQuestion personalInfoQuestion);

        string ResetPasswordAndRedirect(BOPasswordReset passwordReset);

        /// <summary>
        /// Passes the session id to BO system to check if user is still logged on
        /// </summary>
        /// <param name="sessionId">string representing the users session id</param>
        /// <returns>True when user is logged on False otherwise</returns>
        bool IsLoggedOn(string sessionId);

        /// <summary>
        /// Gets all the users from the user store
        /// </summary>
        /// <returns>List of all the users</returns>
        GenericAjaxResponse<List<AHP.Core.DTO.ExternalUserInfo>> GetAllUsers();

        /// <summary>
        /// Creates a user account for logon with portal
        /// </summary>
        /// <param name="userInfo">Instance of <see cref="ExternalUserInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> CreateUser(ExternalUserInfo userInfo);        

        /// <summary>
        /// Gets the users information with a user whose name is matched
        /// </summary>
        /// <param name="username">String representing the name of the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo> GetUserDetails(string username);

        /// <summary>
        /// Unlocks the user account so user can logon
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> UnlockUserAccount(string username, string email);

        /// <summary>
        /// Locks the user account so user cannot logon
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> LockUser(string username, string email);

        /// <summary>
        /// Activates email address for the user
        /// </summary>
        /// <param name="username">string representing username of the user</param>
        /// <param name="email">string representing email of the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> ActivateEmail(string username, string email);

        /// <summary>
        /// Reset's the password if the user has valid account in the portal
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <param name="changePwdOnLogon">True when you want user to be forced to change password on logon, Otherwise false</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> ResetPassword(string username, string email,bool changePwdOnLogon);

        /// <summary>
        /// Gets tableau account name for internal user with <paramref name="userName"/>
        /// </summary>
        /// <param name="userName">string representing username of the internal user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<string> GetTableauAccountname(string userName);

        /// <summary>
        /// Updates user information with the provided data
        /// </summary>
        /// <param name="externalUserAccount">Information on user which needs to be updated</param>
        /// <returns>Instance of <see cref="ExternalUserInfo"/> class</returns>
        GenericAjaxResponse<ExternalUserInfo> UpdateUser(ExternalUserInfo externalUserAccount);

        /// <summary>
        /// Activates the user's account
        /// </summary>
        /// <param name="username">username of the account holder</param>
        /// <param name="email">email of the account holder</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> ActivateUser(string username, string email);        

        /// <summary>
        /// Deactivates the user's account
        /// </summary>
        /// <param name="username">username of the account holder</param>
        /// <param name="email">email of the account holder</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> DeactivateUser(string username, string email);

        /// <summary>
        /// Authenticates user credentials against database
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="password">password of the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo> Login(string username, string password);

        /// <summary>
        /// Authenticates with a REST service to get token, then send that token to Web services sdk to get the user session information
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<BOUserSessionInfo> LogonToWebIntelligence(string username);

        /// <summary>
        /// Gets the security questions for the user
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<List<AHP.Core.DTO.UserSecurityOption>> GetSecurityQuestionsForUser(string username);

        /// <summary>
        /// Updates the password for the user with provided username
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="oldPassword">Old password of the user</param>
        /// <param name="newPassword">New password that needs to be updated for the user</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{bool}"/> class</returns>
        GenericAjaxResponse<bool> ChangePassword(string username,string oldPassword, string newPassword);

        /// <summary>
        /// Gets the list of security questions
        /// </summary>
        /// <returns><see cref="List{string}"/> class</returns>
        List<string> GetSecurityQuestionList();

        /// <summary>
        /// Updates the security question for the user
        /// </summary>
        /// <param name="userName">string representing username of the user</param>
        /// <param name="selectedQuestions">List of all selected questions and answers</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{bool}"/> class</returns>
        GenericAjaxResponse<bool> SetSecurityQuestionsForUser(string userName, List<UserSecurityOption> selectedQuestions);

        /// <summary>
        /// Resets any security questions and answer already setup for the user with username as <paramref name="username"/>
        /// </summary>
        /// <param name="username">username of the user to reset the security questions</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{bool}"/> class</returns>
        GenericAjaxResponse<bool> RemoveSecurityQuestions(string username);

        /// <summary>
        /// Resets the password for the user when the provided security question and answer are right
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="securityQuestionAnswers">Security question and answer</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{bool}"/> class</returns>
        GenericAjaxResponse<bool> ResetPassword(string username, List<Core.DTO.UserSecurityOption> securityQuestionAnswers);

        /// <summary>
        /// Gets a list of all internal users with their tableau account id mapping
        /// </summary>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/></returns>
        GenericAjaxResponse<List<AHP.Core.DTO.InternalUserInfo>> GetInternalUsers();

        /// <summary>
        /// Creates a new record in database for mapping active health id and tableau ID
        /// </summary>
        /// <param name="userInfo">instance of <see cref="InternalUserInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{bool}"/> class</returns>
        GenericAjaxResponse<bool> MapInternalUser(InternalUserInfo userInfo);

        /// <summary>
        /// Updates existing record in database for mapping active health id and tableau ID
        /// </summary>
        /// <param name="userInfo">instance of <see cref="InternalUserInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{bool}"/> class</returns>
        GenericAjaxResponse<bool> UpdateInternalUser(InternalUserInfo userInfo);

        /// <summary>
        /// Gets all the views in the site in tableau
        /// </summary>
        /// <returns>Instance of <see cref="GenericAjaxResponse{List<TableauViewInfo>}"/> class</returns>
        GenericAjaxResponse<List<TableauViewInfo>> GetTableauViews();

        /// <summary>
        /// Inserts the tableau view information to the database
        /// </summary>
        /// <param name="tabInfo">Instance of <see cref="TableauViewInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> AddTableauView(TableauViewInfo tabInfo);

        /// <summary>
        /// Updates the tableau view information to the database
        /// </summary>
        /// <param name="tabInfo">Instance of <see cref="TableauViewInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> UpdateTableauView(TableauViewInfo tabInfo);

        /// <summary>
        /// Lists all the users who are associated with the <paramref name="viewId"/>
        /// </summary>
        /// <param name="viewId">ViewId whose tableau user association needs to be retrieved</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<List<Core.DTO.TableauViewUserAssociation>> ListViewAssociations(string viewId);

        /// <summary>
        /// Updates the user and view association
        /// </summary>
        /// <param name="viewId">Guid representing view id</param>
        /// <param name="userViewInfo">List of all the user and informatin on which view they need to be associated with</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<bool> UpdateTableauUserAssociation(string viewId,List<Core.DTO.TableauViewUserAssociation> userViewInfo);

        /// <summary>
        /// Lists all the views user has access to view
        /// </summary>
        /// <param name="username">string containing the username of the user</param>
        /// <param name="userType">INTERNAL OR EXTERNAL</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericAjaxResponse<List<AHP.Core.DTO.TableauViewInfo>> GetUsersViews(string username, string userType);
    }
}