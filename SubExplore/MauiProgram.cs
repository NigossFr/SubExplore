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
using Microsoft.EntityFrameworkCore;
using SubExplore.Data;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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

        // Chargement de la configuration
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("SubExplore.appsettings.json");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        // Configuration des services
        RegisterServices(builder.Services, configuration);

        // Configuration des ViewModels
        RegisterViewModels(builder.Services);

        // Configuration des pages
        RegisterPages(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Initialiser la base de données si nécessaire
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
                seedService.SeedDatabaseAsync().Wait();
            }
            catch (Exception ex)
            {
                // Logger l'erreur d'initialisation de la BDD
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<App>>();
                logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données.");
            }
        }

        return app;
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration de la base de données
        ConfigureDatabase(services, configuration);

        // Services essentiels
        services.AddSingleton<IConnectivity>(Connectivity.Current);
        services.AddSingleton<IGeolocation>(Geolocation.Default);
        services.AddSingleton<ISecureStorage>(SecureStorage.Default);
        services.AddMemoryCache();

        // Service pour l'initialisation des données
        services.AddTransient<SeedService>();

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
            client.BaseAddress = new Uri(configuration["AppSettings:ApiBaseUrl"] ?? "https://api.subexplore.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["DatabaseProvider"] ?? "MySQL";
        var environmentName = GetEnvironment();
        var connectionString = configuration.GetConnectionString(environmentName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Chaîne de connexion pour l'environnement {environmentName} non trouvée dans la configuration.");
        }

        switch (provider)
        {
            case "MySQL":
                services.AddDbContext<SubExploreDbContext>(options =>
                    options.UseMySql(connectionString,
                        ServerVersion.AutoDetect(connectionString)));
                break;

            case "AzureSQL":
                services.AddDbContext<SubExploreDbContext>(options =>
                    options.UseSqlServer(connectionString));
                break;

            default:
                throw new NotSupportedException($"Le fournisseur de base de données '{provider}' n'est pas pris en charge.");
        }
    }

    private static string GetEnvironment()
    {
#if DEBUG
        return "Development";
#else
        return "Production";
#endif
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