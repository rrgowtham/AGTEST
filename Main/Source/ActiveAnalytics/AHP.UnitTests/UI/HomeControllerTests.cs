#region -- File Headers --

/*
 * Purpose: Unit tests for Home Controller
 * Date: 18 April 2017
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
    public class HomeControllerTests : UIBaseTest
    {
        #region -- Members --

        private HomeController _homeController;

        private Mock<IOwinAuthenticationManager> _mockOwinAuthManager;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _mockOwinAuthManager = new Mock<IOwinAuthenticationManager>();
            _homeController = new HomeController();
            _homeController.ControllerContext = base.MockControllerContext.Object;
            _homeController.Url = MockUrlHelper.Object;
        }

        #endregion

        #region -- Unit Tests --

        [TestMethod]
        public void Index_Should_Redirect_OnLogon()
        {
            ActionResult response = _homeController.Index();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(RedirectToRouteResult));
        }

        #endregion
    }
}
