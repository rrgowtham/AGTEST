using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AHP.Web.Models;
using Moq;
using RestSharp;

namespace AHP.UnitTests.UI
{
    /// <summary>
    /// Unit test for tableau rest connector
    /// </summary>
    [TestClass]
    public class TableauRestConnectorTest
    {
        #region -- Members --
        private AHP.Web.Helpers.TableauRestClient _tableauRestClient; 
        #endregion

        #region -- Test Initialization --
        [TestInitialize]
        public void InitializeTableauConnector()
        {
            TableauServerInfo serverInfo = new TableauServerInfo();
            serverInfo.ApiVersion = "2.3";
            serverInfo.ServerUrl = "http://localhost/tableau/";

            Mock<IRestClient> mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(rstClient => rstClient.Execute(It.IsAny<IRestRequest>())).Returns(new RestResponse());
            _tableauRestClient = new Web.Helpers.TableauRestClient(serverInfo);

        }
        #endregion

        #region -- Test Methods --
        [TestMethod]
        public void SignInMustGiveTicket()
        {
            GenericAjaxResponse<string> response = _tableauRestClient.SignIn("username");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SignInMustWork()
        {
            GenericAjaxResponse<string> response = _tableauRestClient.SignIn("username", "password");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetSitesForUser()
        {
            GenericAjaxResponse<List<TableauSite>> response = _tableauRestClient.SitesForUser("ticket");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetWorkbookForSites()
        {
            GenericAjaxResponse<List<TableauWorkbook>> response = _tableauRestClient.WorkbooksForSite("siteId", "ticket");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetWorkbookForUser()
        {
            GenericAjaxResponse<List<TableauWorkbook>> response = _tableauRestClient.WorkbooksForUser("siteId", "userId", "ticket");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetViewsForWorkbook()
        {
            GenericAjaxResponse<List<TableauWorkbookView>> response = _tableauRestClient.ViewsForWorkbook("siteId", "workbookId", "ticket");
            Assert.IsNotNull(response);
        } 
        #endregion

    }
}
