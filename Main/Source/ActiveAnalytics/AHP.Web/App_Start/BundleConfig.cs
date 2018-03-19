using System.Web;
using System.Web.Optimization;

namespace AHP.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/noVersionJquery").Include(
                        "~/scripts/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/scripts/jquery.validate*"));

           

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/scripts/bootstrap.js",
                      "~/scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/customCSS").Include(
                      //"~/content/jquery.fancybox-1.3.4.css",
                      "~/Content/layout.css",
                      "~/Content/treeview.css",
                      "~/Content/bread-crumb.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/customScripts").Include(                
                      "~/scripts/bread-crumb.js",
                      "~/scripts/switcher.js",
                      "~/scripts/bgStretch.js",
                      //"~/scripts/jquery.fancybox-1.3.4.pack.js",
                      "~/scripts/jquery.easing.js",
                      "~/scripts/scripts.js",
                      "~/scripts/customLoader.js",
                      "~/scripts/jquery.maskedinput.min.js",                      
                      "~/scripts/Views/Shared/login.js",
                      "~/scripts/Views/NeedHelp/needHelp.js",
                      "~/scripts/Views/PasswordReset/pwReset.js",
                      "~/scripts/jquery.placeholderpatch.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularCore").Include(
                "~/scripts/angular.js",
                "~/scripts/ng-idle.js",
                "~/scripts/angular-route.js",
                "~/scripts/angular-resource.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/angular-messages.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/usermanagerapp").Include(
                "~/scripts/Apps/Usermanager/app.js",
                "~/scripts/Apps/Usermanager/factory/userRestFactory.js",
                "~/scripts/Apps/Usermanager/controllers/createUsersController.js",
                "~/scripts/Apps/Usermanager/controllers/userDetailsController.js",
                "~/scripts/Apps/Usermanager/controllers/internalusersController.js",
                "~/scripts/Apps/Usermanager/controllers/tableauController.js",
                "~/scripts/Apps/Usermanager/controllers/users.js"));
        }
    }
}
