using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Constants
{
    public static class AppSettings
    {
        // Configuration générale de l'app
        public const string AppName = "SubExplore";
        public const string AppVersion = "1.0.0";
        public const string AppDescription = "Application communautaire de sports sous-marins";
        public const string CompanyName = "SubExplore";

        // Limites et contraintes
        public const int MaxPhotosPerSpot = 3;
        public const int MaxFileSizeMB = 5;
        public const int MinimumPasswordLength = 8;
        public const int MaxUsernameLength = 30;
        public const int MinUsernameLength = 3;
        public const int MaxDescriptionLength = 2000;
        public const int MaxTitleLength = 100;

        // Timeouts
        public const int ApiTimeoutSeconds = 30;
        public const int LocationTimeoutSeconds = 15;
        public const int CacheExpirationMinutes = 60;
        public const int SessionTimeoutMinutes = 30;

        // Géolocalisation
        public const double DefaultLatitude = 43.2965;  // Marseille
        public const double DefaultLongitude = 5.3698;
        public const int DefaultZoomLevel = 12;
        public const double MaxSearchRadiusKm = 50;
        public const double ClusteringDistanceMeters = 50;

        // Validation
        public const int MinimumSpotPhotos = 1;
        public const string AllowedImageExtensions = ".jpg,.jpeg,.png";
        public const string AllowedVideoExtensions = ".mp4,.mov";
        public const int MaxVideoLengthSeconds = 30;

        // Modération
        public const int ModeratorMinimumLevel = 2;
        public const int RequiredValidationsCount = 2;
        public const int MaxReportsBeforeReview = 3;

        // Cache
        public const int MaxOfflineCacheMB = 500;
        public const int MaxMapCacheMB = 200;
        public const int MaxMediaCacheMB = 300;

        // Notifications
        public const int MaxNotificationsPerDay = 10;
        public const int QuietHoursStart = 22;
        public const int QuietHoursEnd = 7;
    }
}

