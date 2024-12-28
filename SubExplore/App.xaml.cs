using SubExplore.ViewModels;
using Windows.UI.ApplicationSettings;

namespace SubExplore;

public partial class AppShell : Shell
{
    private readonly AppShellViewModel _viewModel;

    public AppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Enregistrer les routes pour la navigation
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        // Routes d'authentification
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("forgot-password", typeof(ForgotPasswordPage));

        // Routes principales
        Routing.RegisterRoute("spot-details", typeof(SpotDetailsPage));
        Routing.RegisterRoute("add-spot", typeof(AddSpotPage));
        Routing.RegisterRoute("structure-details", typeof(StructureDetailsPage));
        Routing.RegisterRoute("story-details", typeof(StoryDetailsPage));

        // Routes de paramètres
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("profile-edit", typeof(ProfileEditPage));
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        // Vérifier si la route nécessite une authentification
        if (RequiresAuthentication(args.Target.Location.OriginalString))
        {
            // Si l'utilisateur n'est pas connecté, rediriger vers la page de connexion
            if (!_viewModel.IsUserLoggedIn)
            {
                args.Cancel();
                Shell.Current.GoToAsync("///login");
            }
        }
    }

    private bool RequiresAuthentication(string route)
    {
        // Liste des routes qui nécessitent une authentification
        var protectedRoutes = new[]
        {
            "profile",
            "profile-edit",
            "add-spot",
            "settings"
        };

        return protectedRoutes.Any(r => route.Contains(r, StringComparison.OrdinalIgnoreCase));
    }
}
