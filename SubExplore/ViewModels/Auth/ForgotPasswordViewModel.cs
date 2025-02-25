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

public partial class ForgotPasswordViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private ForgotPasswordModel _forgotPasswordModel;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private string _successMessage;

    [ObservableProperty]
    private bool _isResetEmailSent;

    public ForgotPasswordViewModel(
        IAuthenticationService authenticationService,
        INavigationService navigationService)
        : base(navigationService)
    {
        _authenticationService = authenticationService;
        _forgotPasswordModel = new ForgotPasswordModel();
    }

    [RelayCommand]
    private async Task RequestPasswordResetAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ForgotPasswordModel.Email))
            {
                ErrorMessage = "Veuillez saisir votre adresse email";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            // Vérifier si l'email existe
            var emailExists = await _authenticationService.CheckEmailExistsAsync(ForgotPasswordModel.Email);
            if (!emailExists)
            {
                ErrorMessage = "Aucun compte n'est associé à cette adresse email";
                return;
            }

            // Générer et envoyer le token de réinitialisation
            var resetToken = await _authenticationService.GeneratePasswordResetTokenAsync(ForgotPasswordModel.Email);
            if (!string.IsNullOrEmpty(resetToken))
            {
                IsResetEmailSent = true;
                SuccessMessage = "Un email de réinitialisation a été envoyé à votre adresse";

                // Rediriger vers la page de confirmation après un délai
                await Task.Delay(2000);
                await NavigationService.NavigateToAsync("reset-password-confirmation");
            }
            else
            {
                ErrorMessage = "Impossible d'envoyer l'email de réinitialisation";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur est survenue lors de la demande de réinitialisation";
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Password reset request error: {ex}");
#endif
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ResetPasswordAsync(ResetPasswordModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.NewPassword) ||
                string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ErrorMessage = "Veuillez remplir tous les champs";
                return;
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ErrorMessage = "Les mots de passe ne correspondent pas";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            var success = await _authenticationService.ResetPasswordAsync(model.Token, model.NewPassword);
            if (success)
            {
                SuccessMessage = "Votre mot de passe a été réinitialisé avec succès";
                await Task.Delay(1500);
                await NavigationService.NavigateToAsync("login");
            }
            else
            {
                ErrorMessage = "La réinitialisation du mot de passe a échoué";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Une erreur est survenue lors de la réinitialisation du mot de passe";
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Password reset error: {ex}");
#endif
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToLoginAsync()
    {
        await NavigationService.NavigateToAsync("login");
    }
}
