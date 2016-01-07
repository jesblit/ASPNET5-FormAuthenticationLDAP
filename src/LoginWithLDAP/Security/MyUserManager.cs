using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;

namespace LoginWithLDAP
{
    /// <summary>
    /// The custom user manager will check user password in LDAP 
    /// and will retrieve the user roles by mapping LDAP groups to applicative roles
    /// 
    /// The role store is not used and we're doing a custom mapping here
    /// </summary>
    public class MyUserManager : UserManager<MyUser>
    {
        private const string LDAP_VIEWER_GROUP_NAME = "[Your LDAP Group Name]";
        private const string LDAP_ADMIN_GROUP_NAME = "[Your LDAP Group Name]";

        public MyUserManager(
            IUserStore<MyUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<MyUser> passwordHasher,
            IEnumerable<IUserValidator<MyUser>> userValidators,
            IEnumerable<IPasswordValidator<MyUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IEnumerable<IUserTokenProvider<MyUser>> tokenProviders,
            IHttpContextAccessor contextAccessor) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                null, null, contextAccessor)
        {

        }

        public async override Task<bool> CheckPasswordAsync(MyUser user, string password)
        {
            return await Task.Run(() => IsAuthenticated(user.UserName, password));
        }

        public async override Task<IList<string>> GetRolesAsync(MyUser user)
        {
            return await Task.Run(() => GetUserMyRoles(user.UserName));
        }

        // Check LDAP if user is in a GROUP
        private List<string> GetUserMyRoles(string username)
        {
            var myRoles = new List<string>();
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    if (IsUserInGroup(context, username, LDAP_VIEWER_GROUP_NAME))
                        myRoles.Add(GetRoleFromGroup(LDAP_VIEWER_GROUP_NAME));

                    if (IsUserInGroup(context, username, LDAP_ADMIN_GROUP_NAME))
                        myRoles.Add(GetRoleFromGroup(LDAP_ADMIN_GROUP_NAME));

                    return myRoles;
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        // Check LDAP if user is in a GROUP
        private bool IsUserInGroup(PrincipalContext context, string user, string group)
        {
            bool found = false;
            try
            {
                GroupPrincipal p = GroupPrincipal.FindByIdentity(context, group);
                UserPrincipal u = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, user);

                found = p.GetMembers(true).Contains(u);
            }
            catch (Exception)
            {
                found = false;
            }

            return found;
        }

        // Map LDAP Group to application Role
        private string GetRoleFromGroup(string group)
        {
            if (group == LDAP_VIEWER_GROUP_NAME)
                return "VIEWER";

            if (group == LDAP_ADMIN_GROUP_NAME)
                return "ADMINISTRATOR";

            else throw new Exception("Undefined LDAP Group");
        }

        // Validate username / password in LDAP
        private bool IsAuthenticated(string username, string pwd)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    return context.ValidateCredentials(username, pwd);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}