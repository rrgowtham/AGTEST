//#region -- File Headers --

///*
// * Purpose: Unit tests for Password Reset Controller
// * Date: 18 April 2017
// * 
// */

//#endregion

//#region -- Using Statement --

//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using AHP.Web.Controllers;
//using Moq;
//using System.Security.Claims;
//using System.Web.Mvc;
//using System;
//using AHP.Web.Models;
//using System.Configuration;
//using AHP.Web.Helpers;
//using AHP.Core.Model;
//using AHP.Web.Api.Models;
//using AHP.Core.Logger;
//using System.Text;
//using System.Web;

//#endregion


//namespace AHP.UnitTests.UI
//{
//    [TestClass]
//    public class PasswordResetControllerTests: UIBaseTest
//    {
//        #region -- Members --

//        private readonly PasswordResetController _passwordResetController;

//        private Mock<IOwinAuthenticationManager> _mockOwinAuthManager;

//        #endregion

//        #region -- Initialize --

//        [TestInitialize]
//        public void Initialize()
//        {
//            _mockOwinAuthManager = new Mock<IOwinAuthenticationManager>();
//            _passwordResetController = new PasswordResetController(mockLogger.Object, mockRestClient.Object,_mockOwinAuthManager.Object);
//            _passwordResetController.ControllerContext = base.MockControllerContext.Object;
//            _passwordResetController.Url = MockUrlHelper.Object;
//        }

//        #endregion

//        #region -- Test Methods --

//        [TestMethod]
//        public void Index_Must_Show_PasswordResetPage()
//        {
//            ActionResult response = _passwordResetController.Home(id: "2");
//            Assert.IsNotNull(response);
//            Assert.IsInstanceOfType(response, typeof(ViewResult));
//        }

//        [TestMethod]
//        public void Password_Is_Required()
//        {
//            Web.ViewModel.PasswordResetViewModel viewModel = new Web.ViewModel.PasswordResetViewModel();
//            viewModel.UserName = "username";
//            viewModel.ConfirmPassword = "confirm_password";
//            ActionResult response = _passwordResetController.Home(viewModel);
//            Assert.IsNotNull(response);
//            Assert.IsInstanceOfType(response, typeof(JsonResult));
//            JsonResult jsonResponse = response as JsonResult;
//            Assert.IsInstanceOfType(jsonResponse.Data, typeof(AjaxResponse));
//            Assert.IsFalse(((AjaxResponse)jsonResponse.Data).Success);
//        }

//        [TestMethod]
//        public void Reset_Password_Redirect()
//        {
//            mockRestClient.Setup(restClient => restClient
//            .ResetPasswordAndRedirect(It.IsAny<BOPasswordReset>()))
//            .Returns("");
//            _passwordResetController = new PasswordResetController(mockLogger.Object, mockRestClient.Object,_mockOwinAuthManager.Object);
//            _passwordResetController.ControllerContext = base.MockControllerContext.Object;
//            _passwordResetController.Url = MockUrlHelper.Object;
//            _passwordResetController.ModelState["Username"] = new ModelState()
//            {
               
//            };
//            Web.ViewModel.PasswordResetViewModel viewModel = new Web.ViewModel.PasswordResetViewModel();
//            viewModel.UserName = "username";
//            viewModel.ConfirmPassword = "confirm_password";
//            ActionResult response = _passwordResetController.Home(viewModel);
//            Assert.IsNotNull(response);
//            Assert.IsInstanceOfType(response, typeof(JsonResult));
//            JsonResult jsonResponse = response as JsonResult;
//            Assert.IsInstanceOfType(jsonResponse.Data, typeof(AjaxResponse));
//            Assert.IsTrue(((AjaxResponse)jsonResponse.Data).Success);
//        }


//        #endregion

//    }
//}
