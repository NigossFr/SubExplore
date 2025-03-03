using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;

namespace SubExplore.ViewModels.Main;

public partial class SpotDetailsViewModel : ViewModelBase
{
    private readonly ISpotService _spotService;
    private int _spotId;

    [ObservableProperty]
    private string _spotName;

    [ObservableProperty]
    private string _spotDescription;

    public SpotDetailsViewModel(
        INavigationService navigationService,
        ISpotService spotService)
        : base(navigationService)
    {
        _spotService = spotService;
    }

    public override async Task InitializeAsync(IDictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("spotId", out var spotIdObj) && spotIdObj is int spotId)
        {
            _spotId = spotId;
            await LoadSpotDetailsAsync();
        }
    }

    private async Task LoadSpotDetailsAsync()
    {
        await SafeExecuteAsync(async () =>
        {
            var spot = await _spotService.GetByIdAsync(_spotId);
            if (spot != null)
            {
                SpotName = spot.Name;
                SpotDescription = spot.Description;
                Title = spot.Name;
            }
            else
            {
                SpotName = "Spot introuvable";
                SpotDescription = "Le spot que vous cherchez n'existe pas.";
                Title = "Spot introuvable";
            }
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await NavigationService.GoBackAsync();
    }
}
