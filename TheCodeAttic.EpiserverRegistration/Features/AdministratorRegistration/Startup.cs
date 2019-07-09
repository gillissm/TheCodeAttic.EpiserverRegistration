using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartup(typeof(TheCodeAttic.EpiserverRegistration.Features.AdministratorRegistration.Startup))]
/// <summary>
/// Required startup class for configuration of OWIN. This is a modified version of the startup class as provided by Episerver in thier Alloy Demo Kit sample site, found at https://github.com/episerver/AlloyDemoKit.
/// </summary>
namespace TheCodeAttic.EpiserverRegistration.Features.AdministratorRegistration
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Add CMS integration for ASP.NET Identity
            app.AddCmsAspNetIdentity<ApplicationUser>();          

            // Use cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/util/login.aspx"),
                Provider = new CookieAuthenticationProvider
                {
                    // If the "/util/login.aspx" has been used for login otherwise you don't need it you can remove OnApplyRedirect.
                    OnApplyRedirect = cookieApplyRedirectContext =>
                    {
                        app.CmsOnCookieApplyRedirect(cookieApplyRedirectContext, cookieApplyRedirectContext.OwinContext.Get<ApplicationSignInManager<ApplicationUser>>());
                    },

                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager<ApplicationUser>, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.GenerateUserIdentityAsync(user))
                }
            });
        }
    }
}
