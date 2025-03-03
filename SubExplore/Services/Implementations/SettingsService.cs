using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SubExplore.Constants;
using SubExplore.Services.Interfaces;
using InterfaceAppTheme = SubExplore.Services.Interfaces.AppTheme;
using MauiAppTheme = Microsoft.Maui.ApplicationModel.AppTheme;

namespace SubExplore.Services.Implementations
{
    public class SettingsService : ISettingsService
    {
        private readonly ISecureStorageService _secureStorage;
        private const string THEME_KEY = "app_theme";
        private const string FOLLOW_SYSTEM_KEY = "follow_system_theme";
        private const string NOTIFICATION_PREFERENCES_KEY = "notification_preferences";
        private const string PRIVACY_SETTINGS_KEY = "privacy_settings";
        private const string MAP_PREFERENCES_KEY = "map_preferences";
        private const string LAST_LOCATION_KEY = "last_location";

        public SettingsService(ISecureStorageService secureStorage)
        {
            _secureStorage = secureStorage;
        }

        #region Paramètres Généraux

        public async Task<T> GetSettingAsync<T>(string key, T defaultValue = default)
        {
            var value = await _secureStorage.GetAsync(key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public async Task SetSettingAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            await _secureStorage.SetAsync(key, json);
        }

        public async Task RemoveSettingAsync(string key)
        {
            await _secureStorage.RemoveAsync(key);
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await _secureStorage.ContainsKeyAsync(key);
        }

        #endregion

        #region Thème et Apparence

        public async Task<InterfaceAppTheme> GetCurrentThemeAsync()
        {
            var themeName = await _secureStorage.GetAsync(THEME_KEY);
            if (string.IsNullOrEmpty(themeName))
                return InterfaceAppTheme.Light; // Thème par défaut

            if (Enum.TryParse<InterfaceAppTheme>(themeName, out var theme))
                return theme;

            return InterfaceAppTheme.Light;
        }

        public async Task SetThemeAsync(InterfaceAppTheme theme)
        {
            await _secureStorage.SetAsync(THEME_KEY, theme.ToString());
        }

        public async Task<bool> GetFollowSystemThemeAsync()
        {
            var value = await _secureStorage.GetAsync(FOLLOW_SYSTEM_KEY);
            return !string.IsNullOrEmpty(value) && bool.Parse(value);
        }

        public async Task SetFollowSystemThemeAsync(bool follow)
        {
            await _secureStorage.SetAsync(FOLLOW_SYSTEM_KEY, follow.ToString());
        }

        // Ces méthodes sont celles qui manquaient et causaient les erreurs
        public async Task<bool> GetThemePreferenceAsync()
        {
            var theme = await GetCurrentThemeAsync();
            return theme == InterfaceAppTheme.Dark;
        }

        public async Task SetThemePreferenceAsync(bool isDarkTheme)
        {
            var theme = isDarkTheme ? InterfaceAppTheme.Dark : InterfaceAppTheme.Light;
            await SetThemeAsync(theme);
        }

        #endregion

        #region Notifications

        public async Task<NotificationPreferences> GetNotificationPreferencesAsync()
        {
            return await GetSettingAsync<NotificationPreferences>(NOTIFICATION_PREFERENCES_KEY, new NotificationPreferences
            {
                EnablePush = true,
                EnableEmail = true,
                TypePreferences = new Dictionary<NotificationType, bool>(),
                QuietHoursStart = TimeSpan.FromHours(AppSettings.QuietHoursStart),
                QuietHoursEnd = TimeSpan.FromHours(AppSettings.QuietHoursEnd),
                EnableQuietHours = false
            });
        }

        public async Task UpdateNotificationPreferencesAsync(NotificationPreferences preferences)
        {
            await SetSettingAsync(NOTIFICATION_PREFERENCES_KEY, preferences);
        }

        public async Task<bool> IsNotificationEnabledAsync(NotificationType notificationType)
        {
            var preferences = await GetNotificationPreferencesAsync();

            if (!preferences.EnablePush)
                return false;

            if (preferences.TypePreferences.TryGetValue(notificationType, out bool isEnabled))
                return isEnabled;

            return true; // Par défaut, toutes les notifications sont activées
        }

        #endregion

        #region Confidentialité et Sécurité

        public async Task<PrivacySettings> GetPrivacySettingsAsync()
        {
            return await GetSettingAsync<PrivacySettings>(PRIVACY_SETTINGS_KEY, new PrivacySettings
            {
                ShareLocation = true,
                AllowProfileVisibility = true,
                EnableAnalytics = true,
                ShareDiveHistory = true,
                CustomPrivacyFlags = new Dictionary<string, bool>()
            });
        }

        public async Task UpdatePrivacySettingsAsync(PrivacySettings settings)
        {
            await SetSettingAsync(PRIVACY_SETTINGS_KEY, settings);
        }

        public async Task SetSecureStorageAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            await _secureStorage.SetAsync(key, json);
        }

