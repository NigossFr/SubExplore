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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubExplore.ViewModels.Auth
{
    public partial class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;

        [ObservableProperty]
        private ForgotPasswordModel _forgotPasswordModel;

        [ObservableProperty]
        private string _submitButtonText = "Envoyer le lien de réinitialisation";

        [ObservableProperty]
        private bool _isEmailSent;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmailValid))]
        [NotifyPropertyChangedFor(nameof(HasEmailError))]
        private string _emailValidationMessage;

        [ObservableProperty]
        private string _successMessage;

        public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);
        public bool IsEmailValid => string.IsNullOrEmpty(EmailValidationMessage);
        public bool HasEmailError => !string.IsNullOrEmpty(EmailValidationMessage);

        public ForgotPasswordViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _forgotPasswordModel = new ForgotPasswordModel();
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
                var result = await _authenticationService.GeneratePasswordResetTokenAsync(ForgotPasswordModel.Email);

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

            if (!ForgotPasswordModel.Email.IsValidEmail())
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
            if (string.IsNullOrWhiteSpace(ForgotPasswordModel.Email) || !ForgotPasswordModel.Email.IsValidEmail())
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