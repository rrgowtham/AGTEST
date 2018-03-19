#region -- File Headers --

/*
 * Purpose: Unit tests for Error Controller
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
    public class ErrorControllerTests
    {

        #region -- Members --

        private ErrorController _errorController;

        #endregion

        #region -- Initialize --

        [TestInitialize]
        public void Initialize()
        {
            _errorController = new ErrorController();
        }

        #endregion

        #region -- Unit Tests --

        [TestMethod]
        public void Should_Show_ErrorPage()
        {
            ActionResult response = _errorController.Index();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
        }

        #endregion

    }
}
