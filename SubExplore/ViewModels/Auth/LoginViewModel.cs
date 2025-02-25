using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Extensions;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Auth;
using SubExplore.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubExplore.ViewModels.Auth
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISecureStorageService _secureStorageService;

        [ObservableProperty]
        private LoginModel _loginModel;

        [ObservableProperty]
        private bool _isPasswordVisible;

        [ObservableProperty]
        private string _loginButtonText = "Se connecter";

        [ObservableProperty]
        private bool _isSocialLoginEnabled;

        [ObservableProperty]
        private string _passwordVisibilityIcon = "eye.png";

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsNotLoading => !IsBusy;

        public LoginViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            ISecureStorageService secureStorageService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _secureStorageService = secureStorageService;
            _loginModel = new LoginModel();
            Title = "Connexion";

            // Vérifier si l'authentification sociale est disponible
            CheckSocialLoginAvailability();
        }

        private async void CheckSocialLoginAvailability()
        {
            // Vérifiez si les fournisseurs d'authentification sociale sont disponibles sur l'appareil
            try
            {
                // Cette vérification dépendra de l'implémentation de votre service d'authentification
                IsSocialLoginEnabled = await _authenticationService.AreSocialProvidersAvailableAsync();
            }
            catch
            {
                IsSocialLoginEnabled = false;
            }
        }

        [RelayCommand]
        private void TogglePassword()
        {
            IsPasswordVisible = !IsPasswordVisible;
            PasswordVisibilityIcon = IsPasswordVisible ? "eye_off.png" : "eye.png";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            // Validation des entrées
            if (!ValidateInputs()) return;

            loginButtonText = "Connexion en cours...";
            await SafeExecuteAsync(async () =>
            {
                // Appel au service d'authentification
                var result = await _authenticationService.LoginAsync(LoginModel.Email, LoginModel.Password);

                if (result == null)
                {
                    ErrorMessage = "Email ou mot de passe incorrect";
                    return;
                }

                // Enregistrer les identifiants si "Se souvenir de moi" est coché
                if (LoginModel.RememberMe)
                {
                    await _secureStorageService.SetAsync("saved_email", LoginModel.Email);
                }
                else
                {
                    // Supprimer les identifiants enregistrés
                    await _secureStorageService.RemoveAsync("saved_email");
                }

                // Redirection vers la page principale
                await NavigationService.NavigateToAsync("///map");
            });

            LoginButtonText = "Se connecter";
        }

        private bool ValidateInputs()
        {
            ClearError();

            if (string.IsNullOrWhiteSpace(LoginModel.Email))
            {
                ErrorMessage = "Veuillez entrer votre adresse email";
                return false;
            }

            if (!LoginModel.Email.IsValidEmail())
            {
                ErrorMessage = "Format d'email invalide";
                return false;
            }

            if (string.IsNullOrWhiteSpace(LoginModel.Password))
            {
                ErrorMessage = "Veuillez entrer votre mot de passe";
                return false;
            }

            return true;
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await NavigationService.NavigateToAsync("register");
        }

        [RelayCommand]
        private async Task NavigateToForgotPasswordAsync()
        {
            await NavigationService.NavigateToAsync("forgot-password");
        }

        [RelayCommand]
        private async Task LoginWithProviderAsync(string provider)
        {
            if (IsBusy) return;

            await SafeExecuteAsync(async () =>
            {
                var result = await _authenticationService.LoginWithOAuthAsync(provider, null);

                if (result == null)
                {
                    ErrorMessage = $"La connexion avec {provider} a échoué";
                    return;
                }

                // Redirection vers la page principale
                await NavigationService.NavigateToAsync("///map");
            });
        }

        public override async Task InitializeAsync(IDictionary<string, object> parameters)
        {
            // Récupérer l'email enregistré si disponible
            var savedEmail = await _secureStorageService.GetAsync("saved_email");

            if (!string.IsNullOrEmpty(savedEmail))
            {
                LoginModel.Email = savedEmail;
                LoginModel.RememberMe = true;
            }
        }
    }
}