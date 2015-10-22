# ASPNET5-FormAuthenticationLDAP
Simple project showing a form authentication using LDAP with ASPNET5 / MVC6 / Beta6

The solution will connect to the default domain LDAP.

Before running the solution, you will need to modify constants in the class MyUserManager :
private const string LDAP_VIEWER_GROUP_NAME = "[Your LDAP Group Name]";
private const string LDAP_ADMIN_GROUP_NAME = "[Your LDAP Group Name]";
