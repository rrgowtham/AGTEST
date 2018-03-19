using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(AHP.Web.App_Start.OwinStartup))]
namespace AHP.Web.App_Start
{    
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    //it resets as and when you use the application
                    SlidingExpiration = true,
                    //session timeout is 20 minutes
                    ExpireTimeSpan = System.TimeSpan.FromMinutes(20),
                    AuthenticationType = Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account/Logout")
                });
        }
    }
}