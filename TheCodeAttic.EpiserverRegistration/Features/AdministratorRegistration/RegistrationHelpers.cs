using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using System.Configuration;

namespace TheCodeAttic.EpiserverRegistration.Features.AdministratorRegistration
{
    public static class RegistrationHelpers
    {
        /// <summary>
        /// Returns TRUE - if the total minimum users have been registered as configured in the web.config
        /// </summary>
        /// <returns>TRUE - total maximum registered users has been reached, FALSE - additional users can be registered</returns>
        public static bool IsMaximumUsersRegistered()
        {
            var provider = ServiceLocator.Current.GetInstance<UIUserProvider>();
            int totalUsers = 0;
            var res = provider.GetAllUsers(0, 1, out totalUsers);
            int maximumAllowedDefault = 1;
            int.TryParse(ConfigurationManager.AppSettings.Get("thecodeattic:maximumtotalusers"), out maximumAllowedDefault);
            return totalUsers >= maximumAllowedDefault;
        }
    }
}