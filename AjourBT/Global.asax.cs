using AjourBT.Domain.Concrete;
using AjourBT.Filters;
using AjourBT.Infrastructure;
using AjourBT.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading;
using AjourBT.Domain.Abstract;
using AjourBT.App_Start;
using Ninject;


namespace AjourBT
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string Culture;
        public static string DatePattern;
        public static string JSDatePattern;

        protected void Application_Start()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(@"/");
            GlobalizationSection section =
              (GlobalizationSection)config.GetSection("system.web/globalization");
            Culture = section.Culture.ToString();

            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture(Culture).DateTimeFormat;

            DatePattern = dtfi.ShortDatePattern;
            JSDatePattern = DatePattern.Replace("M", "m").Replace("yy", "y");

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            //Database.SetInitializer<AjourDbContext>(new AjourDbInitializer());
            // DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(AjourBT.Domain.Entities.Permit.RequiredIfAttribute),typeof(RequiredAttributeAdapter));

            if (WebConfigurationManager.AppSettings["EnableGreetings"] == "true")
            {
                Scheduler.Start(TimeSpan.Parse(WebConfigurationManager.AppSettings["GreetingsSendTime"]), NinjectWebCommon.Kernel.TryGet<IMessenger>());
            }
        }

#if !DEBUG
        protected void Application_Error()
        {
            Exception lastError = Server.GetLastError();
            Server.ClearError();

            int statusCode = 0;

            if (lastError.GetType() == typeof(HttpException))
            {
                statusCode = ((HttpException)lastError).GetHttpCode();
            }
            else
            {
                statusCode = 500;
            }

            HttpContextWrapper contextWrapper = new HttpContextWrapper(this.Context);

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "ShowErrorPage");
            routeData.Values.Add("statusCode", statusCode);
            routeData.Values.Add("exception", lastError);
            //routeData.Values.Add("isAjaxRequet", contextWrapper.Request.IsAjaxRequest());

            IController controller = new ErrorController();

            RequestContext requestContext = new RequestContext(contextWrapper, routeData);

            controller.Execute(requestContext);
            Response.End();
        }
#endif

    }
}