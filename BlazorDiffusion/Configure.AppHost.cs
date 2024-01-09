using Funq;
using BlazorDiffusion.ServiceInterface;

[assembly: HostingStartup(typeof(BlazorDiffusion.AppHost))]

namespace BlazorDiffusion;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            // Configure ASP.NET Core IOC Dependencies
        });

    public AppHost() : base("BlazorDiffusion", typeof(MyServices).Assembly) { }

    // Configure your AppHost with the necessary configuration and dependencies your App needs
    public override void Configure(Container container)
    {
        SetConfig(new HostConfig {
            IgnorePathInfoPrefixes = { "/appsettings", "/_framework" },
        });
    }
    public static void RegisterKey() =>
        ServiceStack.Licensing.RegisterLicense("OSS BSD-3-Clause 2023 https://github.com/NetCoreApps/BlazorDiffusion hXP9cB4QXIpBVtmwd4f6KebB9XGC0G6hnrKGQRkoMrdeiuO9pKP+FtrNYms3tuQrs3SB1h5hxMztUSVJbYwwHfIua9Qsbn68oAQrV0EQPL85nKfnyeH1eSMASZJbvZK9coZ4ULc4LwHQAB8JFAnS6ftkJVcRIahQKGWUz4rw45Y=");
}
