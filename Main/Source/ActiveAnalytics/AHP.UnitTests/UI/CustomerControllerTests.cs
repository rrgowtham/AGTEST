#region -- File Headers --

/*
 * Purpose: Unit tests for Customer Controller
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
    public class CustomerControllerTests : UIBaseTest
    {

        #region -- Members --

        private CustomerController _customerController;

        private Mock<IOwinAuthenticationManager> _mockOwinAuthManager;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _mockOwinAuthManager = new Mock<IOwinAuthenticationManager>();
            mockRestClient.Setup(restClient => restClient.GetReportList(It.IsAny<string>())).Returns(new ReportCategory());
            _customerController = new CustomerController(mockLogger.Object, mockRestClient.Object,null);
            _customerController.ControllerContext = base.MockControllerContext.Object;
            _customerController.Url = MockUrlHelper.Object;
        }

        #endregion

        #region -- Test Methods --

        [TestMethod]
        public void Home_Must_ListUsersReports()
        {
            ActionResult response = _customerController.Home();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)response).Model, typeof(AHP.Web.ViewModel.ReportViewModel));
        }

        #endregion

    }
}
