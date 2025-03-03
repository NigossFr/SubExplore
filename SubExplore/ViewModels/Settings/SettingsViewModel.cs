using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;

namespace SubExplore.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private bool _isDarkTheme;

    public SettingsViewModel(
        INavigationService navigationService,
        ISettingsService settingsService)
        : base(navigationService)
    {
        _settingsService = settingsService;
        Title = "Paramètres";
        LoadSettings();
    }

    private async void LoadSettings()
    {
        IsDarkTheme = await _settingsService.GetThemePreferenceAsync();
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        _settingsService.SetThemePreferenceAsync(value);
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await NavigationService.GoBackAsync();
    }
}
