using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;

namespace SubExplore.ViewModels.Main;

public partial class StoryDetailsViewModel : ViewModelBase
{
    public StoryDetailsViewModel(INavigationService navigationService)
        : base(navigationService)
    {
        Title = "Détails de l'article";
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await NavigationService.GoBackAsync();
    }
}
