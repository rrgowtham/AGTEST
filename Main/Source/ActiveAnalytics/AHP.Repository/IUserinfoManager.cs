using AHP.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHP.Core.DTO;

namespace AHP.Repository
{
    public interface IUserinfoManager
    {

        /// <summary>
        /// Checks if the provided users security question matches as in the database
        /// </summary>
        /// <param name="username">user identity of the user to whom the update needs to be performed</param>
        /// <param name="birthYearMonth">Month and year of birth of the user in format MM/YYYY</param>
        /// <param name="zipCode">Zipcode of the users location</param>
        /// <param name="favTeacher">Favorite teacher for the user</param>
        /// <param name="favPlaceAsKid">Any favorite place for the user as a kid</param>
        /// <returns>True when validation was success, False otherwise</returns>
        //bool IsPersonalInfoValidationSuccess(string username, string birthYearMonth, string zipCode, string favTeacher, string favPlaceAsKid);

        /// <summary>
        /// Gets information about users email address and security question setup info
        /// </summary>
        /// <param name="username">name of the user</param>
        /// <returns>Instance of <see cref="UserSetupInfo"/> class</returns>
        //UserSetupInfo GetUserSetupInfo(string username);

        /// <summary>
        /// Performs an update of security question and answer for the user with name as <paramref name="username"/>
        /// </summary>
        /// <param name="username">user identity of the user to whom the update needs to be performed</param>
        /// <param name="birthYearMonth">Month and year of birth of the user in format MM/YYYY</param>
        /// <param name="zipCode">Zipcode of the users location</param>
        /// <param name="favTeacher">Favorite teacher for the user</param>
        /// <param name="favPlaceAsKid">Any favorite place for the user as a kid</param>
        /// <returns>True when updates have successfully been applied, False otherwise</returns>
        //bool SetupSecurityAnswers(string username,string birthYearMonth, string zipCode, string favTeacher, string favPlaceAsKid);

        ///// <summary>
        ///// Gets or sets the email address for the user
        ///// </summary>
        ///// <param name="username">username of the user</param>
        ///// <param name="emailAddress">email address of the user</param>
        ///// <param name="isEmailvalid">True when email has already been validated</param>
        ///// <returns>True when insert or update operation succeeded, Otherwise false</returns>
        //bool UpsertEmailAddress(string username,string emailAddress,bool isEmailvalid);

        /// <summary>
        /// Get's user's information from the data source
        /// </summary>
        /// <returns>List of all the users</returns>
        GenericResponse<List<AHP.Core.DTO.ExternalUserInfo>> ListUsers();        
        

        /// <summary>
        /// Creates a new user in the data source for external logon
        /// </summary>
        /// <param name="externalUser">Instance of <see cref="AHP.Core.DTO.ExternalUserInfo"/> class</param>
        /// <returns>True when successfully created new user, Otherwise false</returns>
        GenericResponse<bool> CreateUser(AHP.Core.DTO.ExternalUserInfo externalUser);

        /// <summary>
        /// Gets the external user information
        /// </summary>
        /// <param name="username">string representing usersname</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<AHP.Core.DTO.ExternalUserInfo> GetUserDetails(string username);

        /// <summary>
        /// Activates user account. Only activated user accounts can be logged into portal
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> UnlockUserAccount(string username, string email);

        /// <summary>
        /// Deactivates user account. Only activated user accounts can be logged into portal
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> LockUserAccount(string username, string email);

        /// <summary>
        /// Sends an welcome email to the user and if the email is delivered then email is activated
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <param name="activate">True to activate email, False otherwise</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> ActivateEmail(string username, string email,bool activate);

        /// <summary>
        /// Generates a random password as output and sets that as the password for the user
        /// </summary>
        /// <param name="username">Username of the user for whom the password needs to be reset</param>
        /// <param name="email">Email address of the user whom the password needs to be reset</param>
        /// <param name="changePwdOnLogon">True when force change password on next logon, False otherwise</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<string> ResetPassword(string username, string email,bool changePwdOnLogon);

        /// <summary>
        /// Gets user information if logon was successfull. Otherwise null and few errors
        /// </summary>
        /// <param name="username">String representing username of the user</param>
        /// <param name="password">String representing the raw password of the user</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<AHP.Core.DTO.ExternalUserInfo> Logon(string username, string password);

        /// <summary>
        /// Updates user information for the provided username and email address
        /// </summary>
        /// <param name="userInfo">Instance of <see cref="ExternalUserInfo;"/> class</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<ExternalUserInfo> UpdateUser(ExternalUserInfo userInfo);

        /// <summary>
        /// Activates user account. Only activated user accounts can be logged into portal
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> ActivateUser(string username, string email);

