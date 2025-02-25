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

namespace SubExplore.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private RegistrationModel _registrationModel;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage;

    public RegisterViewModel(
        IAuthenticationService authenticationService,
        INavigationService navigationService)
        : base(navigationService)
    {
        _authenticationService = authenticationService;
        _registrationModel = new RegistrationModel();
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        try
        {
            if (!ValidateRegistrationData())
                return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            // Vérifier si l'email existe déjà
            var emailExists = await _authenticationService.CheckEmailExistsAsync(RegistrationModel.Email);
            if (emailExists)
            {
                ErrorMessage = "Cette adresse email est déjà utilisée";
                return;
            }

            // Vérifier si le pseudo existe déjà
            var usernameExists = await _authenticationService.CheckUsernameExistsAsync(RegistrationModel.Username);
            if (usernameExists)
            {
                ErrorMessage = "Ce pseudo est déjà utilisé";
                return;
            }

            // Tenter l'inscription
            var result = await _authenticationService.RegisterAsync(
                RegistrationModel.Email,
                RegistrationModel.Password,
                RegistrationModel.Username,
                RegistrationModel.FirstName,
                RegistrationModel.LastName
            );

            if (result)
            {
                // Rediriger vers la page de confirmation
                await NavigationService.NavigateToAsync("registration-confirmation",
                    new Dictionary<string, object> { { "email", RegistrationModel.Email } });
            }
            else
            {
                ErrorMessage = "L'inscription a échoué. Veuillez réessayer.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur est survenue lors de l'inscription";
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Registration error: {ex}");
#endif
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool ValidateRegistrationData()
    {
        if (string.IsNullOrWhiteSpace(RegistrationModel.Email) ||
            string.IsNullOrWhiteSpace(RegistrationModel.Password) ||
            string.IsNullOrWhiteSpace(RegistrationModel.Username) ||
            string.IsNullOrWhiteSpace(RegistrationModel.FirstName) ||
            string.IsNullOrWhiteSpace(RegistrationModel.LastName))
        {
            ErrorMessage = "Veuillez remplir tous les champs";
            return false;
        }

        if (RegistrationModel.Password != RegistrationModel.ConfirmPassword)
        {
            ErrorMessage = "Les mots de passe ne correspondent pas";
            return false;
        }

        if (!RegistrationModel.AcceptTerms)
        {
            ErrorMessage = "Vous devez accepter les conditions d'utilisation";
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
        await NavigationService.NavigateToAsync("terms");
    }
}
