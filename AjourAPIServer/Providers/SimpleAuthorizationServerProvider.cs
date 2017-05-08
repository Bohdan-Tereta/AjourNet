using AjourAPIServer.Concrete;
using AjourAPIServer.Entity;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace AjourAPIServer.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (AjourAPIServerDbContext _repo = new AjourAPIServerDbContext())
            {
                UserProfile profile = _repo.UserProfile.Where(u => u.UserName == context.UserName).FirstOrDefault();
                int UserId;

                if (profile != null)
                {
                    UserId = profile.UserId;
                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    context.Rejected();
                    return;
                }

                bool result = System.Web.Helpers.Crypto.VerifyHashedPassword(_repo.Membership.Where(u => u.UserId == UserId).FirstOrDefault().Password, context.Password);


                if (!result)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    context.Rejected();
                    return;
                }

                var id = new ClaimsIdentity(context.Options.AuthenticationType);
                id.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

                context.Validated(id);
            }
        }
    }
}