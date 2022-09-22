using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.BackOffice.Security;
using Umbraco.Extensions;

namespace SSOWithUmbraco.Core.SSO.BackOffice.Github
{
    public static class GitHubBackofficeAuthenticationExtensions
    {
        public static IUmbracoBuilder AddGitHubBackofficeAuthentication(this IUmbracoBuilder builder)
        {
            // Register OpenIdConnectBackOfficeExternalLoginProviderOptions here rather than require it in startup
            builder.Services.ConfigureOptions<GitHubBackOfficeExternalLoginProviderOptions>();


            builder.AddBackOfficeExternalLogins(logins =>
            {
                logins.AddBackOfficeLogin(
                    backOfficeAuthenticationBuilder =>
                    {
                        backOfficeAuthenticationBuilder.AddGitHub(

                            backOfficeAuthenticationBuilder.SchemeForBackOffice(GitHubBackOfficeExternalLoginProviderOptions.SchemeName),
                                options =>
                                {
                                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                    options.ClientId = "client-id";
                                    options.ClientSecret = "client-secret";
                                    options.Events.OnTicketReceived = ctx =>
                                    {
                                        var username = ctx.Principal.FindFirstValue(ClaimTypes.Email)
                                                ?? ctx.Principal.FindFirstValue("email")
                                                ?? throw new Exception("Missing email claim");
                                        if (username != null && ctx.Principal?.Identity is ClaimsIdentity claimsIdentity)
                                        {
                                            claimsIdentity.AddClaim(
                                                new Claim(
                                                    ClaimTypes.Email,
                                                    username
                                                )
                                            );
                                        }

                                        return Task.CompletedTask;
                                    };
                                });

                    });
            });

            return builder;
        }
    }
}
