[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AHP.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(AHP.Web.App_Start.NinjectWebCommon), "Stop")]

namespace AHP.Web.App_Start
{
    using System;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Core.Logger;
    using Helpers;
    using RestSharp;
    using System.Configuration;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IActiveAnalyticsLogger>().ToMethod(ctx =>
            {
                var name = ctx.Request.Target.Member.DeclaringType.Namespace;
                return new Log4netLogger(name);
            });

            string webApiUrl = ConfigurationManager.AppSettings["webapibaseurl"];
            string tableauServerUrl = ConfigurationManager.AppSettings["tableauServerUrl"];

            //Rest client for web api
            kernel.Bind<IServerDataRestClient>()
                .To<ServerDataRestClient>()
                .WithConstructorArgument("restClient", new RestClient(webApiUrl));

            Models.TableauServerInfo restClientConfig = new Models.TableauServerInfo()
            {
                ServerUrl = tableauServerUrl
            };

            restClientConfig.EnableProxy = System.Configuration.ConfigurationManager.AppSettings["enableProxy"] == "true";

            //Rest client for Tableau REST api, rest client and tableau server info
            kernel.Bind<ITableauRestConnector>()
                .To<TableauRestClient>()
                .InSingletonScope()
                .WithConstructorArgument("tabInfo", restClientConfig);

            kernel.Bind<IOwinAuthenticationManager>().To<OwinAuthenticationManager>();
        }
    }
}