        public async Task<T> GetSecureStorageAsync<T>(string key)
        {
            var value = await _secureStorage.GetAsync(key);
            if (string.IsNullOrEmpty(value))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return default;
            }
        }

        #endregion

        #region Préférences de Navigation

        public async Task<MapPreferences> GetMapPreferencesAsync()
        {
            return await GetSettingAsync<MapPreferences>(MAP_PREFERENCES_KEY, new MapPreferences
            {
                ShowClusteredMarkers = true,
                AutoRotateMap = false,
                Show3DBuildings = false,
                DefaultMapType = "standard",
                DefaultZoomLevel = AppSettings.DefaultZoomLevel,
                EnableOfflineMode = false,
                LayerVisibility = new Dictionary<string, bool>()
            });
        }

        public async Task UpdateMapPreferencesAsync(MapPreferences preferences)
        {
            await SetSettingAsync(MAP_PREFERENCES_KEY, preferences);
        }

        public async Task<Models.GeoCoordinates> GetLastKnownLocationAsync()
        {
            return await GetSettingAsync<Models.GeoCoordinates>(LAST_LOCATION_KEY, new Models.GeoCoordinates
            {
                Latitude = AppSettings.DefaultLatitude,
                Longitude = AppSettings.DefaultLongitude
            });
        }

        public async Task SaveLastKnownLocationAsync(Models.GeoCoordinates coordinates)
        {
            await SetSettingAsync(LAST_LOCATION_KEY, coordinates);
        }

        #endregion

        #region Gestion du Cache

        public async Task<long> GetCacheSizeAsync()
        {
            // Cette implémentation est simplifiée
            return 0; // À implémenter avec un vrai système de cache
        }

        public async Task ClearCacheAsync()
        {
            // À implémenter avec un vrai système de cache
            await Task.CompletedTask;
        }

        public async Task ConfigureCacheAsync(CacheSettings settings)
        {
            // À implémenter avec un vrai système de cache
            await Task.CompletedTask;
        }

        #endregion

        #region Import/Export

        public async Task<string> ExportSettingsAsync()
        {
            // Construction d'un objet contenant tous les paramètres
            var allSettings = new
            {
                Theme = await GetCurrentThemeAsync(),
                FollowSystemTheme = await GetFollowSystemThemeAsync(),
                NotificationPreferences = await GetNotificationPreferencesAsync(),
                PrivacySettings = await GetPrivacySettingsAsync(),
                MapPreferences = await GetMapPreferencesAsync()
            };

            return JsonSerializer.Serialize(allSettings);
        }

        public async Task ImportSettingsAsync(string settingsJson)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(settingsJson, options);

                // Traitement simplifié, à adapter selon votre format d'export
                if (settings.TryGetValue("Theme", out var themeValue))
                {
                    if (Enum.TryParse<InterfaceAppTheme>(themeValue.GetString(), out var theme))
                        await SetThemeAsync(theme);
                }

                // Les autres paramètres seraient importés de manière similaire
            }
            catch
            {
                // Gérer l'erreur d'importation
            }
        }

        #endregion
    }
}