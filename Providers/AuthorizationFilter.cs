using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Atreemo.Providers
{
    public class AuthorizationFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //HttpContext ctx = HttpContext.Current;
            //string controllerName = ctx.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
            //string viewName = ctx.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            //Atreemo.Models.Tool mTool = Atreemo.Views.Tools.Functions.GetToolByName(controllerName);
            //bool IsAuthorized = true;
            //int UserID = LoggingHelper.GetUserIDByUserName(HttpContext.Current.User.Identity.Name);
            //if (mTool != null)
            //{
            //    IsAuthorized = Atreemo.Views.Tools.Functions.IsAuthorizedTool(mTool.ToolId, UserID);
                
            //}
            //if (!IsAuthorized)
            //{
            //    RedirectResult rr = new RedirectResult("~/Account/NotAuthorized");
            //    filterContext.Result = rr;
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}
            
            //mTool = Atreemo.Views.Tools.Functions.GetToolByName(viewName);
            //if (mTool != null)
            //{
            //    IsAuthorized = Atreemo.Views.Tools.Functions.IsAuthorizedTool(mTool.ToolId, UserID);
            //}

            //if (!IsAuthorized)
            //{
            //    RedirectResult rr = new RedirectResult("~/Account/NotAuthorized");
            //    filterContext.Result = rr;
            //}
            //base.OnActionExecuting(filterContext);

        }
    }
}