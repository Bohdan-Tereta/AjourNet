[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AjourBT.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(AjourBT.App_Start.NinjectWebCommon), "Stop")]

namespace AjourBT.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using AjourBT.Domain.Abstract;
    using AjourBT.Domain.Concrete;
    using AjourBT.Infrastructure;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get { return _kernel; }
            private set { _kernel = value; }
        }
        

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
            //kernel.Bind<IRepository>().To<ListRepository>();
            //kernel.Bind<IRepository>().To<AjourDbRepository>();
            //kernel.Bind<IMessenger>().To<Messenger>();

            kernel.Bind<IRepository>().To<AjourDbRepository>().InRequestScope();
            kernel.Bind<IMessenger>().ToConstant<Messenger>(new Messenger(new AjourDbRepository()));
            kernel.Bind<IXLSExporter>().To<XlsExporter>().InRequestScope();

            Kernel = kernel;
        }        
    }
}
