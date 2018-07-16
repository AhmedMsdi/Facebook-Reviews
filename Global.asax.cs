using Atreemo.Views.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Atreemo
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            try
            {
                // Think about adding a log for all instances
                if (ConfigurationManager.AppSettings["SQLDbName"].ToString().ToLower().Equals("pingponguat"))
                {
                    string uniqueid = DateTime.Now.Ticks.ToString();
                    string logfile = String.Format(@"C:\Inetpub\Log\{0}.txt", uniqueid);
                    if (Request.CurrentExecutionFilePath.ToLower().Contains("api/"))
                        Request.SaveAs(logfile, true);
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
        }

    }
}
