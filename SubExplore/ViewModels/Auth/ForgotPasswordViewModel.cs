using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Extensions;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;

namespace SubExplore.ViewModels.Auth
{
    public partial class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;

        [ObservableProperty]
        private ForgotPasswordModel _forgotPasswordModel = new ForgotPasswordModel();

        [ObservableProperty]
        private string _submitButtonText = "Envoyer le lien de réinitialisation";

        [ObservableProperty]
        private bool _isEmailSent;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmailValid))]
        [NotifyPropertyChangedFor(nameof(HasEmailError))]
        private string _emailValidationMessage = string.Empty;

        [ObservableProperty]
        private string _successMessage = string.Empty;

        // Ajout de cette ligne pour résoudre l'erreur XFC0045
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string _errorMessage = string.Empty;

        public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);
        public bool IsEmailValid => string.IsNullOrEmpty(EmailValidationMessage);
        public bool HasEmailError => !string.IsNullOrEmpty(EmailValidationMessage);
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ForgotPasswordViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            Title = "Mot de passe oublié";
        }

        [RelayCommand]
        private async Task SendResetLinkAsync()
        {
            if (IsBusy) return;

            // Validation de l'email
            if (!ValidateEmail()) return;

            SubmitButtonText = "Envoi en cours...";
            await SafeExecuteAsync(async () =>
            {
                // Appel au service d'authentification
                bool result = await _authenticationService.GeneratePasswordResetTokenAsync(ForgotPasswordModel.Email) != null;

                if (result)
                {
                    IsEmailSent = true;
                    SuccessMessage = $"Un lien de réinitialisation a été envoyé à {ForgotPasswordModel.Email}. Vérifiez votre boîte de réception (et vos spams).";
                    ClearError();
                }
                else
                {
                    ErrorMessage = "Impossible d'envoyer l'email de réinitialisation. Vérifiez votre adresse email.";
                }
            });

            SubmitButtonText = "Envoyer le lien de réinitialisation";
        }

        private bool ValidateEmail()
        {
            EmailValidationMessage = string.Empty;
            ClearError();

            if (string.IsNullOrWhiteSpace(ForgotPasswordModel.Email))
            {
                EmailValidationMessage = "L'email est requis";
                return false;
            }

            if (!Extensions.ValidationExtensions.IsValidEmail(ForgotPasswordModel.Email))
            {
                EmailValidationMessage = "Format d'email invalide";
                return false;
            }

            return true;
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await NavigationService.NavigateToAsync("login");
        }

        [RelayCommand]
        private async Task CheckEmailExistsAsync()
        {
            if (string.IsNullOrWhiteSpace(ForgotPasswordModel.Email) ||
                !Extensions.ValidationExtensions.IsValidEmail(ForgotPasswordModel.Email))
                return;

            try
            {
                var exists = await _authenticationService.CheckEmailExistsAsync(ForgotPasswordModel.Email);
                if (!exists)
                {
                    EmailValidationMessage = "Aucun compte n'est associé à cette adresse email";
                }
                else
                {
                    EmailValidationMessage = string.Empty;
                }
            }
            catch
            {
                // Ignorer les erreurs de vérification
            }
        }

        public override Task InitializeAsync(IDictionary<string, object> parameters)
        {
            // Réinitialiser le modèle
            ForgotPasswordModel = new ForgotPasswordModel();

            // Réinitialiser les états
            IsEmailSent = false;
            SuccessMessage = string.Empty;
            EmailValidationMessage = string.Empty;
            ClearError();

            return base.InitializeAsync(parameters);
        }
    }
}