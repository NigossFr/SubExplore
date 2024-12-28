using Microsoft.Extensions.Logging;
using SubExplore.Services.Authentication;
using SubExplore.Services.Navigation;
using SubExplore.Services.Settings;
using SubExplore.ViewModels;
using SubExplore.Services.Spots;
using SubExplore.Services.Storage;
using SubExplore.Services.Moderation;
using SubExplore.Services.Organizations;
using SubExplore.WinUI;
using Windows.Networking.NetworkOperators;

namespace SubExplore;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps() // Ajout du support des cartes
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configuration des services HTTP
        builder.Services.AddHttpClient("SubExploreAPI", client =>
        {
            client.BaseAddress = new Uri(ApiEndpoints.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Services Core
        ConfigureCoreServices(builder.Services);

        // Services Métier
        ConfigureBusinessServices(builder.Services);

        // ViewModels
        ConfigureViewModels(builder.Services);

        // Pages
        ConfigurePages(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void ConfigureCoreServices(IServiceCollection services)
    {
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ILocationService, LocationService>();
    }

    private static void ConfigureBusinessServices(IServiceCollection services)
    {
        services.AddSingleton<ISpotService, SpotService>();
        services.AddSingleton<IMediaService, MediaService>();
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
    }
}
