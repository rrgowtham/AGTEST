using AHP.Core.Model;
using AHP.Web.Api.Models;
using AHP.Web.Models;
using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System;
using AHP.Core.DTO;

namespace AHP.Web.Helpers
{
    public class ServerDataRestClient : IServerDataRestClient
    {
        private readonly IRestClient _restClient;

        public ServerDataRestClient(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public bool PostNeedHelpMessage(Customer customer)
        {
            var request = new RestRequest("api/needhelp", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(customer);
            var response = _restClient.Execute<bool>(request);

            return response.Data;
        }

        public LDAPUser IsLDAPAuthenticated(string username, string password)
        {
            var request = new RestRequest("api/LDAPAccount", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(new UserCredential(){ Username = username, Password = password});
            var response = _restClient.Execute<LDAPUser>(request);

            return response.Data;
        }

        //public BOUser AuthenticateWithSAPAndGetToken(string username, string password, bool isInternalUser)
        //{
        //    var request = new RestRequest("api/BOAccount", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddBody(new UserCredential() { Username = username, Password = password, IsInternalUser = isInternalUser });
        //    var response = _restClient.Execute<BOUser>(request);
        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        return response.Data;
        //    }
        //    return null;
        //}

        public BOUser AuthenticateWithSAPAndGetToken(string username, string password, bool isInternalUser)
        {
            var request = new RestRequest("api/BOAccount", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(new UserCredential() { Username = username, Password = password, IsInternalUser = isInternalUser });
            var response = _restClient.Execute<BOUser>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }
            return null;
        }

        public bool LogoffFromSAP(string sapToken,string sessionId)
        {
            var request = new RestRequest("api/BOAccount", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("authToken", sapToken);
            request.AddParameter("sessionId",sessionId);
            var response = _restClient.Execute(request);
            bool loggedOff;
            bool.TryParse(response.Content,out loggedOff);
            return loggedOff;
        }

        public ReportCategory GetReportList(string sapToken)
        {
            var request = new RestRequest("api/WebiReports", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sapToken", sapToken);
            var response = _restClient.Execute<ReportCategory>(request);
            return response.Data;
        }

        /// <summary>
        /// Returns the report in html format
        /// </summary>
        /// <param name="sapToken"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public string GetReport(string sapToken, string sessionId, string reportId)
        {
            var request = new RestRequest("api/WebiReports/" + reportId, Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sapToken", sapToken);
            request.AddParameter("sessionId", sessionId);
            var response = _restClient.Execute<List<string>>(request);
            return response.Data.First();
        }


       public bool IsPersonalInfoValidated(PersonalInfoQuestion personalInfoQuestion)
        {
            var request = new RestRequest("api/PersonalInfo", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(personalInfoQuestion);
            var response = _restClient.Execute<bool>(request);

            return response.Data;
        }

        public string ResetPasswordAndRedirect(BOPasswordReset passwordReset)
        {
            var request = new RestRequest("api/PasswordReset", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(passwordReset);
            var response = _restClient.Execute<BOPWReset>(request);

            return response.Data.Message;
            
        }

        public bool IsLoggedOn(string sessionId)
        {
            var request = new RestRequest("api/PersonalInfo", Method.GET);
            request.AddParameter("sessionId",sessionId);
            var response = _restClient.Execute<bool>(request);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                return response.Data;
            }
            else
            {
                return false;
            }
        }

        public GenericAjaxResponse<List<ExternalUserInfo>> GetAllUsers()
        {
            var request = new RestRequest("api/Userinfo/ListUsers", Method.GET);            
            var response = _restClient.Execute<GenericResponse<List<AHP.Core.DTO.ExternalUserInfo>>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                GenericAjaxResponse<List<ExternalUserInfo>> apiResponse = new GenericAjaxResponse<List<ExternalUserInfo>>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> CreateUser(ExternalUserInfo userInfo)
        {
            var request = new RestRequest("api/Userinfo/CreateUser", Method.POST);
            request.RequestFormat = DataFormat.Json;          
            request.AddJsonBody(userInfo);            
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<ExternalUserInfo> GetUserDetails(string username)
        {
            var request = new RestRequest("api/Userinfo/UserDetails", Method.POST);
            request.AddQueryParameter("username",username);
            var response = _restClient.Execute<GenericResponse<ExternalUserInfo>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<ExternalUserInfo> apiResponse = new GenericAjaxResponse<ExternalUserInfo>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> UnlockUserAccount(string username, string email)
        {
            var request = new RestRequest("api/Userinfo/UnlockUser", Method.POST);
            //request.RequestFormat = DataFormat.Json;
            request.AddQueryParameter("username", username);
            request.AddQueryParameter("email", email);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> LockUser(string username, string email)
        {
            var request = new RestRequest("api/Userinfo/LockUser", Method.POST);
            //request.RequestFormat = DataFormat.Json;
            request.AddQueryParameter("username", username);
            request.AddQueryParameter("email", email);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> ActivateEmail(string username, string email)
        {
            var request = new RestRequest("api/Userinfo/ActivateEmail", Method.POST);
            request.AddQueryParameter("username", username);
            request.AddQueryParameter("email", email);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> ResetPassword(string username, string email,bool changePasswordOnLogon)
        {
            var request = new RestRequest("api/Userinfo/ResetPassword", Method.POST);
            request.AddQueryParameter("username", username);
            request.AddQueryParameter("email", email);
            request.AddQueryParameter("changePwdOnLogon", changePasswordOnLogon.ToString());
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<ExternalUserInfo> UpdateUser(ExternalUserInfo externalUserAccount)
        {
            var request = new RestRequest("api/Userinfo/UpdateUser", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(externalUserAccount);
            var response = _restClient.Execute<GenericResponse<ExternalUserInfo>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<ExternalUserInfo> apiResponse = new GenericAjaxResponse<ExternalUserInfo>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> ActivateUser(string username, string email)
        {
            var request = new RestRequest("api/Userinfo/ActivateUser", Method.POST);
            request.AddQueryParameter("username", username);
            request.AddQueryParameter("email", email);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> DeactivateUser(string username, string email)
        {
            var request = new RestRequest("api/Userinfo/DeactivateUser", Method.POST);
            request.AddQueryParameter("username", username);
            request.AddQueryParameter("email", email);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo> Login(string username,string password)
        {
            var request = new RestRequest("api/Userinfo/Login", Method.POST);
            request.AddQueryParameter("username", username);
            request.AddQueryParameter("password", password);
            var response = _restClient.Execute<GenericResponse<ExternalUserInfo>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<ExternalUserInfo> apiResponse = new GenericAjaxResponse<ExternalUserInfo>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<BOUserSessionInfo> LogonToWebIntelligence(string username)
        {
            GenericAjaxResponse<BOUserSessionInfo> response = new GenericAjaxResponse<BOUserSessionInfo>();
            var request = new RestRequest("api/Userinfo/SAPLogon", Method.POST);
            request.AddQueryParameter("username", username);
            var apiResponse = _restClient.Execute<GenericResponse<BOUserSessionInfo>>(request);
            if (apiResponse.ResponseStatus == ResponseStatus.Completed && apiResponse.Data != null)
            {
                response.Success = apiResponse.Data.Success;
                response.Data = apiResponse.Data.Data;
                response.Errors = apiResponse.Data.Errors;
                return response;
            }
            else
            {
                response.Errors.Add("Could not retrieve user session information. Please try again.");
                response.Success = false;
                return response;
            }   
        }

        public GenericAjaxResponse<List<UserSecurityOption>> GetSecurityQuestionsForUser(string username)
        {
            GenericAjaxResponse<List<UserSecurityOption>> response = new GenericAjaxResponse<List<UserSecurityOption>>();
            try
            {
                var request = new RestRequest("api/Userinfo/GetQuestionsForUser", Method.POST);
                request.AddQueryParameter("username", username);
                var restResponse = _restClient.Execute<GenericResponse<List<AHP.Core.DTO.UserSecurityOption>>>(request);
                if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.Data != null)
                {                    
                    response.Success = restResponse.Data.Success;
                    response.Data = restResponse.Data.Data;
                    response.Errors = restResponse.Data.Errors;
                    return response;
                }
                else
                {
                    response.Success = false;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("An error occurred. Please try again.");
            }
            return response;
        }

        public GenericAjaxResponse<bool> ChangePassword(string username, string oldPassword, string newPassword)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                RestRequest restRequest = new RestRequest("api/Userinfo/ChangePassword", Method.POST);
                restRequest.AddQueryParameter("username", username);
                restRequest.AddQueryParameter("oldPassword", oldPassword);
                restRequest.AddQueryParameter("newPassword", newPassword);
                IRestResponse<GenericResponse<bool>> restResponse = _restClient.Execute<GenericResponse<bool>>(restRequest);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.Success = restResponse.Data.Success;
                    response.Data = restResponse.Data.Data;
                    response.Errors = restResponse.Data.Errors;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add("Error occurred updating your password. Please try again");
                response.Success = false;
                response.Data = false;
            }
            return response;
        }

        public List<string> GetSecurityQuestionList()
        {
            List<string> response = new List<string>();
            try
            {
                RestRequest restRequest = new RestRequest("api/Userinfo/GetSecurityQuestion", Method.POST);                
                IRestResponse<List<string>> restResponse = _restClient.Execute<List<string>>(restRequest);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response = restResponse.Data;
                }
            }
            catch (Exception ex)
            {
                
            }
            return response;
        }

        public GenericAjaxResponse<bool> SetSecurityQuestionsForUser(string userName, List<UserSecurityOption> selectedQuestions)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            RestRequest restRequest = new RestRequest("api/Userinfo/UpdateSecurityQuestions", Method.POST);
            UpdateSecurityQuestionsRequest apiRequest = new UpdateSecurityQuestionsRequest()
            {
                Username = userName,
                SecurityQuestions = selectedQuestions
            };
            restRequest.AddJsonBody(apiRequest);
            IRestResponse<GenericResponse<bool>> restResponse = _restClient.Execute<GenericResponse<bool>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                response.Success = restResponse.Data.Success;
                response.Data = restResponse.Data.Data;
                response.Errors = restResponse.Data.Errors;
            }
            return response;
        }

        public GenericAjaxResponse<bool> RemoveSecurityQuestions(string username)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            RestRequest restRequest = new RestRequest("api/Userinfo/RemoveQuestions", Method.POST);
            restRequest.AddParameter("username", username);            
            IRestResponse<GenericResponse<bool>> restResponse = _restClient.Execute<GenericResponse<bool>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                response.Success = restResponse.Data.Success;
                response.Data = restResponse.Data.Data;
                response.Errors = restResponse.Data.Errors;
            }
            return response;
        }

        public GenericAjaxResponse<bool> ResetPassword(string username, List<UserSecurityOption> securityQuestionAnswers)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            RestRequest restRequest = new RestRequest("api/Userinfo/ValidateQuestions", Method.POST);
            UpdateSecurityQuestionsRequest apiRequest = new UpdateSecurityQuestionsRequest()
            {
                Username = username,
                SecurityQuestions = securityQuestionAnswers
            };
            restRequest.AddJsonBody(apiRequest);            
            IRestResponse<GenericResponse<bool>> restResponse = _restClient.Execute<GenericResponse<bool>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (restResponse.Data != null)
                {
                    response.Success = restResponse.Data.Success;
                    response.Data = restResponse.Data.Data;
                    response.Errors = restResponse.Data.Errors;
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add("Could not reset password. Please try again.");
                }                
            }
            return response;
        }

        public GenericAjaxResponse<List<InternalUserInfo>> GetInternalUsers()
        {
            GenericAjaxResponse<List<InternalUserInfo>> apiResponse = new GenericAjaxResponse<List<InternalUserInfo>>();
            RestRequest restRequest = new RestRequest("api/Userinfo/GetAllInternalUsers", Method.POST);
            IRestResponse<GenericResponse<List<InternalUserInfo>>> restResponse = _restClient.Execute<GenericResponse<List<InternalUserInfo>>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                apiResponse.Data = restResponse.Data.Data;
                apiResponse.Success = restResponse.Data.Success;
                apiResponse.Errors = restResponse.Data.Errors;
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not get list of users. Please try again.");
            }
            return apiResponse;
        }

        public GenericAjaxResponse<bool> MapInternalUser(InternalUserInfo userInfo)
        {
            var request = new RestRequest("api/Userinfo/MapInternalUser", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(userInfo);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> UpdateInternalUser(InternalUserInfo userInfo)
        {
            var request = new RestRequest("api/Userinfo/UpdateInternalUser", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(userInfo);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<List<TableauViewInfo>> GetTableauViews()
        {
            GenericAjaxResponse<List<TableauViewInfo>> apiResponse = new GenericAjaxResponse<List<TableauViewInfo>>();
            RestRequest restRequest = new RestRequest("api/Userinfo/GetAllTableauViews", Method.POST);
            IRestResponse<GenericResponse<List<TableauViewInfo>>> restResponse = _restClient.Execute<GenericResponse<List<TableauViewInfo>>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                apiResponse.Data = restResponse.Data.Data;
                apiResponse.Success = restResponse.Data.Success;
                apiResponse.Errors = restResponse.Data.Errors;
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not get list of tableau views. Please try again.");
            }
            return apiResponse;
        }

        public GenericAjaxResponse<bool> AddTableauView(TableauViewInfo tabInfo)
        {
            var request = new RestRequest("api/Userinfo/AddTableauInfo", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(tabInfo);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<bool> UpdateTableauView(TableauViewInfo tabInfo)
        {
            var request = new RestRequest("api/Userinfo/UpdateTableauInfo", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(tabInfo);
            var response = _restClient.Execute<GenericResponse<bool>>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.Data != null)
            {
                GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
                apiResponse.Success = response.Data.Success;
                apiResponse.Data = response.Data.Data;
                apiResponse.Errors = response.Data.Errors;
                return apiResponse;
            }
            else
            {
                return null;
            }
        }

        public GenericAjaxResponse<List<TableauViewUserAssociation>> ListViewAssociations(string viewId)
        {
            GenericAjaxResponse<List<TableauViewUserAssociation>> apiResponse = new GenericAjaxResponse<List<TableauViewUserAssociation>>();
            RestRequest restRequest = new RestRequest("api/Userinfo/GetAllViewAssociation", Method.POST);
            restRequest.AddQueryParameter("viewId", viewId);
            IRestResponse<GenericResponse<List<TableauViewUserAssociation>>> restResponse = _restClient.Execute<GenericResponse<List<TableauViewUserAssociation>>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                apiResponse.Data = restResponse.Data.Data;
                apiResponse.Success = restResponse.Data.Success;
                apiResponse.Errors = restResponse.Data.Errors;
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not get list of users. Please try again.");
            }
            return apiResponse;
        }

        public GenericAjaxResponse<bool> UpdateTableauUserAssociation(string viewId, List<TableauViewUserAssociation> userViewInfo)
        {
            GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
            RestRequest restRequest = new RestRequest("api/Userinfo/UpdateViewAssociation", Method.POST);
            restRequest.AddQueryParameter("viewId", viewId);
            restRequest.AddJsonBody(userViewInfo);
            IRestResponse<GenericResponse<bool>> restResponse = _restClient.Execute<GenericResponse<bool>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                apiResponse.Data = restResponse.Data.Data;
                apiResponse.Success = restResponse.Data.Success;
                apiResponse.Errors = restResponse.Data.Errors;
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not process your request");
            }
            return apiResponse;
        }

        public GenericAjaxResponse<List<TableauViewInfo>> GetUsersViews(string username, string userType)
        {
            GenericAjaxResponse<List<TableauViewInfo>> apiResponse = new GenericAjaxResponse<List<TableauViewInfo>>();
            RestRequest restRequest = new RestRequest("api/Userinfo/GetViewsForUser", Method.POST);
            restRequest.AddQueryParameter("username", username);
            restRequest.AddQueryParameter("usertype", userType);
            IRestResponse<GenericResponse<List<TableauViewInfo>>> restResponse = _restClient.Execute<GenericResponse<List<TableauViewInfo>>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                apiResponse.Data = restResponse.Data.Data;
                apiResponse.Success = restResponse.Data.Success;
                apiResponse.Errors = restResponse.Data.Errors;
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not process your request");
            }
            return apiResponse;
        }

        public GenericAjaxResponse<string> GetTableauAccountname(string userName)
        {
            GenericAjaxResponse<string> apiResponse = new GenericAjaxResponse<string>();
            RestRequest restRequest = new RestRequest("api/Userinfo/GetTableauAccountname", Method.POST);
            restRequest.AddQueryParameter("username", userName);
            IRestResponse<GenericResponse<string>> restResponse = _restClient.Execute<GenericResponse<string>>(restRequest);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                apiResponse.Data = restResponse.Data.Data;
                apiResponse.Success = restResponse.Data.Success;
                apiResponse.Errors = restResponse.Data.Errors;
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not process your request");
            }
            return apiResponse;
        }
    }
}

