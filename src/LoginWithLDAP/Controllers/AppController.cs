using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;

namespace LoginWithLDAP
{
    [Authorize]
    public class AppController : Controller
    {
        public UserManager<MyUser> UserManager { get; private set; }
        public SignInManager<MyUser> SignInManager { get; private set; }

        public AppController(IHttpContextAccessor httpContextAccessor,
                UserManager<MyUser> userManager,
                IOptions<IdentityOptions> optionsAccessor)
        {
            SignInManager = new MySignInManager(userManager as MyUserManager, httpContextAccessor, new MyClaimsPrincipleFactory());
            UserManager = userManager;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // If user is logged in, we redirect to the main page
                return RedirectToAction("Index", "App");
            }
            else
            {
                // If user is not authenticated, we display the login page 
                return View();
            }
        }


        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set shouldLockout: true
                var user = new MyUser { Id = model.Username, UserName = model.Username };

                // This is where we do the LDAP Authentication
                var result = await SignInManager.PasswordSignInAsync(user, model.Password, false, false);

                // If user authenticates, we the do the Authorization (set Claims)
                if (result.Succeeded)
                {
                    // Query LDAP to get the user
                    user = await UserManager.FindByIdAsync(user.Id);

                    // Set user roles (by mapping LDAP groups to application defined roles)
                    user.Roles = await UserManager.GetRolesAsync(user) as List<string>;

                    var claimsPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
                    if (claimsPrincipal != null && claimsPrincipal.Identity != null)
                    {
                        // Set the claims to the user 
                        await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                        return RedirectToAction("Index", "App");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Insufficient privileges. Please contact your Administrator to get access to the application.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        public IActionResult LogOff()
        {
            SignInManager.SignOutAsync();
            return RedirectToAction("Login", "App");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = (ClaimsIdentity)User.Identity;
            ViewBag.NameIdentifier = user.Name;

            var userName = user.FindFirst(x => x.Type == ClaimTypes.Name);
            var userEmail = user.FindFirst(x => x.Type == ClaimTypes.Email);
            var userFirstName = user.FindFirst(x => x.Type == ClaimTypes.GivenName);
            var userLastName = user.FindFirst(x => x.Type == ClaimTypes.Surname);

            ViewBag.Name = userName.Value ?? "";
            ViewBag.Email = userEmail.Value ?? "";
            ViewBag.GivenName = userFirstName.Value ?? "";
            ViewBag.Surname = userLastName.Value ?? "";

            ViewBag.Administrator = user.FindFirst(x => x.Type == ClaimTypes.Role && x.Value == "ADMINISTRATOR") != null ? "true" : "false";
            ViewBag.Viewer = user.FindFirst(x => x.Type == ClaimTypes.Role && x.Value == "VIEWER") != null ? "true" : "false";

            return View();
        }
    }

}
