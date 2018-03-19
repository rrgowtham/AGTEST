#region -- File Header --

/*
 * Author: James
 * Purpose: Signature methods to connect to Tableau REST api
 * Date: July 6 2017
 * 
 */

#endregion

#region -- Using Statements --

using AHP.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AHP.Web.Helpers
{
    /// <summary>
    /// Connects to Tableau REST Api for authentication and user information
    /// </summary>
    public interface ITableauRestConnector
    {

        /// <summary>
        /// Performs a Sign on using user credentials to Tableau
        /// </summary>
        /// <param name="username">username of existing user on the tableau server</param>
        /// <param name="password">password used for the existing user account</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> which has the ticket</returns>
        GenericAjaxResponse<string> SignIn(string username,string password);

        /// <summary>
        /// Performs a trusted sign in using the username
        /// </summary>
        /// <param name="username">username of existing tableau user on the server</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> which has the ticket</returns>
        GenericAjaxResponse<string> SignIn(string username);

        /// <summary>
        /// Logs the user out from the tableau
        /// </summary>
        /// <param name="ticket">ticket provided to the user to signout</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/></returns>
        GenericAjaxResponse<bool> SignOut(string ticket);

        /// <summary>
        /// Gets the list of all sites the user with the <paramref name="ticket"/> has read access
        /// </summary>
        /// <param name="ticket">Ticket issued to user after trusted authentication is established</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<List<TableauSite>> SitesForUser(string ticket);

        /// <summary>
        /// Queries workbooks for a site
        /// </summary>
        /// <param name="siteId">Site id of the site to query workbook</param>
        /// <param name="ticket">Ticket issued to user after trusted authentication</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<List<TableauWorkbook>> WorkbooksForSite(string siteId, string ticket);

        /// <summary>
        /// Returns the workbooks that the specified user owns in addition to those that the user has Read (view) permissions for.
        /// </summary>
        /// <param name="siteId">The ID of the site that contains the user.</param>
        /// <param name="userId">The ID of the user to get workbooks for.</param>
        /// <param name="ticket">Authentication ticket on successfull trusted authentication</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<List<TableauWorkbook>> WorkbooksForUser(string siteId,string userId, string ticket);

        /// <summary>
        /// Returns all the views for the specified workbook, optionally including usage statistics
        /// </summary>
        /// <param name="siteId">The ID of the site that contains the workbook.</param>
        /// <param name="workbookId">The ID of the workbook to get the views for.</param>
        /// <param name="ticket">Authentication ticket on successfull trusted authentication</param>
        /// <returns>Instance of <see cref="GenericAjaxResponse{T}"/> class</returns>
        GenericAjaxResponse<List<TableauWorkbookView>> ViewsForWorkbook(string siteId, string workbookId, string ticket);
    }
}