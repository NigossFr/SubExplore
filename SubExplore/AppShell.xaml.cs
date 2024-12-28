namespace SubExplore
{
    public partial class AppShell : Shell
    {
        private readonly AppShellViewModel _viewModel;

        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            // Routes d'authentification
            Routing.RegisterRoute("login", typeof(Views.Auth.LoginPage));
            Routing.RegisterRoute("register", typeof(Views.Auth.RegisterPage));
            Routing.RegisterRoute("forgot-password", typeof(Views.Auth.ForgotPasswordPage));

            // Routes principales
            Routing.RegisterRoute("spot-details", typeof(Views.Main.SpotDetailsPage));
            Routing.RegisterRoute("add-spot", typeof(Views.Main.AddSpotPage));
            Routing.RegisterRoute("structure-details", typeof(Views.Main.StructureDetailsPage));
            Routing.RegisterRoute("story-details", typeof(Views.Main.StoryDetailsPage));

            // Routes de paramètres
            Routing.RegisterRoute("settings", typeof(Views.Settings.SettingsPage));
            Routing.RegisterRoute("profile-edit", typeof(Views.Profile.ProfileEditPage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            // Vérifier si la route nécessite une authentification
            if (RequiresAuthentication(args.Target.Location.OriginalString))
            {
                if (!_viewModel.IsAuthenticated)
                {
                    args.Cancel();
                    Shell.Current.GoToAsync("///login");
                }
            }
        }

        private bool RequiresAuthentication(string route)
        {
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
}
