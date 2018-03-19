#region -- File Headers --

/*
 * Purpose: Unit tests for Help Controller
 * Date: 12 April 2017
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
    public class HelpControllerTests : UIBaseTest
    {
        #region -- Members --

        private HelpController _helpController;

        private Mock<IOwinAuthenticationManager> _mockOwinAuthManager;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _mockOwinAuthManager = new Mock<IOwinAuthenticationManager>();
            mockRestClient.Setup(rstClient => rstClient.PostNeedHelpMessage(It.IsAny<Customer>())).Returns(true);
            _helpController = new HelpController(mockLogger.Object, mockRestClient.Object);
            _helpController.ControllerContext = base.MockControllerContext.Object;
            _helpController.Url = MockUrlHelper.Object;
        }

        #endregion

        #region -- Test Methods --

        [TestMethod]
        public void Need_Help_MustShow_View()
        {
            var needHelpResponse = _helpController.NeedHelp("1");
            Assert.IsNotNull(needHelpResponse);
            Assert.IsInstanceOfType(needHelpResponse, typeof(ViewResult));
            AHP.Web.ViewModel.CustomerViewModel viewModel = ((ViewResult)needHelpResponse).Model as AHP.Web.ViewModel.CustomerViewModel;
            Assert.AreEqual(6, viewModel.SelectedIssueId);
        }

        [TestMethod]
        public void Need_Help_MustSendEmail()
        {
            Web.ViewModel.CustomerViewModel input = new Web.ViewModel.CustomerViewModel();
            input.Company = "Company Name";
            input.Email = "username@domain.com";
            input.FirstName = "First Name";
            input.IssueDescription = "Password Reset";
            input.LastName = "Last Name";
            input.PhoneNumber = "123-123-1234";
            input.SelectedIssueId = 1;
            ActionResult response = _helpController.NeedHelp(input);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(JsonResult));
            JsonResult needhelpResponse = response as JsonResult;
            Assert.IsTrue(((AjaxResponse)needhelpResponse.Data).Success);
            Assert.IsTrue(((AjaxResponse)needhelpResponse.Data).HomeUrl.Equals("/controller/action/id",StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(((AjaxResponse)needhelpResponse.Data).Message.Equals("Your message has been sent.",StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}
