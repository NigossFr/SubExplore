using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Models;
using SubExplore.Services.Navigation;
using SubExplore.Services.Spots;
using SubExplore.Services.Location;
using SubExplore.ViewModels.Base;

namespace SubExplore.ViewModels.Main;

public partial class MapViewModel : ViewModelBase
{
    private readonly ISpotService _spotService;
    private readonly ILocationService _locationService;
    private double _currentLatitude;
    private double _currentLongitude;
    private float _zoomLevel = 12f;

    [ObservableProperty]
    private ObservableCollection<SpotMarker> _spots;

    [ObservableProperty]
    private bool _isLocationAvailable;

    [ObservableProperty]
    private SpotFilter _activeFilter;

    [ObservableProperty]
    private bool _isFilterPanelVisible;

    public MapViewModel(
        INavigationService navigationService,
        ISpotService spotService,
        ILocationService locationService)
        : base(navigationService)
    {
        _spotService = spotService;
        _locationService = locationService;
        Spots = new ObservableCollection<SpotMarker>();
        ActiveFilter = new SpotFilter();

        Title = "Carte";
    }

    public override async Task InitializeAsync(IDictionary<string, object> parameters)
    {
        await base.InitializeAsync(parameters);
        await LoadCurrentLocationAsync();
        await LoadSpotsAsync();
    }

    private async Task LoadCurrentLocationAsync()
    {
        try
        {
            var location = await _locationService.GetCurrentLocationAsync();
            if (location != null)
            {
                _currentLatitude = location.Latitude;
                _currentLongitude = location.Longitude;
                IsLocationAvailable = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible d'obtenir votre position. Vérifiez les autorisations de localisation.", "OK");
            IsLocationAvailable = false;
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Location error: {ex}");
#endif
        }
    }

    private async Task LoadSpotsAsync()
    {
        await SafeExecuteAsync(async () =>
        {
            var spots = await _spotService.GetSpotsAsync(ActiveFilter);

            Spots.Clear();
            foreach (var spot in spots)
            {
                Spots.Add(new SpotMarker
                {
                    Id = spot.Id,
                    Latitude = spot.Latitude,
                    Longitude = spot.Longitude,
                    Title = spot.Name,
                    Type = spot.Type,
                    DifficultyLevel = spot.DifficultyLevel,
                    IconPath = GetMarkerIcon(spot.Type)
                });
            }

            IsEmpty = !Spots.Any();
        }, "Erreur lors du chargement des spots");
    }

    private string GetMarkerIcon(SpotType type) => type switch
    {
        SpotType.Diving => "marker_diving.png",
        SpotType.Snorkeling => "marker_snorkeling.png",
        SpotType.Freediving => "marker_freediving.png",
        _ => "marker_default.png"
    };

    [RelayCommand]
    private async Task RefreshSpotsAsync()
    {
        IsRefreshing = true;
        await LoadSpotsAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task SpotSelectedAsync(SpotMarker spot)
    {
        if (spot == null) return;

        var parameters = new Dictionary<string, object>
        {
            { "spotId", spot.Id }
        };

        await NavigationService.NavigateToAsync("spot-details", parameters);
    }

    [RelayCommand]
    private async Task AddSpotAsync()
    {
        await NavigationService.NavigateToAsync("add-spot", new Dictionary<string, object>
        {
            { "latitude", _currentLatitude },
            { "longitude", _currentLongitude }
        });
    }

    [RelayCommand]
    private Task ToggleFilterPanel()
    {
        IsFilterPanelVisible = !IsFilterPanelVisible;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ApplyFilterAsync()
    {
        IsFilterPanelVisible = false;
        await LoadSpotsAsync();
    }

    [RelayCommand]
    private async Task ResetFilterAsync()
    {
        ActiveFilter = new SpotFilter();
        await LoadSpotsAsync();
    }

    [RelayCommand]
    private async Task CenterOnLocationAsync()
    {
        if (!IsLocationAvailable)
        {
            await LoadCurrentLocationAsync();
        }

        if (IsLocationAvailable)
        {
            // La vue devra réagir à ces changements
            _currentLatitude = await _locationService.GetLatitudeAsync();
            _currentLongitude = await _locationService.GetLongitudeAsync();
            _zoomLevel = 14f;
        }
    }

    public class SpotMarker
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Title { get; set; }
        public SpotType Type { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string IconPath { get; set; }
    }
}
