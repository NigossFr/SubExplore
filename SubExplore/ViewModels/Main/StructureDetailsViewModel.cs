using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;
using System.Threading.Tasks;

namespace SubExplore.ViewModels.Main;

public partial class StructureDetailsViewModel : ViewModelBase
{
    public StructureDetailsViewModel(INavigationService navigationService)
        : base(navigationService)
    {
        Title = "Détails de la structure";
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await NavigationService.GoBackAsync();
    }
}
