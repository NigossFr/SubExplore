using Microsoft.Extensions.Logging;
using SubExplore.Services.Interfaces;
using SubExplore.Services.Implementations;
using SubExplore.ViewModels;
using SubExplore.Views;
using SubExplore.Views.Auth;
using SubExplore.Views.Main;
using Microsoft.Extensions.Caching.Memory;
using CommunityToolkit.Maui;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.JSInterop;

namespace SubExplore;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Ajout du support WebView/React
        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        ConfigureServices(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Services essentiels
        ConfigureEssentialServices(services);

        // Configuration HTTP
        ConfigureHttpClients(services);

        // Services de l'application
        ConfigureApplicationServices(services);

        // ViewModels
        ConfigureViewModels(services);

        // Pages
        ConfigurePages(services);
    }

    private static void ConfigureEssentialServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IConnectivity>(Connectivity.Current);
        services.AddSingleton<IGeolocation>(Geolocation.Default);
        services.AddSingleton<IMap>(Map.Default);
        services.AddSingleton<ISecureStorage>(SecureStorage.Default);
    }

    private static void ConfigureHttpClients(IServiceCollection services)
    {
        // Handler pour l'authentification
        services.AddTransient<AuthenticationDelegatingHandler>();

        // Client HTTP principal
        services.AddHttpClient("SubExploreAPI", client =>
        {
            client.BaseAddress = new Uri(ApiEndpoints.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthenticationDelegatingHandler>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddPolicyHandler(GetRetryPolicy());
    }

    private static void ConfigureApplicationServices(IServiceCollection services)
    {
        // Services Core
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISecureStorageService, SecureStorageService>();
        services.AddSingleton<ISettingsService, SettingsService>();

        // Services Métier
        services.AddSingleton<ILocationService, LocationService>();
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<ISpotService, SpotService>();
        services.AddSingleton<IOrganizationService, OrganizationService>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        // Shell
        services.AddSingleton<AppShellViewModel>();

        // Auth ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<ForgotPasswordViewModel>();

        // Main ViewModels
        services.AddTransient<MapViewModel>();
        services.AddTransient<SpotDetailsViewModel>();
        services.AddTransient<AddSpotViewModel>();
        services.AddTransient<MagazineViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<StructuresViewModel>();

        // Settings ViewModels
        services.AddTransient<SettingsViewModel>();
    }

    private static void ConfigurePages(IServiceCollection services)
    {
        // Shell
        services.AddTransient<AppShell>();

        // Auth Pages
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<ForgotPasswordPage>();

        // Main Pages
        services.AddTransient<MapPage>();
        services.AddTransient<SpotDetailsPage>();
        services.AddTransient<AddSpotPage>();
        services.AddTransient<MagazinePage>();
        services.AddTransient<ProfilePage>();
        services.AddTransient<StructuresPage>();

        // Settings Pages
        services.AddTransient<SettingsPage>();
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}

public static class ApiEndpoints
{
    public static string BaseUrl
    {
#if DEBUG
        get => "https://api-dev.subexplore.com/";
#else
        get => "https://api.subexplore.com/";
#endif
    }
}