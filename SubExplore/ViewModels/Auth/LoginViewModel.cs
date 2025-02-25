using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;
using SubExplore.ViewModels.Auth;

namespace SubExplore.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISecureStorageService _secureStorageService;

        [ObservableProperty]
        private LoginModel _loginModel;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        public LoginViewModel(
            IAuthenticationService authenticationService,
            ISecureStorageService secureStorageService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _secureStorageService = secureStorageService;
            _loginModel = new LoginModel();
        }

        public override async Task InitializeAsync(IDictionary<string, object> parameters)
        {
            // Vérifier s'il y a des identifiants sauvegardés
            var savedEmail = await _secureStorageService.GetSecureStorageAsync<string>("saved_email");
            if (!string.IsNullOrEmpty(savedEmail))
            {
                LoginModel.Email = savedEmail;
                LoginModel.RememberMe = true;
            }
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(LoginModel.Email) || string.IsNullOrEmpty(LoginModel.Password))
                {
                    ErrorMessage = "Veuillez remplir tous les champs";
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                var result = await _authenticationService.LoginAsync(LoginModel.Email, LoginModel.Password);

                if (LoginModel.RememberMe)
                {
                    await _secureStorageService.SetSecureStorageAsync("saved_email", LoginModel.Email);
                }

                // Rediriger vers la page principale
                await NavigationService.NavigateToAsync("///map");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur de connexion. Veuillez vérifier vos identifiants.";
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
#endif
            }
            finally
            {
                IsLoading = false;
            }
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
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // TODO: Implémenter la logique de connexion OAuth
                var token = await GetProviderTokenAsync(provider);
                if (string.IsNullOrEmpty(token))
                {
                    ErrorMessage = "Échec de l'authentification avec le fournisseur";
                    return;
                }

                var result = await _authenticationService.LoginWithOAuthAsync(provider, token);
                await NavigationService.NavigateToAsync("///map");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de la connexion avec le fournisseur";
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"OAuth login error: {ex}");
#endif
            }
            finally
            {
                IsLoading = false;
