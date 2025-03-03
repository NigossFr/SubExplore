using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.Maui.Devices.Sensors;
using System.Diagnostics;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;
using SubExplore.Models;
using SubExplore.Models.DTOs;
using Microsoft.Maui.Maps;

namespace SubExplore.ViewModels.Main;

public partial class MapViewModel : ViewModelBase
{
    private const float DEFAULT_ZOOM = 12f;
    private const float DETAIL_ZOOM = 14f;
    private const double SEARCH_RADIUS_KM = 5.0;

    private readonly ISpotService _spotService;
    private readonly ILocationService _locationService;
    private readonly IConnectivityService _connectivityService;

    private Location? _currentLocation;
    private float _zoomLevel = DEFAULT_ZOOM;
    private CancellationTokenSource? _loadingCancellation;

    [ObservableProperty]
    private ObservableCollection<SpotMarker> _spots;

    [ObservableProperty]
    private bool _isLocationAvailable;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanApplyFilter))]
    private Models.DTOs.SpotFilter _activeFilter;

    [ObservableProperty]
    private bool _isFilterPanelVisible;

    [ObservableProperty]
    private bool _isOfflineMode;

    public bool CanApplyFilter => ActiveFilter != null && !IsBusy;

    public MapViewModel(
        INavigationService navigationService,
        ISpotService spotService,
        ILocationService locationService,
        IConnectivityService connectivityService)
        : base(navigationService)
    {
        _spotService = spotService ?? throw new ArgumentNullException(nameof(spotService));
        _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        _connectivityService = connectivityService ?? throw new ArgumentNullException(nameof(connectivityService));

        Title = "Carte";
        _spots = new ObservableCollection<SpotMarker>();
        _activeFilter = new Models.DTOs.SpotFilter();

        InitializeConnectivityMonitoring();
    }

    private void InitializeConnectivityMonitoring()
    {
        _connectivityService.ConnectivityChanged += OnConnectivityChanged;
        IsOfflineMode = !_connectivityService.IsConnected;
    }

    private async void OnConnectivityChanged(object? sender, Services.Interfaces.ConnectivityChangedEventArgs e)
    {
        IsOfflineMode = !e.IsConnected;
        if (e.IsConnected && Spots?.Count == 0)
        {
            await LoadSpotsAsync();
        }
    }

    public override async Task InitializeAsync(IDictionary<string, object> parameters)
    {
        _loadingCancellation?.Cancel();
        _loadingCancellation = new CancellationTokenSource();

        try
        {
            await Task.WhenAll(
                LoadCurrentLocationAsync(),
                LoadSpotsAsync()
            ).WaitAsync(_loadingCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            // Opération annulée, rien à faire
        }
    }

    private async Task LoadCurrentLocationAsync()
    {
        try
        {
            _currentLocation = await Geolocation.Default.GetLocationAsync();
            IsLocationAvailable = _currentLocation != null;
        }
        catch (Exception ex)
        {
            IsLocationAvailable = false;
            await HandleLocationError(ex);
        }
    }

    private async Task HandleLocationError(Exception ex)
    {
        var message = ex switch
        {
            PermissionException => "Permission de localisation refusée",
            FeatureNotSupportedException => "Service de localisation désactivé",
            _ => "Erreur de localisation"
        };

        await DisplayAlert("Erreur", message, "OK");
        Debug.WriteLine($"Location error: {ex}");
    }

    [RelayCommand]
    private async Task LoadSpotsAsync()
    {
        if (IsBusy) return;

        await SafeExecuteAsync(async () =>
        {
            IEnumerable<SpotDto> spots;

            if (IsOfflineMode)
            {
                spots = await _spotService.GetCachedSpotsAsync(ActiveFilter);
            }
            else if (_currentLocation != null && ActiveFilter.WithinCurrentArea)
            {
                // Si on a une position et que le filtre indique de chercher autour de la position actuelle
                spots = await _spotService.GetNearbyAsync(
                    _currentLocation.Latitude,
                    _currentLocation.Longitude,
                    ActiveFilter.RadiusInKm ?? SEARCH_RADIUS_KM);
            }
            else
            {
                // Utiliser les paramètres de recherche avancés
                var searchParams = new Models.DTOs.SpotSearchParameters
                {
                    SearchTerm = ActiveFilter.SearchTerm,
                    ActivityTypes = ActiveFilter.ActivityTypes,
                    MinDepth = ActiveFilter.MinDepth,
                    MaxDepth = ActiveFilter.MaxDepth,
                    DifficultyLevel = ActiveFilter.DifficultyLevel,
                    Latitude = ActiveFilter.Latitude,
                    Longitude = ActiveFilter.Longitude,
                    RadiusInKm = ActiveFilter.RadiusInKm,
                    ValidatedOnly = ActiveFilter.ValidatedOnly
                };

                spots = await _spotService.SearchAsync(searchParams);
            }

            UpdateSpotCollection(spots);
        });
    }

    private void UpdateSpotCollection(IEnumerable<SpotDto> spots)
    {
        Spots.Clear();
        foreach (var spot in spots)
        {
            Spots.Add(CreateSpotMarker(spot));
        }
        IsEmpty = !Spots.Any();
    }

    private static SpotMarker CreateSpotMarker(SpotDto spot) => new()
    {
        Id = spot.Id,
        Latitude = spot.Latitude,
        Longitude = spot.Longitude,
        Title = spot.Name,
        Type = spot.Type,
        DifficultyLevel = spot.DifficultyLevel,
        IconPath = GetMarkerIcon(spot.Type)
    };

    private static string GetMarkerIcon(SpotType type)
    {
        return type.Category switch
        {
            Models.Enums.ActivityCategory.Diving => "marker_diving.png",
            Models.Enums.ActivityCategory.Freediving => "marker_freediving.png",
            Models.Enums.ActivityCategory.Snorkeling => "marker_snorkeling.png",
            _ => "marker_default.png"
        };
    }

    [RelayCommand]
    private Task SpotSelectedAsync(SpotMarker spot)
    {
        if (spot == null) return Task.CompletedTask;

        return NavigationService.NavigateToAsync("spot-details", new Dictionary<string, object>
       {
           { "spotId", spot.Id }
       });
    }

    [RelayCommand]
    private Task AddSpotAsync()
    {
        if (!IsLocationAvailable || _currentLocation == null) return Task.CompletedTask;

        return NavigationService.NavigateToAsync("add-spot", new Dictionary<string, object>
       {
           { "latitude", _currentLocation.Latitude },
           { "longitude", _currentLocation.Longitude }
       });
    }

    [RelayCommand]
    private Task ToggleFilterPanel()
    {
        IsFilterPanelVisible = !IsFilterPanelVisible;
        return Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanApplyFilter))]
    private async Task ApplyFilterAsync()
    {
        IsFilterPanelVisible = false;
        await LoadSpotsAsync();
    }

    [RelayCommand]
    private async Task ResetFilterAsync()
    {
        ActiveFilter = new Models.DTOs.SpotFilter();
        await LoadSpotsAsync();
    }

    [RelayCommand]
    private async Task CenterOnLocationAsync()
    {
        if (!IsLocationAvailable)
        {
            await LoadCurrentLocationAsync();
        }

        if (_currentLocation != null)
        {
            // Cette méthode serait normalement dans ILocationService
            await MoveToLocationAsync(_currentLocation, DETAIL_ZOOM);
        }
    }

    // Méthode temporaire pour la gestion de la carte
    private Task MoveToLocationAsync(Location location, float zoom)
    {
        // Cette méthode serait implémentée avec la logique de la carte
        Debug.WriteLine($"Moving to location: {location.Latitude}, {location.Longitude} with zoom {zoom}");
        return Task.CompletedTask;
    }

    public async Task CenterOnUser()
    {
        await LoadCurrentLocationAsync();
        await CenterOnLocationAsync();
    }

    public void HandleMapClick(Location location)
    {
        // Méthode appelée quand l'utilisateur clique sur la carte
        Debug.WriteLine($"Map clicked at: {location.Latitude}, {location.Longitude}");
    }

    public void PerformSearch()
    {
        // Méthode appelée quand l'utilisateur effectue une recherche
        Task.Run(LoadSpotsAsync);
    }

    public void OnAppearing()
    {
        // Méthode appelée quand la page devient visible
    }

    public void OnDisappearing()
    {
        // Méthode appelée quand la page n'est plus visible
    }

    public override void Dispose()
    {
        _loadingCancellation?.Cancel();
        _loadingCancellation?.Dispose();
        _connectivityService.ConnectivityChanged -= OnConnectivityChanged;
        base.Dispose();
    }
}