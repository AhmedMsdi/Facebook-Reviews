using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Atreemo.Models;
using System.Web.Security;

namespace Atreemo.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        private List<Claim> GetClaimsByUser(MembershipUser user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Sid, user.ProviderUserKey.ToString()));
            return claims;
        }

        private void SignIn(List<Claim> claims,OAuthAuthorizationEndpointResponseContext HttpContext)//Mind!!! This is System.Security.Claims not WIF claims
        {

            var claimsIdentity = new DemoIdentity(claims,
            DefaultAuthenticationTypes.ApplicationCookie);

            LoggingHelper.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            LoggingHelper.AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimsIdentity);
        }
#if AUTH_IDENTITY

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Should really do some validation here :)
            context.Validated();
            return base.ValidateClientAuthentication(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager);
            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

#else
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            } 

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            //var user = await userManager.FindByEmailAsync(context.UserName);
            //if (user == null)
            //{
            //    context.SetError("invalid_grant", "The user name or password is incorrect.");
            //    return;
            //}
            //else
            //{
            //    var result = userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, context.Password);
            //    if (!(result == PasswordVerificationResult.Success))
            //    {
            //        context.SetError("invalid_grant", "The user name or password is incorrect.");
            //        return;
            //    }
            //}
            //if (user == null)
            //{
            //    context.SetError("invalid_grant", "The user name or password is incorrect.");
            //    return;
            //}
            //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager);
            //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager);
            //AuthenticationProperties properties = CreateProperties(user.UserName);
            //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            //context.Validated(ticket);
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);


            var provider = Membership.Provider;
            string name = provider.ApplicationName; // Get the application name here
            if (Membership.ValidateUser(context.UserName, context.Password))
            {
                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

                var mmuser = Membership.GetUser(context.UserName);
                List<Claim> claims = GetClaimsByUser(mmuser);
                ApplicationUser user = new ApplicationUser();
                user.UserName = mmuser.UserName;
                user.Id = mmuser.ProviderUserKey.ToString();
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                var claimsIdentity = new DemoIdentity(claims,
                DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationProperties properties = CreateProperties(user.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(claimsIdentity, properties);
                context.Validated(ticket);
            }
            else
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
    }
#endif

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}