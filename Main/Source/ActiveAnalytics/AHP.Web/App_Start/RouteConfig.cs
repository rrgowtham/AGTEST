using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AHP.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Tableau/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Customer",
                url: "Customer",
                defaults: new { controller = "Customer", action = "Home", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "",
                defaults: new { controller = "Default", action = "Login", id = UrlParameter.Optional }
            );         

            routes.MapRoute(
              name: "OpenDoc",
              url: "BOE/OpenDocument/opendoc/openDocument.jsp",
              defaults: new { controller = "Reports", action = "OpenDocument" }
             );

            //routes.MapRoute(
            //  name: "Tableau",
            //  url: "Tableau/Launchpad/Website/site.jsp",
            //  defaults: new { controller = "Reports", action = "LaunchTableau" }
            // );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );            

        }
    }
}
