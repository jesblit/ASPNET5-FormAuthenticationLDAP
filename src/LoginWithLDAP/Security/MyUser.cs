using System.Collections.Generic;

namespace LoginWithLDAP
{
    /// <summary>
    /// Application User
    /// </summary>
    public class MyUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}