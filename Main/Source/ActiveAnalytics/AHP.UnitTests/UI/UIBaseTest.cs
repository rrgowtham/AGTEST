using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AHP.Core.Logger;
using AHP.Web.Helpers;
using AHP.Web.Models;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace AHP.UnitTests.UI
{
    [TestClass]
    public class UIBaseTest
    {
        public Mock<IActiveAnalyticsLogger> mockLogger;

        public Mock<IServerDataRestClient> mockRestClient;

        private ClaimsPrincipal _userClaims;

        public Mock<System.Web.Mvc.ControllerContext> MockControllerContext { get; private set; }

        public Mock<System.Web.Mvc.UrlHelper> MockUrlHelper { get; private set; }

        [TestInitialize]
        public void InitiallizeBase()
        {
            mockLogger = new Mock<IActiveAnalyticsLogger>();            
            mockRestClient = new Mock<IServerDataRestClient>();
            BOUser authenticateSapResponse = new BOUser();
            authenticateSapResponse.BOSerializedSessionId = "session";
            authenticateSapResponse.BOSessionId = "serializedsession";
            authenticateSapResponse.LoginToken = "logintoken";
            authenticateSapResponse.MustChangePassword = false;
            mockRestClient.Setup(client => client.AuthenticateWithSAPAndGetToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(authenticateSapResponse);
            mockRestClient.Setup(client => client.LogoffFromSAP(It.IsAny<string>(),It.IsAny<string>())).Returns(true);

            //Setting up claims identity
            ClaimsIdentity userClaims = new ClaimsIdentity("cookie");            
            userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.IsInternalUser, bool.FalseString));
            userClaims.AddClaim(new Claim(System.Security.Claims.ClaimTypes.Name, "TestUser"));
            userClaims.AddClaim(new Claim(System.Security.Claims.ClaimTypes.GivenName, "Test User"));
            userClaims.AddClaim(new Claim(System.Security.Claims.ClaimTypes.Surname, "Testuser Surname"));
            userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.LogonToken, "Blah Blah Blah"));
            userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.BOSessionId, "BO Session Id"));
            userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.BOSerializedSession, "BO Serialized Session Id"));
            userClaims.AddClaim(new Claim(AHP.Core.ClaimTypes.DisplayName,"Display Name"));
            _userClaims = new ClaimsPrincipal(userClaims);


            MockControllerContext = new Mock<System.Web.Mvc.ControllerContext>();            
            MockControllerContext.SetupGet(context => context.HttpContext.User).Returns(_userClaims);
            MockControllerContext.SetupGet(context => context.HttpContext.Request.IsAuthenticated).Returns(true);

            MockUrlHelper = new Mock<System.Web.Mvc.UrlHelper>();
            MockUrlHelper.Setup(urlHelper => urlHelper.Action(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<object>())).Returns("/Controller/Action/id");
            MockUrlHelper.Setup(urlHelper => urlHelper.Action(It.IsAny<string>(), It.IsAny<string>())).Returns("/Controller/Action/id");
            /*
            //setting up request context
            var request = new HttpRequest("", "http://google.com", "rUrl=http://www.google.com")
            {
                ContentEncoding = Encoding.UTF8  //UrlDecode needs this to be set
            };
            
            var ctx = new HttpContext(request, new HttpResponse(new StringWriter()));

            //Session need to be set
            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(), 10, true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, false);

            //this adds aspnet session
            ctx.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new[] { typeof(HttpSessionStateContainer) },
                null)
                .Invoke(new object[] { sessionContainer });

            var data = new Dictionary<string, object>()
            {
                { "a", "b"} // fake whatever  you need here.
            };
            ctx.Items["owin.Environment"] = data;

            var wrapper = new HttpContextWrapper(ctx);
            wrapper.User = UserClaims;
            Thread.CurrentPrincipal = UserClaims;
            var routeData = new System.Web.Routing.RouteData();
            routeData.Values.Add("Action", "Logout");
            var requestContext = new System.Web.Routing.RequestContext(wrapper, routeData);
            ControllerContext = requestContext;
            */
        }
    }
}
