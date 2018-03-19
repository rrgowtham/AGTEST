#region -- File Headers --

/*
 * Purpose: Unit tests for Setup Controller
 * Date: 8 June 2017
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
    public class SetupControllerTests : UIBaseTest
    {

        #region -- Members --

        private SetupUserController _userSetupController;

        private Mock<IOwinAuthenticationManager> _mockOwinAuthManager;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _mockOwinAuthManager = new Mock<IOwinAuthenticationManager>();
            _userSetupController = new SetupUserController(mockLogger.Object,
                mockRestClient.Object,
                _mockOwinAuthManager.Object);
            _userSetupController.ControllerContext = base.MockControllerContext.Object;
            _userSetupController.Url = MockUrlHelper.Object;
        }

        #endregion

        #region -- Unit Test Methods --


        [TestMethod]
        public void Index_Should_Redirect_Home()
        {
            ActionResult result = _userSetupController.Index();
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Change_Password_Should_Redirect_ToHome()
        {
            ActionResult result = _userSetupController.ChangePassword();
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Change_Password_Should_Show_View()
        {

            var userInfo = ((ClaimsPrincipal)MockControllerContext.Object.HttpContext.User);
            var unAuthenticatedClaims = new ClaimsIdentity(userInfo.Claims, "cookie");
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangePassword, bool.TrueString));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.Company, "AHM"));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, bool.FalseString));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Email,"someone@activehealth.net"));
            MockControllerContext.SetupGet(context => context.HttpContext.User).Returns(new ClaimsPrincipal(unAuthenticatedClaims));
            //MockControllerContext.SetupGet(context => context.HttpContext.Request.IsAuthenticated).Returns(true);
            _userSetupController.ControllerContext = MockControllerContext.Object;
            ActionResult result = _userSetupController.ChangePassword();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Change_Password_ShouldWork()
        {
            var userInfo = ((ClaimsPrincipal)MockControllerContext.Object.HttpContext.User);
            var unAuthenticatedClaims = new ClaimsIdentity(userInfo.Claims, "cookie");
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangePassword, bool.TrueString));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.Company, "AHM"));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, bool.FalseString));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Email, "someone@activehealth.net"));
            MockControllerContext.SetupGet(context => context.HttpContext.User).Returns(new ClaimsPrincipal(unAuthenticatedClaims));


            mockRestClient.Setup(rst => rst.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new GenericAjaxResponse<bool>()
            {
                Data = true,
                Success = true            
            });
            _userSetupController = new SetupUserController(mockLogger.Object, mockRestClient.Object, _mockOwinAuthManager.Object);
            _userSetupController.ControllerContext = MockControllerContext.Object;

            AHP.Web.ViewModel.PasswordResetViewModel vModel = new Web.ViewModel.PasswordResetViewModel();
            vModel.OldPassword = "ahm1234AHM";
            vModel.ConfirmPassword  = vModel.NewPassword = "AHM1234ahm";            

            ActionResult result = _userSetupController.ChangePassword(vModel);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Select_Questions_Must_DisplayView()
        {
            var userInfo = ((ClaimsPrincipal)MockControllerContext.Object.HttpContext.User);
            var unAuthenticatedClaims = new ClaimsIdentity(userInfo.Claims, "cookie");
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangePassword, bool.TrueString));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, bool.TrueString));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.Company, "AHM"));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Email, "someone@activehealth.net"));
            MockControllerContext.SetupGet(context => context.HttpContext.User).Returns(new ClaimsPrincipal(unAuthenticatedClaims));

            mockRestClient.Setup(rst => rst.GetSecurityQuestionList()).Returns(new List<string>());
            _userSetupController = new SetupUserController(mockLogger.Object, mockRestClient.Object, _mockOwinAuthManager.Object);
            _userSetupController.ControllerContext = MockControllerContext.Object;

            ActionResult response = _userSetupController.SelectQuestions();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
        }

        [TestMethod]
        public void Select_Questions_Must_Update()
        {

            var userInfo = ((ClaimsPrincipal)MockControllerContext.Object.HttpContext.User);
            var unAuthenticatedClaims = new ClaimsIdentity(userInfo.Claims, "cookie");
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangePassword, bool.TrueString));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.MustChangeSecurityQuestion, bool.TrueString));
            unAuthenticatedClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.Company, "AHM"));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            unAuthenticatedClaims.AddClaim(new Claim(ClaimTypes.Email, "someone@activehealth.net"));
            MockControllerContext.SetupGet(context => context.HttpContext.User).Returns(new ClaimsPrincipal(unAuthenticatedClaims));

            mockRestClient.Setup(rst => rst.GetSecurityQuestionList()).Returns(new List<string>());
            mockRestClient.Setup(rst => rst.SetSecurityQuestionsForUser(It.IsAny<string>(), It.IsAny<List<AHP.Core.DTO.UserSecurityOption>>())).
                Returns(new GenericAjaxResponse<bool>() { Success = true, Data = true });
            _userSetupController = new SetupUserController(mockLogger.Object, mockRestClient.Object, _mockOwinAuthManager.Object);
            _userSetupController.ControllerContext = MockControllerContext.Object;

            ActionResult response = _userSetupController.SelectQuestions(new Web.ViewModel.SecurityQuestionsViewModel() {
                ThirdProvidedAnswer = "ans",
                SecondaryProvidedAnswer = "ans",
                PrimaryProvidedAnswer = "ans",
                PrimarySelectedQuestion = "ques",
                SecondarySelectedQuestion = "ques",
                ThirdSelectedQuestion = "ques"
            });
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
        }

        #endregion

    }
}
