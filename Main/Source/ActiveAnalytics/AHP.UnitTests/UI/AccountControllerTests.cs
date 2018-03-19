#region -- File Headers --

/*
 * Purpose: Unit tests for Account Controller
 * Date: 2 March 2017
 * 
 */

#endregion

#region -- Using Statement --

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AHP.Web.Controllers;
using Moq;
using System.Security.Claims;
using System.Web.Mvc;
using System;
using AHP.Web.Models;
using System.Configuration;
using AHP.Web.Helpers;
using AHP.Core.Model;
using AHP.Web.Api.Models;
using AHP.Core.Logger;
using System.Text;
using System.Web;

#endregion

namespace AHP.UnitTests.UI
{
    /// <summary>
    /// Summary description for AccountControllerTests
    /// </summary>
    [TestClass]
    public class AccountControllerTests : UIBaseTest
    {
        #region -- Members --

        private AccountController _accountController;

        private Mock<IOwinAuthenticationManager> _mockOwinAuthManager;

        private Mock<AHP.Web.Helpers.ITableauRestConnector> _mockTableauConnector;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _mockOwinAuthManager = new Mock<IOwinAuthenticationManager>();
            _mockTableauConnector = new Mock<ITableauRestConnector>();
            _accountController = new AccountController(mockLogger.Object, mockRestClient.Object, _mockOwinAuthManager.Object,_mockTableauConnector.Object);
            _accountController.ControllerContext = base.MockControllerContext.Object;
            _accountController.Url = MockUrlHelper.Object;
        }

        #endregion

        #region -- Test Methods --

        [TestMethod]
        public void Login_External_Using_Correct_Credentials()
        {
            Web.Models.User loginRequest = new Web.Models.User()
            {
                IsInternalUser = false,
                Password = "@ahm1234",
                UserName = "test_user" 
            };
            var loginResponse = _accountController.Login(loginRequest);
            Assert.IsNotNull(loginResponse);
            Assert.IsInstanceOfType(loginResponse, typeof(ViewResult));
        }

        [TestMethod]
        public void Login_Internal_Using_Correct_Credentials()
        {
            LDAPUser authResponse = new LDAPUser();
            authResponse.AccountName = "TEST_USER";
            authResponse.DisplayName = "TEST_USER";
            authResponse.FirstName = "TEST";
            authResponse.LastName = "USER";
            authResponse.LDAPAccessStatus = LDAPAccessStatus.UserLogonSuccessful;
            authResponse.LDAPAccessStatusMessage = string.Empty;            
            mockRestClient.Setup(restClient => restClient.IsLDAPAuthenticated(It.IsAny<string>(), It.IsAny<string>())).Returns(authResponse);
            _accountController = new AccountController(mockLogger.Object, mockRestClient.Object, _mockOwinAuthManager.Object,_mockTableauConnector.Object);
            _accountController.Url = MockUrlHelper.Object;
            Web.Models.User loginRequest = new User()
            {
                IsInternalUser = true,
                UserName = "test_user",
                Password = "test_p@ssword"
            };
            var loginResponse = _accountController.Login(loginRequest);
            Assert.IsNotNull(loginResponse);
            Assert.IsInstanceOfType(loginResponse, typeof(ViewResult));           
        }

        [TestMethod]
        public void Login_Invalid_User_Details()
        {
            Web.Models.User loginRequest = new User();
            var loginResponse = _accountController.Login(loginRequest);
            Assert.IsNotNull(loginResponse);
            Assert.IsInstanceOfType(loginResponse, typeof(ViewResult));           
        }

        [TestMethod]
        public void Logout_User_Triggered()
        {
            RedirectToRouteResult logoutResponse = _accountController.Logout() as RedirectToRouteResult;
            Assert.IsNotNull(logoutResponse);
            Assert.IsInstanceOfType(logoutResponse, typeof(RedirectToRouteResult));
            Assert.AreEqual("Default", logoutResponse.RouteValues["controller"]);
            Assert.AreEqual("Login", logoutResponse.RouteValues["action"]);
            Assert.AreEqual("2", logoutResponse.RouteValues["id"]);
        }

        [TestMethod]
        public void Logout_Session_Timedout()
        {
            mockRestClient.Setup(rstClient => rstClient.LogoffFromSAP(It.IsAny<string>(),It.IsAny<string>())).Returns(false);
            _accountController = new AccountController(mockLogger.Object, mockRestClient.Object, _mockOwinAuthManager.Object,_mockTableauConnector.Object);
            RedirectToRouteResult logoutResponse = _accountController.Logout() as RedirectToRouteResult;
            Assert.IsNotNull(logoutResponse);
            Assert.IsInstanceOfType(logoutResponse, typeof(RedirectToRouteResult));
            Assert.AreEqual("Default", logoutResponse.RouteValues["controller"]);
            Assert.AreEqual("Login", logoutResponse.RouteValues["action"]);
            //2 or 3 determines if session timed out or logout by end user
            Assert.AreEqual("3", logoutResponse.RouteValues["id"]);
        }

        [TestMethod]
        public void ForotPassword_Should_Redirect()
        {
            //var passwordResetResponse =  _accountController.ForgotPassword(new ForgotPassword());
            //Assert.IsNotNull(passwordResetResponse);
            //Assert.IsInstanceOfType(passwordResetResponse, typeof(RedirectToRouteResult));
        }

        #endregion

    }
}
