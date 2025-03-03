using Microsoft.Extensions.Logging;
using SubExplore.Services.Interfaces;
using SubExplore.Services.Implementations;
using SubExplore.ViewModels;
using SubExplore.ViewModels.Auth;
using SubExplore.ViewModels.Main;
using SubExplore.ViewModels.Spot;
using SubExplore.ViewModels.Profile;
using SubExplore.ViewModels.Settings;
using SubExplore.Views.Auth;
using SubExplore.Views.Main;
using SubExplore.Views.Spot;
using SubExplore.Views.Profile;
using SubExplore.Views.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using SubExplore.Models;
using CommunityToolkit.Maui;

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

        // Configuration des services
        RegisterServices(builder.Services);

        // Configuration des ViewModels
        RegisterViewModels(builder.Services);

        // Configuration des pages
        RegisterPages(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Services essentiels
        services.AddSingleton<IConnectivity>(Connectivity.Current);
        services.AddSingleton<IGeolocation>(Geolocation.Default);
        services.AddSingleton<ISecureStorage>(SecureStorage.Default);
        services.AddMemoryCache();

        // Services Core
        services.AddSingleton<ISecureStorageService, SecureStorageService>();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IConnectivityService, ConnectivityService>();

        // Services de base pour les données
        services.AddSingleton<ILocationService, LocationService>();
        services.AddSingleton<ISpotService, SpotService>();
        // services.AddSingleton<IMediaService, MediaService>();

        // Client HTTP
        services.AddHttpClient("SubExploreAPI", client =>
        {
            client.BaseAddress = new Uri("https://api.subexplore.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        // Shell ViewModel
        services.AddSingleton<AppShellViewModel>();

        // Auth ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<ForgotPasswordViewModel>();
        services.AddTransient<ResetPasswordConfirmationViewModel>();

        // Main ViewModels
        services.AddTransient<MapViewModel>();
        services.AddTransient<SpotDetailsViewModel>();
        services.AddTransient<StructureDetailsViewModel>();
        services.AddTransient<StoryDetailsViewModel>();

        // Spot ViewModels
        services.AddTransient<AddSpotViewModel>();

        // Profile ViewModels
        services.AddTransient<ProfileEditViewModel>();

        // Settings ViewModels
        services.AddTransient<SettingsViewModel>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        // Shell
        services.AddTransient<AppShell>();

        // Auth Pages
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<ForgotPasswordPage>();
        services.AddTransient<ResetPasswordConfirmationPage>();

        // Main Pages
        services.AddTransient<MapPage>();
        services.AddTransient<SpotDetailsPage>();
        services.AddTransient<StructureDetailsPage>();
        services.AddTransient<StoryDetailsPage>();

        // Spot Pages
        services.AddTransient<AddSpotPage>();

        // Profile Pages
        services.AddTransient<ProfileEditPage>();

        // Settings Pages
        services.AddTransient<SettingsPage>();
    }
}