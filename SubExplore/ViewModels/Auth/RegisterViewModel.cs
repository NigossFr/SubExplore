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
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;

        [ObservableProperty]
        private RegistrationModel _registrationModel;

        [ObservableProperty]
        private bool _isPasswordVisible;

        [ObservableProperty]
        private bool _isConfirmPasswordVisible;

        [ObservableProperty]
        private string _registerButtonText = "Créer mon compte";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmailValid))]
        [NotifyPropertyChangedFor(nameof(HasEmailError))]
        private string _emailValidationMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUsernameValid))]
        [NotifyPropertyChangedFor(nameof(HasUsernameError))]
        private string _usernameValidationMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPasswordValid))]
        [NotifyPropertyChangedFor(nameof(HasPasswordError))]
        private string _passwordValidationMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPasswordsMatch))]
        [NotifyPropertyChangedFor(nameof(HasPasswordMatchError))]
        private string _passwordMatchValidationMessage;

        [ObservableProperty]
        private string _passwordStrength;

        [ObservableProperty]
        private string _passwordVisibilityIcon = "eye.png";

        [ObservableProperty]
        private string _confirmPasswordVisibilityIcon = "eye.png";

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsEmailValid => string.IsNullOrEmpty(EmailValidationMessage);
        public bool IsUsernameValid => string.IsNullOrEmpty(UsernameValidationMessage);
        public bool IsPasswordValid => string.IsNullOrEmpty(PasswordValidationMessage);
        public bool IsPasswordsMatch => string.IsNullOrEmpty(PasswordMatchValidationMessage);

        public bool HasEmailError => !string.IsNullOrEmpty(EmailValidationMessage);
        public bool HasUsernameError => !string.IsNullOrEmpty(UsernameValidationMessage);
        public bool HasPasswordError => !string.IsNullOrEmpty(PasswordValidationMessage);
        public bool HasPasswordMatchError => !string.IsNullOrEmpty(PasswordMatchValidationMessage);

        public RegisterViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _registrationModel = new RegistrationModel();
            Title = "Inscription";

            // Initialiser les messages de validation
            ClearValidationMessages();
        }

        private void ClearValidationMessages()
        {
            EmailValidationMessage = string.Empty;
            UsernameValidationMessage = string.Empty;
            PasswordValidationMessage = string.Empty;
            PasswordMatchValidationMessage = string.Empty;
            PasswordStrength = string.Empty;
        }

        [RelayCommand]
        private void TogglePassword()
        {
            IsPasswordVisible = !IsPasswordVisible;
            PasswordVisibilityIcon = IsPasswordVisible ? "eye_off.png" : "eye.png";
        }

        [RelayCommand]
        private void ToggleConfirmPassword()
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
            ConfirmPasswordVisibilityIcon = IsConfirmPasswordVisible ? "eye_off.png" : "eye.png";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            // Validation complète des entrées
            if (!await ValidateAllInputsAsync()) return;

            RegisterButtonText = "Création en cours...";
            await SafeExecuteAsync(async () =>
            {
                // Créer l'objet de requête pour l'inscription
                var userCreation = new RegistrationRequest
                {
                    Email = RegistrationModel.Email,
                    Username = RegistrationModel.Username,
                    Password = RegistrationModel.Password,
                    FirstName = RegistrationModel.FirstName,
                    LastName = RegistrationModel.LastName
                };

                // Appel au service d'authentification
                var result = await _authenticationService.RegisterAsync(userCreation);

                if (result)
                {
                    await DisplayAlert("Inscription réussie",
                        "Votre compte a été créé avec succès. Un email de confirmation vous a été envoyé.",
                        "OK");

                    // Naviguer vers la page de connexion
                    await NavigationService.NavigateToAsync("login");
                }
                else
                {
                    ErrorMessage = "L'inscription a échoué";
                }
            });

            RegisterButtonText = "Créer mon compte";
        }

        private async Task<bool> ValidateAllInputsAsync()
        {
            ClearError();

            bool isValid = true;

            // Valider tous les champs
            isValid &= await ValidateEmailAsync();
            isValid &= await ValidateUsernameAsync();
            isValid &= ValidatePassword();
            isValid &= ValidatePasswordsMatch();
            isValid &= ValidateRequiredFields();
            isValid &= ValidateTermsAccepted();

            return isValid;
        }

        private bool ValidateRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(RegistrationModel.FirstName) ||
                string.IsNullOrWhiteSpace(RegistrationModel.LastName))
            {
                ErrorMessage = "Veuillez remplir tous les champs obligatoires";
                return false;
            }

            return true;
        }

        private bool ValidateTermsAccepted()
        {
            if (!RegistrationModel.AcceptTerms)
            {
                ErrorMessage = "Vous devez accepter les conditions d'utilisation";
                return false;
            }

            return true;
        }

        [RelayCommand]
        private async Task ValidateEmailAsync()
        {
            // Vider le message d'erreur précédent
            EmailValidationMessage = string.Empty;

            // Vérifier si l'email est vide
            if (string.IsNullOrWhiteSpace(RegistrationModel.Email))
            {
                EmailValidationMessage = "L'email est requis";
                return false;
            }

            // Vérifier le format de l'email
            if (!RegistrationModel.Email.IsValidEmail())
            {
                EmailValidationMessage = "Format d'email invalide";
                return false;
            }

            // Vérifier si l'email est déjà utilisé
            var isAvailable = await _authenticationService.IsEmailAvailableAsync(RegistrationModel.Email);
            if (!isAvailable)
            {
                EmailValidationMessage = "Cet email est déjà utilisé";
                return false;
            }

            return true;
        }

        [RelayCommand]
        private async Task ValidateUsernameAsync()
        {
            // Vider le message d'erreur précédent
            UsernameValidationMessage = string.Empty;

            // Vérifier si le nom d'utilisateur est vide
            if (string.IsNullOrWhiteSpace(RegistrationModel.Username))
            {
                UsernameValidationMessage = "Le nom d'utilisateur est requis";
                return false;
            }

            // Vérifier le format du nom d'utilisateur
            if (!RegistrationModel.Username.IsValidUsername())
            {
                UsernameValidationMessage = "Le nom d'utilisateur doit contenir entre 3 et 30 caractères, uniquement des lettres, chiffres, tirets et underscores";
                return false;
            }

            // Vérifier si le nom d'utilisateur est déjà utilisé
            var isAvailable = await _authenticationService.IsUsernameAvailableAsync(RegistrationModel.Username);
            if (!isAvailable)
            {
                UsernameValidationMessage = "Ce nom d'utilisateur est déjà utilisé";
                return false;
            }

            return true;
        }

        [RelayCommand]
        private bool ValidatePassword()
        {
            // Vider le message d'erreur précédent
            PasswordValidationMessage = string.Empty;

            // Vérifier si le mot de passe est vide
            if (string.IsNullOrWhiteSpace(RegistrationModel.Password))
            {
                PasswordValidationMessage = "Le mot de passe est requis";
                return false;
            }

            // Vérifier la force du mot de passe
            var (isValid, strength) = RegistrationModel.Password.GetPasswordStrength();
            PasswordStrength = strength;

            if (!isValid)
            {
                var validation = RegistrationModel.Password.ValidatePassword();
                PasswordValidationMessage = validation.Message;
                return false;
            }

            return true;
        }

        [RelayCommand]
        private bool ValidatePasswordsMatch()
        {
            // Vider le message d'erreur précédent
            PasswordMatchValidationMessage = string.Empty;

            // Vérifier si les mots de passe correspondent
            if (RegistrationModel.Password != RegistrationModel.ConfirmPassword)
            {
                PasswordMatchValidationMessage = "Les mots de passe ne correspondent pas";
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
        private async Task ShowTermsAndConditionsAsync()
        {
            await NavigationService.NavigateToAsync("terms-and-conditions");
        }

        public override Task InitializeAsync(IDictionary<string, object> parameters)
        {
            // Réinitialiser le modèle
            RegistrationModel = new RegistrationModel();

            // Réinitialiser les messages de validation
            ClearValidationMessages();
            ClearError();

            return base.InitializeAsync(parameters);
        }
    }

    public class RegistrationRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}