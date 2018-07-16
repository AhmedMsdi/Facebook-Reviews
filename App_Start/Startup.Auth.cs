using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Atreemo.Models;
using Microsoft.Owin.Security.Cookies;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Web.Http;
using Atreemo.Providers;
using Microsoft.AspNet.Identity.Owin;
using Atreemo;
using System.Configuration;

namespace Atreemo
{
    public partial class Startup
    {

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        private void ConfigureAuth(IAppBuilder app)
        {
#if AUTH_IDENTITY
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(15),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(720),

            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            var ActeolADFS = new Microsoft.Owin.Security.WsFederation.WsFederationAuthenticationOptions
            {
                //MetadataAddress = "https://acteoladfs.sweetspot-technologies.com/FederationMetadata/2007-06/FederationMetadata.xml",
                MetadataAddress = ConfigurationManager.AppSettings["ida:FedMetadataURI"],
                AuthenticationType = "ADFS Authentication",
                Caption = ConfigurationManager.AppSettings["ida:ADFSCaption"],
                //"ADFS Authentication",
                //localhost
                //Wreply = @"https://atreemouat.itsucomms.com/Account/ExternalLoginCallback",
                //Wtrealm = @"https://atreemouat.itsucomms.com/",
                //Wreply = @"https://localhost:44355/Account/ExternalLoginCallback",
                //Wtrealm = @"https://localhost:44355/",

                Wtrealm = ConfigurationManager.AppSettings["ida:Wtrealm"],
                Wreply = ConfigurationManager.AppSettings["ida:Wtrealm"] + @"Account/ExternalLoginCallback",

                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
                //SignOutWreply
            };
            //add to pipeline
            app.UseWsFederationAuthentication(ActeolADFS);
#endif

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
                // In production mode set AllowInsecureHttp = false
                //AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }


    }
}



/*
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Atreemo.Models;
using Microsoft.Owin.Security.Cookies;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Web.Http;
using Atreemo.Providers;

namespace LocalAccountsApp
{
    public partial class Startup
    {

        // This method is required by Katana:
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var webApiConfiguration = ConfigureWebApi();
            app.Use(webApiConfiguration);          
        }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        private void ConfigureAuth(IAppBuilder app)
        {
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("~/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }


        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            return config;
        }

    }
}
*/
