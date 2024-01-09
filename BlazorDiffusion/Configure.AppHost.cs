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
        ServiceStack.Licensing.RegisterLicense("OSS GPL-3.0 2024 https://github.com/NetCoreApps/BlazorDiffusionAuto pBDTGJjpThY+Q87ldigaTPN5cKuCGyTdH7a9C6yT4q3ltfWWFydwNz/rhECDTKwIxdgqHNX4Xfhb7b2+2rkZAQaT8/CXH/7xJcITyKVQmtd0BKyXTgEQXUm1MFxQuyaT4+UzmkKU6iGrfQB4jH3u2JS6gDDi/RABOgszzBTDyXE=");
}
