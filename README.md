# ASPNET5-FormAuthenticationLDAP
Simple project showing a form authentication using LDAP with ASPNET5 / MVC6 / RC1

The solution will connect to the default domain LDAP.

Context:
How to use ASPNET5 (RC1), MVC6 and LDAP. 
This sounds easy, but I actually struggled putting things together in a simple way. 
Let’s say, we just want an application that authenticates and authorizes through LDAP via a simple login/password form.
We’ll have 2 LDAP groups in this example:
-	Viewers
-	Administrators

Before running the solution, you will need to modify constants in the class MyUserManager :
- private const string LDAP_VIEWER_GROUP_NAME = "[Your LDAP Group Name]";
- private const string LDAP_ADMIN_GROUP_NAME = "[Your LDAP Group Name]";
