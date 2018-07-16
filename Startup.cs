using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Atreemo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Security;
using Atreemo.Providers;
using System.Web.Helpers;

[assembly: OwinStartup(typeof(Atreemo.Startup))]

namespace Atreemo
{
    public partial class Startup
    {
        // This method is required by Katana:
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //// Configure Web API for self-host. 
            //HttpConfiguration config = new HttpConfiguration();
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //app.Map("/api", inner =>
            //{
            //    inner.UseWebApi(config);
            //});
        }

/*

        Introduction of mixed Auth of Identity and ADFS


        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            //{
            //    AllowInsecureHttp = true,
            //    TokenEndpointPath = new PathString("/Token"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
            //    Provider = new SimpleAuthorizationServerProvider()
            //};
            //app.UseOAuthBearerTokens(OAuthServerOptions);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }
*/
    }
}
