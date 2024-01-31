using Amazon.S3;
using Funq;
using BlazorDiffusion.ServiceInterface;
using BlazorDiffusion.ServiceModel;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.IO;

[assembly: HostingStartup(typeof(BlazorDiffusion.AppHost))]

namespace BlazorDiffusion;

public class AppHost : AppHostBase, IHostingStartup
{
    public const string LocalBaseUrl = "https://localhost:5001";
    
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            // Configure ASP.NET Core IOC Dependencies
            var cdnUrl = Environment.GetEnvironmentVariable("DEPLOY_CDN");
            cdnUrl = !string.IsNullOrEmpty(cdnUrl)
                ? $"https://{cdnUrl}"
                : null;

            var baseUrl = cdnUrl ?? LocalBaseUrl;
            var apiUrl = Environment.GetEnvironmentVariable("DEPLOY_API");
            apiUrl = !string.IsNullOrEmpty(apiUrl)
                ? $"https://{apiUrl}"
                : null;

            services.AddPlugin(new CorsFeature(allowedHeaders: "Content-Type,Authorization",
                allowOriginWhitelist: new[]
                {
                    "https://localhost:5002",
                    "http://localhost:5000",
                    "http://localhost:8080",
                    "https://diffusion.works",
                    baseUrl,
                    "https://blazordiffusion.com",
                    "https://www.blazordiffusion.com",
                    "https://pub-e17dff5b2d09437a97efdbb7f6ee3701.r2.dev", // CDN diffusion-client public bucket
                }, allowCredentials: true));

            // set in launchSettings.json
            var r2AccessId = Environment.GetEnvironmentVariable("R2_ACCESS_KEY_ID")!;
            var r2AccessKey = Environment.GetEnvironmentVariable("R2_SECRET_ACCESS_KEY")!;

            var appConfig = AppConfig.Set(new AppConfig
            {
                BaseUrl = baseUrl,
                ApiBaseUrl = apiUrl ?? baseUrl,
                WwwBaseUrl = cdnUrl != null ? $"https://blazordiffusion.netcore.io" : baseUrl,
                R2Account = "b95f38ca3a6ac31ea582cd624e6eb385",
                R2AccessId = r2AccessId,
                R2AccessKey = r2AccessKey,
                ArtifactBucket = "diffusion",
                CdnBucket = "diffusion-client",
                AssetsBasePath = r2AccessId != null ? "https://cdn.diffusion.works" : "/uploads",
                FallbackAssetsBasePath =
                    r2AccessId != null ? "https://pub-97bba6b94a944260b10a6e7d4bf98053.r2.dev" : "/uploads",
                SyncTasksInterval = TimeSpan.FromHours(24),
                DisableWrites = true,
            });

            services.AddSingleton(appConfig);

            var hasR2 = !string.IsNullOrEmpty(r2AccessId);
            if (!hasR2)
                Log.Warn($"Starting without R2 access.");

            var s3Client = new AmazonS3Client(appConfig.R2AccessId, appConfig.R2AccessKey, new AmazonS3Config
            {
                ServiceURL = $"https://{appConfig.R2Account}.r2.cloudflarestorage.com"
            });
            services.AddSingleton(s3Client);
            var localFs =
                new FileSystemVirtualFiles(context.HostingEnvironment.ContentRootPath.CombineWith("App_Files").AssertDir());
            var appFs = VirtualFiles = hasR2 ? new R2VirtualFiles(s3Client, appConfig.ArtifactBucket) : localFs;
            services.AddPlugin(new FilesUploadFeature(
                new UploadLocation("artifacts", appFs,
                    readAccessRole: RoleNames.AllowAnon,
                    maxFileBytes: AppData.MaxArtifactSize),
                new UploadLocation("avatars", appFs, allowExtensions: FileExt.WebImages,
                    // Use unique URL to invalidate CDN caches
                    resolvePath: ctx => X.Map((CustomUserSession)ctx.Session,
                        x => $"/avatars/{x.RefIdStr[..2]}/{x.RefIdStr}/{ctx.FileName}")!,
                    maxFileBytes: AppData.MaxAvatarSize,
                    transformFile: ImageUtils.TransformAvatarAsync)
            ));

            // Don't use public prefix if working locally
            services.AddSingleton<IStableDiffusionClient>(new DreamStudioClient
            {
                ApiKey = Environment.GetEnvironmentVariable("DREAMAI_APIKEY") ?? "<your_api_key>",
                OutputPathPrefix = "artifacts",
                PublicPrefix = appConfig.AssetsBasePath,
                VirtualFiles = appFs
            });
        });
    
    public AppHost() : base("BlazorDiffusion", typeof(MyServices).Assembly) { }

    // Configure your AppHost with the necessary configuration and dependencies your App needs
    public override void Configure(Container container)
    {
        SetConfig(new HostConfig {
            IgnorePathInfoPrefixes = { "/appsettings", "/_framework" },
        });

        ScriptContext.Args[nameof(AppData)] = AppData.Instance;
    }
    
    public static void RegisterKey() =>
        ServiceStack.Licensing.RegisterLicense("OSS GPL-3.0 2024 https://github.com/NetCoreApps/BlazorDiffusionAuto pBDTGJjpThY+Q87ldigaTPN5cKuCGyTdH7a9C6yT4q3ltfWWFydwNz/rhECDTKwIxdgqHNX4Xfhb7b2+2rkZAQaT8/CXH/7xJcITyKVQmtd0BKyXTgEQXUm1MFxQuyaT4+UzmkKU6iGrfQB4jH3u2JS6gDDi/RABOgszzBTDyXE=");
}
