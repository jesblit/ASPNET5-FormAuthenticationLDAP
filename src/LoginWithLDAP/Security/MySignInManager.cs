using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.OptionsModel;

namespace LoginWithLDAP
{
    /// <summary>
    /// The sign in manager manges the sign in and sign out functionality
    /// </summary>
    public class MySignInManager : SignInManager<MyUser>
    {
        private IHttpContextAccessor _ContextAccessor { get; set; }
        public MySignInManager(MyUserManager userManager,
                                IHttpContextAccessor contextAccessor,
                                IUserClaimsPrincipalFactory<MyUser> claimsFactory,
                                IOptions<IdentityOptions> optionsAccessor = null)
                : base(userManager, contextAccessor, claimsFactory, optionsAccessor, null)
        {
            _ContextAccessor = contextAccessor;
        }

        public async override Task SignOutAsync()
        {
            await _ContextAccessor.HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

}