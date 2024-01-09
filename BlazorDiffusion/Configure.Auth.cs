using ServiceStack.Auth;
using BlazorDiffusion.Data;

[assembly: HostingStartup(typeof(BlazorDiffusion.ConfigureAuth))]

namespace BlazorDiffusion;

public class ConfigureAuth : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureAppHost(appHost => 
        {
            appHost.Plugins.Add(new AuthFeature(IdentityAuth.For<ApplicationUser>(options => {
                options.EnableCredentialsAuth = true;
                options.SessionFactory = () => new CustomUserSession();
            })));
        });
}
