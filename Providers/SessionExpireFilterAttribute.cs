using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Atreemo.Providers
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
            {

            HttpContext ctx = HttpContext.Current;
            // check if session is supported
            if (ctx.Session != null)
            {
                // check if a new session id was generated
                if ((ctx.Session.IsNewSession)/*||((ctx.Session["access_Token"] == null))*/)
                {

                    // If it says it is a new session, but an existing cookie exists, then it must
                    // have timed out
                    string sessionCookie = ctx.Request.Headers["Cookie"];
                    if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        
                        string loginUrl = "~/Account/TimeOut";
                        if (ctx.Session.IsNewSession)
                        {
                            string redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                            string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                            loginUrl = "/Account/Login" + redirectUrl;
                        }
                        if (ctx.Request.IsAuthenticated)
                        {
#if AUTH_IDENTITY
                            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            HttpContext.Current.Response.Cache.SetNoServerCaching();
                            HttpContext.Current.Response.Cache.SetNoStore();
                            HttpContext.Current.Session.Clear();
                            LoggingHelper.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
#else
            FormsAuthentication.SignOut();
#endif
                        }
                        RedirectResult rr = new RedirectResult(loginUrl);
                        filterContext.Result = rr;

                    }
                }
            }

            base.OnActionExecuting(filterContext);

        }
    }
}