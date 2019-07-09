using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace TheCodeAttic.EpiserverRegistration.Features.AdministratorRegistration
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class AdminRegistrationRouteInitialization : IInitializableModule
    {
        /// <summary>
        /// Intialization that registers an MVC route to the Administration Registration Page  (/registeradmin) if the site is running as Localhost or the 'allowregistrationpage' configuration is marked as true 
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(InitializationEngine context)
        {
            //Only want to provide registration if running as Localhost OR if configuration has been set to TRUE
            if (HttpContext.Current.Request.IsLocal || (bool.TryParse(ConfigurationManager.AppSettings.Get("thecodeattic:allowregistrationpage"), out var b) && b))
            {
                //Register the route for the registration page.
                var routeData = new RouteValueDictionary();
                routeData.Add("controller", "AdminRegistration");
                routeData.Add("action", "Index");
                routeData.Add("id", " UrlParameter.Optional");
                RouteTable.Routes.Add("Register", new Route("AdminRegistration", routeData, new MvcRouteHandler()) { RouteExistingFiles = false });
            }
        }

        /// <summary>
        /// Unused in this implementation but required as part of the IInitializableModule interface
        /// </summary>
        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}
