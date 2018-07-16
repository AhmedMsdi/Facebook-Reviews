using Atreemo.Views.Tools;
using Atrremo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Atreemo.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Text;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using Newtonsoft.Json;
using Atreemo.Providers;
using System.Net;
using System.Reflection;
using System.Net.Http.Headers;
using System.Web.Http.Results;

using System.Web.Script.Serialization;

using System.Threading;
using Atreemo.Tools;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.WsFederation;
using Microsoft.Owin.Security.Cookies;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.ComponentModel;
using System.DirectoryServices;

namespace Atreemo.Controllers
{
    [ErrorFilter]
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        [SessionExpireFilterAttribute]
        public ActionResult Index()
        {
            User user = new User();
            return View(user);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult ExternalLogin()
        {
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult Terms()
        {
            return this.View();
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult TimeOut()
        {
            return View();
        }

        public ActionResult ForgottenPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult PasswordForNewUser(string Token)
        {
            //#if AUTH_IDENTITY
            //            string UserID = "";
            //            using (WebClient webClient = new WebClient())
            //            {
            //                HttpClient client = new HttpClient();
            //                client.BaseAddress = new Uri(Functions.SERVICE_URI);

            //                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

            //                //var response = client.PostAsJsonAsync<SetPasswordBindingModel>("/API/Account/SetPassword", sc).Result;
            //                UserID = client.GetStringAsync("ApiAccount/GetIdentityUserID").Result;
            //            }
            //            ApplicationUser aUser = UserManager.FindById(UserID.Replace("\"",""));
            //            SignInManager.SignIn(aUser, false, false);
            //            //SignInManager.SignInAsync(aUser, false, false);


            //#endif

            return View();
        }

        [AllowAnonymous]
        public ActionResult PasswordChanged()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            Response.StatusCode = 500;
            return View();
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }


        public ActionResult ResendCredentials()
        {
            return View();
        }

        public ActionResult GetEncryptedPassword()
        {
            int UserID = (int)System.Web.Security.Membership.GetUser(User.Identity.Name, true).ProviderUserKey;
            bool GroupManagementEnabled = Atreemo.Views.Tools.Functions.IsAuthorizedTool(26, UserID);//If User Has Groups Management Access Then He will have access to this View
            if (GroupManagementEnabled)
            {
                return View();
            }
            else
                return RedirectToAction("NotAuthorized", "Account");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SignIn(string returnUrl, string Token)
        {
            return this.RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpPost]

        public ActionResult Login(LoginModel model, string returnUrl)
        {
            return this.View(model);
        }

        private void SignIn(List<Claim> claims)//Mind!!! This is System.Security.Claims not WIF claims
        {

            var claimsIdentity = new DemoIdentity(claims,
            DefaultAuthenticationTypes.ApplicationCookie);

            //This uses OWIN authentication

            LoggingHelper.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            LoggingHelper.AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimsIdentity);

            HttpContext.User = new DemoPrincipal(LoggingHelper.AuthenticationManager.AuthenticationResponseGrant.Principal);
        }

        private void Signout()
        {



            //This uses OWIN authentication
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            LoggingHelper.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.User = null;



        }

        private List<Claim> GetClaimsByUser(MembershipUser user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Sid, user.ProviderUserKey.ToString()));
            return claims;
        }

        public ActionResult LogOff()
        {
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoServerCaching();
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Session.Clear();

            FormsAuthentication.SignOut();

            Signout();
            return this.RedirectToAction("Index", "Home");
        }


    }
}