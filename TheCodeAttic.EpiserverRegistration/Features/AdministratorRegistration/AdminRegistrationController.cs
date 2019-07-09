using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Profile;
using EPiServer.Security;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;
using System.EnterpriseServices;

namespace TheCodeAttic.EpiserverRegistration.Features.AdministratorRegistration
{
    public class AdminRegistrationController : Controller
    {
        const string AdminRoleName = "WebAdmins";
        public const string ErrorKey = "CreateError";

        private const string _ViewPath = "~/Features/AdministratorRegistration/Index.cshtml";

        private UIUserProvider _UIUserProvider;
        private UIRoleProvider _UIRoleProvider;
        private UISignInManager _UISignInManager;
        private IContentSecurityRepository _contentSecurityRepository;

        public AdminRegistrationController()
        {
            _UIUserProvider = ServiceLocator.Current.GetInstance<UIUserProvider>(); ;
            _UIRoleProvider = ServiceLocator.Current.GetInstance<UIRoleProvider>();
            _UISignInManager = ServiceLocator.Current.GetInstance<UISignInManager>();
            _contentSecurityRepository = ServiceLocator.Current.GetInstance<IContentSecurityRepository>();
        }       

        public ActionResult Index()
        {
            return View(_ViewPath, new RegisterViewModel { RegistrationNotAllowed = RegistrationHelpers.IsMaximumUsersRegistered() });
        }

        
        // POST: /Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UIUserCreateStatus status;
                IEnumerable<string> errors = Enumerable.Empty<string>();
                var result = _UIUserProvider.CreateUser(model.Username, model.Password, model.Email, null, null, true, out status, out errors);
                if (status == UIUserCreateStatus.Success)
                {
                    //Check if role exists, if it does not create it and set full permission to all pages of the site
                    SetFullAccessToRole(AdminRoleName);

                    //Add the newly created user to the role.
                    _UIRoleProvider.AddUserToRoles(result.Username, new string[] { AdminRoleName });

                    if (ProfileManager.Enabled)
                    {
                        var profile = EPiServerProfile.Wrap(ProfileBase.Create(result.Username));
                        profile.Email = model.Email;
                        profile.Save();
                    }

                    //After user creation, sign-in the user account and redirect to Episerver CMS
                    var resFromSignIn = _UISignInManager.SignIn(_UIUserProvider.Name, model.Username, model.Password);
                    if (resFromSignIn)
                        return Redirect("/episerver/cms");
                }
                AddErrors(errors);
            }
            // If we got this far, something failed, redisplay form
            return View(_ViewPath, model);
        }

        private void SetFullAccessToRole(string roleName)
        {
            if (!_UIRoleProvider.RoleExists(roleName))
            {
                _UIRoleProvider.CreateRole(roleName);
                var securityrep = ServiceLocator.Current.GetInstance<IContentSecurityRepository>();
                var permissions = securityrep.Get(ContentReference.RootPage).CreateWritableClone() as IContentSecurityDescriptor;
                permissions.AddEntry(new AccessControlEntry(roleName, AccessLevel.FullAccess));
                securityrep.Save(ContentReference.RootPage, permissions, SecuritySaveType.Replace);
            }
        }

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(ErrorKey, error);
            }
        }

    }
}