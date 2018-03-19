[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AHP.Web.Api.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(AHP.Web.Api.App_Start.NinjectWebCommon), "Stop")]

namespace AHP.Web.Api.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using System.Web.Http;
    using Ninject.Web.WebApi;
    using Core.Service;
    using Service;
    using Core.Repository;
    using Repository;
    using Core.Logger;

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
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
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
            // Service registrations
            kernel.Bind<IAuthenticationService>().To<AuthenticationService>();
            kernel.Bind<IBOReportService>().To<BOReportService>();
            kernel.Bind<ILDAPAuthenticationService>().To<LDAPAuthenticationService>();
            kernel.Bind<IEmailSenderService>().To<EmailDeliveryService>();

            // Repository registrations
            kernel.Bind<IBIRepository>().To<BIRepository>();
            kernel.Bind<ILDAPAuthenticationRepository>().To<LDAPAuthenticationRepository>();
            //kernel.Bind<IPersonalInfoValidation>().To<PersonalInfoValidation>();
            kernel.Bind<IUserinfoManager>().To<UserinfoManager>();
            kernel.Bind<AHP.Crypto.Interfaces.ICryptoService>().To<AHP.Crypto.PBKDF2>();
            kernel.Bind<IAuditEventManager>().To<AuditEventManager>();
            kernel.Bind<IActiveAnalyticsLogger>().ToMethod(ctx => 
            {
                var name = ctx.Request.Target.Member.DeclaringType.Namespace;
                return new Log4netLogger(name);
            });
        }        
    }
}
