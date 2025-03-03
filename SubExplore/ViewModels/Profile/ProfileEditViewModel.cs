using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;

namespace SubExplore.ViewModels.Profile;

public partial class ProfileEditViewModel : ViewModelBase
{
    public ProfileEditViewModel(INavigationService navigationService)
        : base(navigationService)
    {
        Title = "Modifier le profil";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        // TODO: Implémenter la sauvegarde du profil
        await DisplayAlert("Info", "Fonctionnalité en cours de développement", "OK");
        await NavigationService.GoBackAsync();
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await NavigationService.GoBackAsync();
    }
}
