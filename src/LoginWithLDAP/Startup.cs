using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LoginWithLDAP
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

        }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthorization();
            services.AddIdentity<MyUser, MyRole>()
                .AddUserStore<MyUserStore<MyUser>>()
                .AddRoleStore<MyRoleStore<MyRole>>()
                .AddUserManager<MyUserManager>()
                .AddDefaultTokenProviders();
        }

        public void Configure(IApplicationBuilder app)
        {
            ConfigureAuth(app);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Login" });
            });
        }


        private void ConfigureAuth(IApplicationBuilder app)
        {
            // Use Microsoft.AspNet.Identity & Cookie authentication
            app.UseIdentity();
            app.UseCookieAuthentication(options =>
            {
                options.AutomaticAuthenticate = true;
                options.LoginPath = new PathString("/App/Login");
            });
        }
    }
}