        /// <summary>
        /// Deactivates user account. Only activated user accounts can be logged into portal
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="email">email address of the user</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> DeactivateUser(string username, string email);

        /// <summary>
        /// Gets the user's security questions
        /// </summary>
        /// <param name="username">username of the user whom to get security questions</param>
        /// <returns>Instance Of <see cref="GenericResponse{T}"/> class with list of security questions</returns>
        GenericResponse<List<UserSecurityOption>> GetSecurityOptionsForUser(string username);

        /// <summary>
        /// Creates a new entry in personal info questions table for the user
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="securityQuestionAnswers">Instance of <see cref="List{AHP.Core.DTO.UserSecurityOption}"/> class</param>
        /// <returns>Instance of <see cref="GenericResponse{bool}"/> class</returns>
        GenericResponse<bool> SetupQuestions(string username, List<AHP.Core.DTO.UserSecurityOption> securityQuestionAnswers);

        /// <summary>
        /// Resets the security question and answer for the user
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <returns>Instance of <see cref="GenericResponse{bool}"/> class</returns>
        GenericResponse<bool> ClearSecurityAnswers(string username);

        /// <summary>
        /// Updates user's password with the new password
        /// </summary>
        /// <param name="username">Username of the user to change password</param>
        /// <param name="oldPassword">Existing password of the user</param>
        /// <param name="newPassword">New password to update for the user</param>
        /// <returns>Instance of <see cref="GenericResponse{bool}"/> class</returns>
        GenericResponse<bool> ChangePassword(string username,string oldPassword, string newPassword);

        /// <summary>
        /// Gets all the security questions for the user
        /// </summary>
        /// <returns>Instance of <see cref="List{string}"/> class</returns>
        List<string> GetSecurityQuestions();

        /// <summary>
        /// Resets the users password and returns it in response if the provided security question and answer match as in file
        /// </summary>
        /// <param name="username">string representing the users unique name</param>
        /// <param name="securityQuestionAnswers">List of all the users question and answer</param>
        /// <returns>Instance Of <see cref="GenericResponse{string}"/> class</returns>
        GenericResponse<AHP.Core.DTO.PasswordResetResponse> ResetPassword(string username, List<AHP.Core.DTO.UserSecurityOption> securityQuestionAnswers);

        /// <summary>
        /// Gets list of all internal users
        /// </summary>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<List<InternalUserInfo>> ListAllInternalUsers();

        /// <summary>
        /// Maps active health ID to Tableau ID
        /// </summary>
        /// <param name="userInfo">Instance of <see cref="InternalUserInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericResponse{bool}"/> class</returns>
        GenericResponse<bool> MapInternalUser(InternalUserInfo userInfo);

        /// <summary>
        /// Updates active health ID to Tableau ID
        /// </summary>
        /// <param name="userInfo">Instance of <see cref="InternalUserInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericResponse{bool}"/> class</returns>
        GenericResponse<bool> UpdateInternalUser(InternalUserInfo userInfo);

        /// <summary>
        /// Gets all the tableau workbook view info
        /// </summary>
        /// <returns>Instance of <see cref="GenericResponse{List<TableauViewInfo>}"/> class</returns>
        GenericResponse<List<TableauViewInfo>> ListAllTableauViews();

        /// <summary>
        /// Add's new tableau view information
        /// </summary>
        /// <param name="tabInfo">Instance of <see cref="TableauViewInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> AddTableauInfo(TableauViewInfo tabInfo);

        /// <summary>
        /// Updates tableau workbook view information
        /// </summary>
        /// <param name="tabInfo">Instance of <see cref="TableauViewInfo"/> class</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> UpdateTableauInfo(TableauViewInfo tabInfo);

        /// <summary>
        /// Gets the list of all users associated with a tableau <paramref name="viewId"/>
        /// </summary>
        /// <param name="viewId">string guid representing a view id</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<List<TableauViewUserAssociation>> GetUsersForView(string viewId);

        /// <summary>
        /// Removes and updates all association between user and tableau view
        /// </summary>
        /// <param name="viewId">string representing guid for the view id</param>
        /// <param name="usrViewAssoc">List of all the user and view association</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<bool> UpdateUsersForView(string viewId, List<TableauViewUserAssociation> usrViewAssoc);

        /// <summary>
        /// Gets all the enabled views assigned to the user with <paramref name="username"/>
        /// </summary>
        /// <param name="username">string with valid username</param>
        /// <param name="usertype">string with valid usertype</param>
        /// <returns>Instance of <see cref="GenericResponse{T}"/> class</returns>
        GenericResponse<List<TableauViewInfo>> GetViewsOnUser(string username, string usertype);
    }
}