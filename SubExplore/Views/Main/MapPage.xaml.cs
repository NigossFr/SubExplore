using Microsoft.Maui.Maps;
using Microsoft.Maui.ApplicationModel;
using SubExplore.ViewModels.Main;
using Microsoft.Maui.Controls.Maps;

namespace SubExplore.Views.Main;

public partial class MapPage : ContentPage, IDisposable
{
    private readonly MapViewModel _viewModel;
    private readonly IDispatcherTimer _debounceTimer;
    private const int DEBOUNCE_INTERVAL_MS = 300;
    private const double PANEL_CLOSE_THRESHOLD = 100.0;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        InitializeDebounceTimer();
        InitializeMapEvents();
    }

    private void InitializeDebounceTimer()
    {
        _debounceTimer = Application.Current?.Dispatcher?.CreateTimer();
        if (_debounceTimer != null)
        {
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(DEBOUNCE_INTERVAL_MS);
            _debounceTimer.Tick += OnSearchDebounceTimeout;
        }
    }

    private void InitializeMapEvents() => map.MapClicked += OnMapClicked;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();
        await CheckAndRequestLocationPermission();
    }

    private async Task CheckAndRequestLocationPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                await _viewModel.CenterOnUser();
            }
        }
        else
        {
            await _viewModel.CenterOnUser();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing();
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        if (_viewModel.IsFiltersPanelVisible)
        {
            _viewModel.IsFiltersPanelVisible = false;
        }
        _viewModel.HandleMapClick(e.Location);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _debounceTimer?.Stop();
        _debounceTimer?.Start();
    }

    private void OnSearchDebounceTimeout(object sender, EventArgs e)
    {
        _debounceTimer?.Stop();
        _viewModel.PerformSearch();
    }

    private double _previousPanelTranslation;

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _previousPanelTranslation = filtersPanel.TranslationY;
                break;
            case GestureStatus.Running:
                var newTranslation = Math.Max(0, _previousPanelTranslation + e.TotalY);
                filtersPanel.TranslationY = newTranslation;
                break;
            case GestureStatus.Completed:
                HandlePanelSlideCompletion();
                break;
        }
    }

    private void HandlePanelSlideCompletion()
    {
        if (filtersPanel.TranslationY > PANEL_CLOSE_THRESHOLD)
        {
            _viewModel.IsFiltersPanelVisible = false;
        }
        else
        {
            filtersPanel.TranslationY = 0;
        }
    }

    public void Dispose()
    {
        _debounceTimer?.Stop();
        map.MapClicked -= OnMapClicked;
        GC.SuppressFinalize(this);
    }
}