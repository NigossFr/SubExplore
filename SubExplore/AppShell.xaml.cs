using SubExplore.Services.Interfaces;
using SubExplore.Views.Profile;
using SubExplore.Views.Settings;

namespace SubExplore;

public partial class AppShell : Shell
{
    private readonly IAuthenticationService _authService;

    public AppShell(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;

        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        // Routes d'authentification
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("forgot-password", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("reset-password-confirmation", typeof(ResetPasswordConfirmationPage));

        // Routes principales
        Routing.RegisterRoute("spot-details", typeof(SpotDetailsPage));
        Routing.RegisterRoute("add-spot", typeof(AddSpotPage));
        Routing.RegisterRoute("structure-details", typeof(StructureDetailsPage));
        Routing.RegisterRoute("story-details", typeof(StoryDetailsPage));

        // Routes de paramètres
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("profile-edit", typeof(ProfileEditPage));
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        var currentUser = await _authService.GetCurrentUserAsync();
        var route = args.Target?.Location?.OriginalString;

        // Liste des routes nécessitant une authentification
        var protectedRoutes = new[] {
            "profile",
            "profile-edit",
            "add-spot",
            "settings"
        };

        if (protectedRoutes.Any(r => route?.Contains(r, StringComparison.OrdinalIgnoreCase) == true))
        {
            if (currentUser == null)
            {
                args.Cancel();
                await GoToAsync("///login");
            }
        }
    }

    protected override bool OnBackButtonPressed()
    {
        var currentRoute = Current?.CurrentState?.Location?.OriginalString;

        // Empêcher le retour sur certaines pages
        if (currentRoute?.Contains("login") == true && !Current.Navigation.NavigationStack.Any())
        {
            return true;
        }

        return base.OnBackButtonPressed();
    }
}