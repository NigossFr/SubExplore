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
using SubExplore.Models.DTOs;
using SubExplore.Models.Enums;
using System.Diagnostics;
using MediaFile = SubExplore.Models.Media.IMediaFile;
using GeoCoordinates = SubExplore.Models.GeoCoordinates;

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
            // Conversion en SpotCreationDto
            var spotCreationDto = new SpotCreationDto
            {
                Name = _spot.Name,
                Description = _spot.Description,
                Latitude = _spot.Latitude,
                Longitude = _spot.Longitude,
                Type = _spot.Type,
                DifficultyLevel = _spot.DifficultyLevel,
                RequiredEquipment = _spot.RequiredEquipment ?? "",
                SafetyNotes = _spot.SafetyNotes ?? "",
                BestConditions = _spot.BestConditions ?? "",
                MaxDepth = _spot.MaxDepth,
                CurrentStrength = _spot.CurrentStrength,
                HasMooring = _spot.HasMooring,
                BottomType = _spot.BottomType
            };

            // Upload des photos si présentes
            var mediaUploads = new List<MediaUploadResult>();
            foreach (var photo in _photos)
            {
                var uploadResult = await _mediaService.UploadAsync(
                    photo,
                    MediaServiceType.Spot,
                    0 // Remplacer par l'ID utilisateur réel
                );
                mediaUploads.Add(uploadResult);
            }

            // Création du spot
            var createdSpot = await _spotService.CreateAsync(spotCreationDto, 0);

            HasUnsavedChanges = false;
            await NavigationService.NavigateToAsync("spot-details",
                new Dictionary<string, object> { { "spotId", createdSpot.Id } });
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
                // Conversion du résultat MediaPicker en IMediaFile
                var mediaFile = new SubExplore.Models.DTOs.FormFileAdapter(
                    await photo.OpenReadAsync(),
                    photo.FileName,
                    photo.ContentType,
                    new System.IO.FileInfo(photo.FullPath).Length,
                    photo.FullPath,
                    _photos.Count == 0 // La première photo est considérée comme principale
                );

                _photos.Add(mediaFile);
                HasUnsavedChanges = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible d'ajouter la photo", "OK");
            Debug.WriteLine($"Photo error: {ex}");
        }
    }

    [RelayCommand]
    private void RemovePhoto(MediaFile photo)
    {
        _photos.Remove(photo);
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