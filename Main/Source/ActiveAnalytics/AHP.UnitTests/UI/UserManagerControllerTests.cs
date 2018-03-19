#region -- File Headers --

/*
 * Purpose: Unit tests for User Manager Controller
 * Date: 12 June 2017
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
    [TestClass]
    public class UserManagerControllerTests : UIBaseTest
    {

        #region -- Members --

        private UserManagerController _userManagerController;

        private Mock<IOwinAuthenticationManager> _mockOwinAuthManager;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _mockOwinAuthManager = new Mock<IOwinAuthenticationManager>();
            mockRestClient.Setup(rst => rst.GetAllUsers()).
                Returns(new GenericAjaxResponse<List<Core.DTO.ExternalUserInfo>>() { Success = true, Data = new List<Core.DTO.ExternalUserInfo>() });
            mockRestClient.Setup(rst => rst.CreateUser(It.IsAny<Core.DTO.ExternalUserInfo>())).Returns(new GenericAjaxResponse<bool>() { Success = true, Data = true });
            mockRestClient.Setup(rst => rst.LockUser(It.IsAny<string>(),It.IsAny<string>())).Returns(new GenericAjaxResponse<bool>() { Success = true, Data = true });
            mockRestClient.Setup(rst => rst.ResetPassword(It.IsAny<string>(), It.IsAny<List<AHP.Core.DTO.UserSecurityOption>>())).Returns(new GenericAjaxResponse<bool>() { Success = true, Data = true });
            mockRestClient.Setup(rst => rst.GetUserDetails(It.IsAny<string>())).Returns(new GenericAjaxResponse<Core.DTO.ExternalUserInfo>() { Success = true, Data = new Core.DTO.ExternalUserInfo() { Username = "username" } });
            mockRestClient.Setup(rst => rst.ActivateEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(new GenericAjaxResponse<bool>() { Data = false, Success = false });
            mockRestClient.Setup(rst => rst.UpdateUser(It.IsAny<AHP.Core.DTO.ExternalUserInfo>())).Returns(new GenericAjaxResponse<Core.DTO.ExternalUserInfo>() { Success = true, Data = new Core.DTO.ExternalUserInfo() { } });
            mockRestClient.Setup(rst => rst.ActivateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(new GenericAjaxResponse<bool>() { Data = true, Success = true });
            mockRestClient.Setup(rst => rst.DeactivateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(new GenericAjaxResponse<bool>() { Data = true, Success = true });

            _userManagerController = new UserManagerController(mockLogger.Object, mockRestClient.Object);
            _userManagerController.ControllerContext = base.MockControllerContext.Object;
            _userManagerController.Url = MockUrlHelper.Object;
        }

        #endregion

        #region -- Unit Test Methods --

        [TestMethod]
        public void Index_Should_Show_View()
        {
            ActionResult response = _userManagerController.Index();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
        }

        [TestMethod]
        public void GetUsersList_Must_Return_Users()
        {
            ActionResult response = _userManagerController.GetUsersList();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
        }

        [TestMethod]
        public void CreateUser_Must_Succeed()
        {
            ActionResult response = _userManagerController.CreateUser(new Web.ViewModel.UserinfoViewModel()
            {
                BirthMonth = "12",
                BirthYear = "1987",
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsEmailActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"
            });
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
        }

        [TestMethod]
        public void LockUser_Must_Succeed()
        {
            ActionResult response = _userManagerController.LockUser(new Core.DTO.ExternalUserInfo()
            {
                BirthMonth = 12,
                BirthYear = 1987,
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"

            });
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
        }

        [TestMethod]
        public void UnlockUser_Must_Succeed()
        {
            ActionResult response = _userManagerController.UnlockUser(new Core.DTO.ExternalUserInfo()
            {
                BirthMonth = 12,
                BirthYear = 1987,
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"

            });
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
        }

        [TestMethod]
        public void ResetPassword_Must_Succeed()
        {
            ActionResult response = _userManagerController.ResetPassword(new Core.DTO.ExternalUserInfo()
            {
                BirthMonth = 12,
                BirthYear = 1987,
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"
            });
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
        }

        [TestMethod]
        public void UserDetail_Must_Return_Details()
        {
            ActionResult response = _userManagerController.Userdetail("username");
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
            Assert.AreEqual("username", ((GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo>)((JsonResult)response).Data).Data.Username);
        }

        [TestMethod]
        public void Activate_Email_Must_Work()
        {
            ActionResult response = _userManagerController.ActivateEmail(new Core.DTO.ExternalUserInfo()
            {
                BirthMonth = 12,
                BirthYear = 1987,
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"
            });

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
            Assert.IsFalse(((GenericAjaxResponse<bool>)((JsonResult)response).Data).Data);
        }

        [TestMethod]
        public void Update_User_Should_UpdateDetails()
        {
            ActionResult response = _userManagerController.UpdateUser(new Web.ViewModel.UserinfoViewModel()
            {
                BirthMonth = "12",
                BirthYear = "1987",
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"
            });

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
            Assert.IsInstanceOfType(((GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo>)((JsonResult)response).Data).Data,typeof(AHP.Core.DTO.ExternalUserInfo));
        }

        [TestMethod]
        public void Activate_User_ShouldWork()
        {
            ActionResult response = _userManagerController.ActivateUser(new Core.DTO.ExternalUserInfo()
            {
                BirthMonth = 12,
                BirthYear = 1987,
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"
            });

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
            Assert.IsTrue(((GenericAjaxResponse<bool>)((JsonResult)response).Data).Data);
        }

        [TestMethod]
        public void DeactivateUser_Should_Work()
        {
            ActionResult response = _userManagerController.DeactivateUser(new Core.DTO.ExternalUserInfo()
            {
                BirthMonth = 12,
                BirthYear = 1987,
                ChangePasswordOnLogon = true,
                Company = "AHM",
                Email = "someone@company.com",
                Firstname = "First Name",
                IsActive = true,
                IsLocked = false,
                Lastname = "Last Name",
                Role = "User",
                SupplierId = "123,123,1244",
                Username = "Username1234",
                ZipCode = "12312"
            });

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
            Assert.IsTrue(((GenericAjaxResponse<bool>)((JsonResult)response).Data).Data);
        }

        #endregion

    }
}