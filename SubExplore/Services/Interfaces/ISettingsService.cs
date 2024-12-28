using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Services.Interfaces
{
    /// <summary>
    /// Interface du service de gestion des paramètres
    /// </summary>
    public interface ISettingsService
    {
        #region Paramètres Généraux

        /// <summary>
        /// Obtient un paramètre
        /// </summary>
        /// <typeparam name="T">Type de la valeur</typeparam>
        /// <param name="key">Clé du paramètre</param>
        /// <param name="defaultValue">Valeur par défaut</param>
        Task<T> GetSettingAsync<T>(string key, T defaultValue = default);

        /// <summary>
        /// Définit un paramètre
        /// </summary>
        /// <typeparam name="T">Type de la valeur</typeparam>
        /// <param name="key">Clé du paramètre</param>
        /// <param name="value">Valeur à stocker</param>
        Task SetSettingAsync<T>(string key, T value);

        /// <summary>
        /// Supprime un paramètre
        /// </summary>
        /// <param name="key">Clé du paramètre</param>
        Task RemoveSettingAsync(string key);

        /// <summary>
        /// Vérifie si un paramètre existe
        /// </summary>
        /// <param name="key">Clé du paramètre</param>
        Task<bool> ContainsKeyAsync(string key);

        #endregion

        #region Thème et Apparence

        /// <summary>
        /// Obtient le thème actuel
        /// </summary>
        Task<AppTheme> GetCurrentThemeAsync();

        /// <summary>
        /// Définit le thème de l'application
        /// </summary>
        /// <param name="theme">Thème à appliquer</param>
        Task SetThemeAsync(AppTheme theme);

        /// <summary>
        /// Obtient si le thème suit le système
        /// </summary>
        Task<bool> GetFollowSystemThemeAsync();

        /// <summary>
        /// Active/désactive le suivi du thème système
        /// </summary>
        /// <param name="follow">true pour suivre le système</param>
        Task SetFollowSystemThemeAsync(bool follow);

        #endregion

        #region Notifications

        /// <summary>
        /// Obtient les préférences de notification
        /// </summary>
        Task<NotificationPreferences> GetNotificationPreferencesAsync();

        /// <summary>
        /// Met à jour les préférences de notification
        /// </summary>
        /// <param name="preferences">Nouvelles préférences</param>
        Task UpdateNotificationPreferencesAsync(NotificationPreferences preferences);

        /// <summary>
        /// Vérifie si les notifications sont autorisées pour un type donné
        /// </summary>
        /// <param name="notificationType">Type de notification</param>
        Task<bool> IsNotificationEnabledAsync(NotificationType notificationType);

        #endregion

        #region Confidentialité et Sécurité

        /// <summary>
        /// Obtient les paramètres de confidentialité
        /// </summary>
        Task<PrivacySettings> GetPrivacySettingsAsync();

        /// <summary>
        /// Met à jour les paramètres de confidentialité
        /// </summary>
        /// <param name="settings">Nouveaux paramètres</param>
        Task UpdatePrivacySettingsAsync(PrivacySettings settings);

        /// <summary>
        /// Stocke une information de manière sécurisée
        /// </summary>
        /// <typeparam name="T">Type de la donnée</typeparam>
        /// <param name="key">Clé</param>
        /// <param name="value">Valeur à stocker</param>
        Task SetSecureStorageAsync<T>(string key, T value);

        /// <summary>
        /// Récupère une information stockée de manière sécurisée
        /// </summary>
        /// <typeparam name="T">Type de la donnée</typeparam>
        /// <param name="key">Clé</param>
        Task<T> GetSecureStorageAsync<T>(string key);

        #endregion

        #region Préférences de Navigation

        /// <summary>
        /// Obtient les préférences de carte
        /// </summary>
        Task<MapPreferences> GetMapPreferencesAsync();

        /// <summary>
        /// Met à jour les préférences de carte
        /// </summary>
        /// <param name="preferences">Nouvelles préférences</param>
        Task UpdateMapPreferencesAsync(MapPreferences preferences);

        /// <summary>
        /// Obtient la dernière position connue
        /// </summary>
        Task<GeoCoordinates> GetLastKnownLocationAsync();

        /// <summary>
        /// Enregistre la dernière position
        /// </summary>
        /// <param name="coordinates">Coordonnées à sauvegarder</param>
        Task SaveLastKnownLocationAsync(GeoCoordinates coordinates);

        #endregion

        #region Gestion du Cache

        /// <summary>
        /// Obtient la taille actuelle du cache
        /// </summary>
        Task<long> GetCacheSizeAsync();

        /// <summary>
        /// Vide le cache de l'application
        /// </summary>
        Task ClearCacheAsync();

        /// <summary>
        /// Configure les paramètres de mise en cache
        /// </summary>
        /// <param name="settings">Paramètres de cache</param>
        Task ConfigureCacheAsync(CacheSettings settings);

        #endregion

        #region Import/Export

        /// <summary>
        /// Exporte tous les paramètres
        /// </summary>
        /// <returns>Paramètres au format JSON</returns>
        Task<string> ExportSettingsAsync();

        /// <summary>
        /// Importe des paramètres
        /// </summary>
        /// <param name="settingsJson">Paramètres au format JSON</param>
        Task ImportSettingsAsync(string settingsJson);

        #endregion
    }

    public enum AppTheme
    {
        Light,
        Dark,
        System
    }

    public class NotificationPreferences
    {
        public bool EnablePush { get; set; }
        public bool EnableEmail { get; set; }
        public Dictionary<NotificationType, bool> TypePreferences { get; set; }
        public TimeSpan QuietHoursStart { get; set; }
        public TimeSpan QuietHoursEnd { get; set; }
        public bool EnableQuietHours { get; set; }
    }

    public enum NotificationType
    {
        SpotValidation,
        SpotComment,
        SecurityAlert,
        NewsUpdate,
        ModerationRequest
    }

    public class PrivacySettings
    {
        public bool ShareLocation { get; set; }
        public bool AllowProfileVisibility { get; set; }
        public bool EnableAnalytics { get; set; }
        public bool ShareDiveHistory { get; set; }
        public Dictionary<string, bool> CustomPrivacyFlags { get; set; }
    }

    public class MapPreferences
    {
        public bool ShowClusteredMarkers { get; set; }
        public bool AutoRotateMap { get; set; }
        public bool Show3DBuildings { get; set; }
        public string DefaultMapType { get; set; }
        public int DefaultZoomLevel { get; set; }
        public bool EnableOfflineMode { get; set; }
        public Dictionary<string, bool> LayerVisibility { get; set; }
    }

    public class CacheSettings
    {
        public long MaxCacheSize { get; set; }
        public TimeSpan CacheExpiration { get; set; }
        public bool EnableOfflineCache { get; set; }
        public List<string> CacheableContentTypes { get; set; }
    }
}
