#region -- File Headers --

/*
 * Purpose: Unit tests for Default Controller
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
    public class DefaultControllerTests:UIBaseTest
    {
        #region -- Members --

        private DefaultController _defaultController;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _defaultController = new DefaultController(mockLogger.Object);
            _defaultController.ControllerContext = base.MockControllerContext.Object;
            _defaultController.Url = MockUrlHelper.Object;
        }

        #endregion

        #region -- Test Methods --     

        [TestMethod]
        public void Login_MustProperly_Redirect()
        {
            ActionResult response = _defaultController.Login("1");
            //if user us already authenticated, its redirect response
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Login_Must_ShowLogonView()
        {
            var userInfo = ((ClaimsPrincipal)MockControllerContext.Object.HttpContext.User);
            var unAuthenticatedClaims = new ClaimsIdentity(userInfo.Claims,string.Empty);
            MockControllerContext.SetupGet(context => context.HttpContext.User).Returns(new ClaimsPrincipal(unAuthenticatedClaims));
            MockControllerContext.SetupGet(context => context.HttpContext.Request.IsAuthenticated).Returns(false);
            _defaultController.ControllerContext = MockControllerContext.Object;
            ActionResult response = _defaultController.Login("2");
            //if user isn't authenticated, its login view
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
        }

        #endregion
    }
}
