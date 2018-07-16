using Atreemo.Views.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atreemo.Providers
{
    public class ErrorFilter : HandleErrorAttribute 
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                return;

            Exception ex = filterContext.Exception ?? new Exception("No further information exists.");

            Functions.SendErrorEmail(ex);

            base.OnException(filterContext);
        }
    }
}