using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Authentication;
using SubExplore.Services.Navigation;
using SubExplore.Services.Settings;

namespace SubExplore.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly IAuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private bool _isUserLoggedIn;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private bool _isDarkTheme;

    public AppShellViewModel(
        IAuthenticationService authenticationService,
        INavigationService navigationService,
        ISettingsService settingsService)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;
        _settingsService = settingsService;

        // Initialiser l'état de l'application
        InitializeAppState();
    }

    private async void InitializeAppState()
    {
        // Vérifier le thème actuel
        IsDarkTheme = await _settingsService.GetThemePreferenceAsync();

        // Vérifier l'état de connexion
        var authState = await _authenticationService.GetCurrentUserAsync();
        IsUserLoggedIn = authState != null;
        if (IsUserLoggedIn)
        {
            Username = authState.Username;
        }

        // S'abonner aux changements d'authentification
        _authenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    private void OnAuthenticationStateChanged(object sender, AuthenticationEventArgs e)
    {
        IsUserLoggedIn = e.IsAuthenticated;
        Username = e.IsAuthenticated ? e.Username : string.Empty;
    }

    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        if (IsUserLoggedIn)
        {
            await _navigationService.NavigateToAsync("///profile");
        }
        else
        {
            await _navigationService.NavigateToAsync("///login");
        }
    }

    [RelayCommand]
    private async Task NavigateToMapAsync()
    {
        await _navigationService.NavigateToAsync("///map");
    }

    [RelayCommand]
    private async Task NavigateToMagazineAsync()
    {
        await _navigationService.NavigateToAsync("///magazine");
    }

    [RelayCommand]
    private async Task NavigateToStructuresAsync()
    {
        await _navigationService.NavigateToAsync("///structures");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Déconnexion",
            "Êtes-vous sûr de vouloir vous déconnecter ?",
            "Oui",
            "Non");

        if (confirmed)
        {
            await _authenticationService.LogoutAsync();
            await _navigationService.NavigateToAsync("///login");
        }
    }

    [RelayCommand]
    private async Task ToggleThemeAsync()
    {
        IsDarkTheme = !IsDarkTheme;
        await _settingsService.SetThemePreferenceAsync(IsDarkTheme);
        // Notifier le changement de thème à l'application
        MessagingCenter.Send(this, "ThemeChanged", IsDarkTheme);
    }

    public void Dispose()
    {
        _authenticationService.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}
