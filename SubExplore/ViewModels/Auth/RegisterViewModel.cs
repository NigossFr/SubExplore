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
        private string _emailValidationMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUsernameValid))]
        [NotifyPropertyChangedFor(nameof(HasUsernameError))]
        private string _usernameValidationMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPasswordValid))]
        [NotifyPropertyChangedFor(nameof(HasPasswordError))]
        private string _passwordValidationMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPasswordsMatch))]
        [NotifyPropertyChangedFor(nameof(HasPasswordMatchError))]
        private string _passwordMatchValidationMessage = string.Empty;

        [ObservableProperty]
        private string _passwordStrength = string.Empty;

        [ObservableProperty]
        private string _passwordVisibilityIcon = "eye.png";

        [ObservableProperty]
        private string _confirmPasswordVisibilityIcon = "eye.png";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string _errorMessage = string.Empty;

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
                // Utiliser une approche directe sans utiliser RegistrationRequest
                // On passe directement les paramètres via la méthode PostAsJsonAsync 
                // qui va créer un objet anonyme adapté au service web
                var email = RegistrationModel.Email;
                var username = RegistrationModel.Username;
                var password = RegistrationModel.Password;
                var firstName = RegistrationModel.FirstName;
                var lastName = RegistrationModel.LastName;

                // On crée un objet simple sans utiliser la classe RegistrationRequest
                var request = new
                {
                    Email = email,
                    Username = username,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName
                };

                // Appeler RegisterAsync avec l'objet simple
                var result = await RegisterUserAsync(request);

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

        // Méthode intermédiaire qui fait le pont avec le service d'authentification
        private async Task<bool> RegisterUserAsync(object registrationData)
        {
            // Cette méthode va charger le type RegistrationRequest depuis le service
            using var httpClient = new HttpClient();
            var url = "api/auth/register"; // Ajustez selon votre API

            try
            {
                // Envoi direct de l'objet anonyme sans utiliser le type RegistrationRequest
                var response = await httpClient.PostAsJsonAsync(url, registrationData);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> ValidateAllInputsAsync()
        {
            ClearError();

            bool isValid = true;

            // Valider tous les champs
            isValid &= await ValidateEmailInternalAsync();
            isValid &= await ValidateUsernameInternalAsync();
            isValid &= await ValidatePasswordInternalAsync();
            isValid &= await ValidatePasswordsMatchInternalAsync();
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

        // Méthode pour le binding XAML
        [RelayCommand]
        private async Task ValidateEmailAsync()
        {
            await ValidateEmailInternalAsync();
        }

        // Méthode interne avec retour booléen pour validation
        private async Task<bool> ValidateEmailInternalAsync()
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
            var isValidEmail = ValidationExtensions.IsValidEmail(RegistrationModel.Email);
            if (!isValidEmail)
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

        // Méthode pour le binding XAML
        [RelayCommand]
        private async Task ValidateUsernameAsync()
        {
            await ValidateUsernameInternalAsync();
        }

        // Méthode interne avec retour booléen pour validation
        private async Task<bool> ValidateUsernameInternalAsync()
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
            var isValidUsername = ValidationExtensions.IsValidUsername(RegistrationModel.Username);
            if (!isValidUsername)
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

        // Méthode pour le binding XAML
        [RelayCommand]
        private async Task ValidatePasswordAsync()
        {
            await ValidatePasswordInternalAsync();
        }

        // Méthode interne avec retour booléen pour validation
        private async Task<bool> ValidatePasswordInternalAsync()
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
            var passwordResult = ValidationExtensions.GetPasswordStrength(RegistrationModel.Password);
            PasswordStrength = passwordResult.Strength;

            if (!passwordResult.IsValid)
            {
                var validation = ValidationExtensions.ValidatePassword(RegistrationModel.Password);
                PasswordValidationMessage = validation.Message;
                return false;
            }

            return true;
        }

        // Méthode pour le binding XAML
        [RelayCommand]
        private async Task ValidatePasswordsMatchAsync()
        {
            await ValidatePasswordsMatchInternalAsync();
        }

        // Méthode interne avec retour booléen pour validation
        private async Task<bool> ValidatePasswordsMatchInternalAsync()
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
}