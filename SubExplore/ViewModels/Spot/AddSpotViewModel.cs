using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;
using SubExplore.Models;
using System.Diagnostics;

namespace SubExplore.ViewModels.Spot;

public partial class AddSpotViewModel : ViewModelBase
{
    private const int MAX_PHOTOS = 3;

    private readonly ISpotService _spotService;
    private readonly ILocationService _locationService;
    private readonly IMediaService _mediaService;

    [ObservableProperty]
    private SpotCreationModel _spot;

    [ObservableProperty]
    private bool _isLocationValid;

    [ObservableProperty]
    private ObservableCollection<MediaFile> _photos;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    public AddSpotViewModel(
        INavigationService navigationService,
        ISpotService spotService,
        ILocationService locationService,
        IMediaService mediaService)
        : base(navigationService)
    {
        _spotService = spotService;
        _locationService = locationService;
        _mediaService = mediaService;
        _spot = new SpotCreationModel();
        _photos = new ObservableCollection<MediaFile>();
    }

    public override async Task InitializeAsync(IDictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("latitude", out var lat) &&
            parameters.TryGetValue("longitude", out var lon))
        {
            _spot.Latitude = Convert.ToDouble(lat);
            _spot.Longitude = Convert.ToDouble(lon);
            await ValidateLocation();
        }
    }

    private async Task ValidateLocation()
    {
        var validation = await _locationService.ValidateMaritimeLocationAsync(
            new GeoCoordinates
            {
                Latitude = _spot.Latitude,
                Longitude = _spot.Longitude
            });
        IsLocationValid = validation.IsValid;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (!await ValidateForm()) return;

        await SafeExecuteAsync(async () =>
        {
            var spotDto = await _spotService.CreateAsync(_spot.ToDto(), 0);
            HasUnsavedChanges = false;
            await NavigationService.NavigateToAsync("spot-details",
                new Dictionary<string, object> { { "spotId", spotDto.Id } });
        });
    }

    [RelayCommand]
    private async Task AddPhotosAsync()
    {
        if (_photos.Count >= MAX_PHOTOS)
        {
            await DisplayAlert("Erreur", $"Maximum {MAX_PHOTOS} photos autorisées", "OK");
            return;
        }

        try
        {
            var photo = await MediaPicker.PickPhotoAsync();
            if (photo != null)
            {
                await ProcessPhotoAsync(photo);
                HasUnsavedChanges = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible d'ajouter la photo", "OK");
            Debug.WriteLine($"Photo error: {ex}");
        }
    }

    private async Task ProcessPhotoAsync(FileResult photo)
    {
        var mediaFile = await _mediaService.ProcessPhotoAsync(photo);
        if (mediaFile != null)
        {
            _photos.Add(mediaFile);
            _spot.Photos.Add(mediaFile);
        }
    }

    [RelayCommand]
    private void RemovePhoto(MediaFile photo)
    {
        _photos.Remove(photo);
        _spot.Photos.Remove(photo);
        HasUnsavedChanges = true;
    }

    private async Task<bool> ValidateForm()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_spot.Name))
            errors.Add("Nom requis");

        if (!IsLocationValid)
            errors.Add("Position invalide");

        if (_spot.Type == null)
            errors.Add("Type d'activité requis");

        if (_spot.DifficultyLevel == null)
            errors.Add("Niveau de difficulté requis");

        if (errors.Any())
        {
            await DisplayAlert("Validation", string.Join("\n", errors), "OK");
            return false;
        }

        return true;
    }
}