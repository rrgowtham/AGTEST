#region -- File Header --

/*
 * Author: James
 * Purpose: Implementation for connecting to Tableau REST api
 * Date: July 6 2017
 * 
 */

#endregion

#region -- Using Statements --

using AHP.Web.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AHP.Web.Helpers
{
    /// <summary>
    /// Allows to connect to Tableau server and query its REST Api
    /// </summary>
    public class TableauRestClient : ITableauRestConnector
    {

        #region -- Members --

        private TableauServerInfo _tableauServerInfo;

        private IRestClient _tableauRestClient;

        #endregion

        #region -- Constructors --

        /// <summary>
        /// Creates an instance of <see cref="TableauRestClient"/> class
        /// </summary>
        /// <param name="tabInfo"></param>
        public TableauRestClient(TableauServerInfo tabInfo)
        {
            _tableauServerInfo = tabInfo;
            _tableauRestClient = new RestClient(_tableauServerInfo.ServerUrl);
            if (_tableauServerInfo.EnableProxy)
            {
                _tableauRestClient.Proxy = new System.Net.WebProxy(TableauServerInfo.Proxy, TableauServerInfo.ProxyPort);
            }
        }

        #endregion

        #region -- Interface Implementation --

        public GenericAjaxResponse<string> SignIn(string username)
        {
            GenericAjaxResponse<string> apiResponse = new GenericAjaxResponse<string>() { Success = false };
            string signOnUrl = string.Format("/trusted");
            RestRequest signInRequest = new RestRequest(signOnUrl);
            signInRequest.Method = Method.POST;
            signInRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            signInRequest.AddParameter("username", username);
            signInRequest.AddParameter("target_site", System.Configuration.ConfigurationManager.AppSettings["tableauSite"]);
            IRestResponse signOnResponse = _tableauRestClient.Execute(signInRequest);
            if (signOnResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //AUTH Ticket seems to be always a 49 characters
                if (signOnResponse.Content != "-1" && signOnResponse.ContentType == "*; charset=UTF-8" && signOnResponse.ContentLength == 49)
                {
                    apiResponse.Data = signOnResponse.Content;
                    apiResponse.Success = true;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = string.Empty;
                }
            }
            return apiResponse;
        }

        public GenericAjaxResponse<string> SignIn(string username, string password)
        {
            string signOnUrl = string.Format("/api/{0}/auth/signin", _tableauServerInfo.ApiVersion);
            GenericAjaxResponse<string> apiResponse = new GenericAjaxResponse<string>();
            RestRequest signInRequest = new RestRequest(signOnUrl);
            signInRequest.AddXmlBody(string.Format("<tsRequest><credentials name='{0}' password='{1}'><site contentUrl='{2}'/></credentials></tsRequest>", username, password, System.Configuration.ConfigurationManager.AppSettings["tableauSite"]));
            signInRequest.Method = Method.POST;
            IRestResponse signOnResponse = _tableauRestClient.Execute(signInRequest);
            if (signOnResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return apiResponse;
            }
            return apiResponse;
        }

        public GenericAjaxResponse<bool> SignOut(string ticket)
        {
            string signOutUrl = string.Format("/api/{0}/auth/signout", _tableauServerInfo.ApiVersion);
            GenericAjaxResponse<bool> apiResponse = new GenericAjaxResponse<bool>();
            RestRequest signOutRequest = new RestRequest(signOutUrl);
            signOutRequest.Method = Method.POST;
            signOutRequest.AddHeader("X-Tableau-Auth", ticket);
            IRestResponse signOutResponse = _tableauRestClient.Execute(signOutRequest);
            return apiResponse;
        }

        public GenericAjaxResponse<List<TableauSite>> SitesForUser(string ticket)
        {
            //does not work
            string sitesForUserUrl = string.Format("api/{0}/sites", _tableauServerInfo.ApiVersion);
            GenericAjaxResponse<List<TableauSite>> apiResponse = new GenericAjaxResponse<List<TableauSite>>();
            RestRequest sitesForUserRequest = new RestRequest(sitesForUserUrl);
            sitesForUserRequest.AddHeader("X-Tableau-Auth", ticket);
            IRestResponse sitesForUserResponse = _tableauRestClient.Execute(sitesForUserRequest);
            return apiResponse;
        }

        public GenericAjaxResponse<List<TableauWorkbook>> WorkbooksForSite(string siteId, string ticket)
        {
            //does not work
            string workBooksForSiteUrl = string.Format("/api/{0}/sites/{1}/workbooks", _tableauServerInfo.ApiVersion, siteId);
            GenericAjaxResponse<List<TableauWorkbook>> apiResponse = new GenericAjaxResponse<List<TableauWorkbook>>();
            RestRequest workbooksForSiteRequest = new RestRequest(workBooksForSiteUrl);
            workbooksForSiteRequest.AddHeader("X-Tableau-Auth", ticket);
            IRestResponse workooksForSiteResponse = _tableauRestClient.Execute(workbooksForSiteRequest);
            return apiResponse;
        }

        public GenericAjaxResponse<List<TableauWorkbook>> WorkbooksForUser(string siteId, string userId, string ticket)
        {
            //does not work
            string workbooksForUserUrl = string.Format("/api/{0}/sites/{1}/users/{2}/workbooks", _tableauServerInfo.ApiVersion, siteId, userId);
            GenericAjaxResponse<List<TableauWorkbook>> apiResponse = new GenericAjaxResponse<List<TableauWorkbook>>();
            RestRequest workbookForUserRequest = new RestRequest(workbooksForUserUrl);
            workbookForUserRequest.AddHeader("X-Tableau-Auth", ticket);
            IRestResponse workbooksForUserResponse = _tableauRestClient.Execute(workbookForUserRequest);
            return apiResponse;
        }

        public GenericAjaxResponse<List<TableauWorkbookView>> ViewsForWorkbook(string siteId, string workbookId, string ticket)
        {
            //does not work
            string viewsForWorkbookUrl = string.Format("/api/{0}/sites/{1}/workbooks/{2}/views", _tableauServerInfo.ApiVersion, siteId, workbookId);
            GenericAjaxResponse<List<TableauWorkbookView>> apiResponse = new GenericAjaxResponse<List<TableauWorkbookView>>();
            RestRequest viewsForWorkbookRequest = new RestRequest(viewsForWorkbookUrl);
            viewsForWorkbookRequest.AddHeader("X-Tableau-Auth", ticket);
            IRestResponse viewsForWorkbookResponse = _tableauRestClient.Execute(viewsForWorkbookRequest);
            return apiResponse;
        }

        #endregion

    }
}