using SubExplore.ViewModels;
using Microsoft.Maui.Maps;
using Microsoft.Maui.ApplicationModel;
using SubExplore.ViewModels.Main;

namespace SubExplore.Views.Main;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    private IDispatcherTimer _debounceTimer;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Initialiser le timer pour le debounce de la recherche
        _debounceTimer = Application.Current?.Dispatcher?.CreateTimer();
        if (_debounceTimer != null)
        {
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(300);
            _debounceTimer.Tick += OnSearchDebounceTimeout;
        }

        // S'abonner aux �v�nements de la carte
        map.MapClicked += OnMapClicked;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();

        // Demander la permission de g�olocalisation si n�cessaire
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                await _viewModel.CenterOnUser();
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing();
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        // Fermer le panneau des filtres si ouvert
        if (_viewModel.IsFiltersPanelVisible)
        {
            _viewModel.IsFiltersPanelVisible = false;
        }

        // Transmettre l'�v�nement au ViewModel
        _viewModel.HandleMapClick(e.Location);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_debounceTimer == null) return;

        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    private void OnSearchDebounceTimeout(object sender, EventArgs e)
    {
        _debounceTimer?.Stop();
        _viewModel.PerformSearch();
    }

    // Gestion du comportement du panneau des filtres
    private double previousPanelTranslation;

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                previousPanelTranslation = filtersPanel.TranslationY;
                break;

            case GestureStatus.Running:
                var translation = previousPanelTranslation + e.TotalY;
                if (translation >= 0) // Emp�cher de tirer vers le bas
                {
                    filtersPanel.TranslationY = translation;
                }
                break;

            case GestureStatus.Completed:
                // Si le panneau a �t� tir� de plus de 100 unit�s, le fermer
                if (filtersPanel.TranslationY > 100)
                {
                    _viewModel.IsFiltersPanelVisible = false;
                }
                else
                {
                    // Sinon, le remettre en position initiale
                    filtersPanel.TranslationY = 0;
                }
                break;
        }
    }
}