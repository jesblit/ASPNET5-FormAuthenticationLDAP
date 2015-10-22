using System;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace LoginWithLDAP
{
    /// <summary>
    /// We define the FindByIdAsync method to get a user populated thanks to LDAP
    /// </summary>
    /// <typeparam name="T">T is MyUser type</typeparam>
    public class MyUserStore<T> : IUserStore<MyUser>, IUserPasswordStore<MyUser>
    {
        // Search and create a user from Active Directory
        public async Task<MyUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId == null)
                return await Task.Run(() => new MyUser()
                {
                    UserName = "Anonymous",
                    Id = "Anonymous"
                });

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var directoryUser = UserPrincipal.FindByIdentity(context, userId);
                if (directoryUser != null)
                {
                    return await Task.Run(() => new MyUser()
                    {
                        Id = userId,
                        UserName = userId,
                        Email = directoryUser.EmailAddress,
                        FirstName = directoryUser.GivenName,
                        LastName = directoryUser.Surname,
                        DisplayName = directoryUser.Name
                    }
                    );
                }
                else
                    return await Task.Run(() => new MyUser() {
                        UserName = "Anonymous",
                        Id = "Anonymous"
                    });
            }
        }

        public void Dispose()
        {

        }

        #region Not Implemented

        public Task<string> GetUserIdAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<MyUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(MyUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(MyUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(MyUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}