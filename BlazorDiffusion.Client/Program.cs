using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorDiffusion.Client;
using BlazorDiffusion.Client.UI;
using BlazorDiffusion.ServiceModel;
using Ljbc1994.Blazor.IntersectionObserver;
using ServiceStack;
using ServiceStack.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// Use / for local or CDN resources
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddBlazorApiClient(apiBaseUrl);
builder.Services.AddLocalStorage();
builder.Services.AddScoped<KeyboardNavigation>();
builder.Services.AddScoped<UserState>();
builder.Services.AddIntersectionObserver();

var app = builder.Build();

BlazorConfig.Set(new BlazorConfig
{
    IsWasm = true,
    Services = app.Services,
    ApiBaseUrl = apiBaseUrl,
    AssetsBasePath = "https://cdn.diffusion.works",
    FallbackAssetsBasePath = "https://pub-97bba6b94a944260b10a6e7d4bf98053.r2.dev",
    EnableLogging = true,
    EnableVerboseLogging = builder.HostEnvironment.IsDevelopment(),
    DefaultProfileUrl = Icons.AnonUserUri,
    OnApiErrorAsync = (request, apiError) =>
    {
        BlazorConfig.Instance.GetLog()?.LogDebug("\n\nOnApiErrorAsync(): {0}", apiError.Error.GetDetailedError());
        return Task.CompletedTask;
    }
});

await app.RunAsync();
