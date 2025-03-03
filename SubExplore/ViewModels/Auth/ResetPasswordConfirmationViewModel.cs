using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;

namespace SubExplore.ViewModels.Auth;

public partial class ResetPasswordConfirmationViewModel : ViewModelBase
{
    public ResetPasswordConfirmationViewModel(INavigationService navigationService)
        : base(navigationService)
    {
        Title = "Réinitialisation du mot de passe";
    }

    [RelayCommand]
    private async Task NavigateToLoginAsync()
    {
        await NavigationService.NavigateToAsync("login");
    }
}
