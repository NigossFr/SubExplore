using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Devices.Sensors;

namespace SubExplore.Services.Interfaces
{
    /// <summary>
    /// Interface du service de gestion des localisations
    /// </summary>
    public interface ILocationService
    {
        #region Géocodage

        /// <summary>
        /// Convertit une adresse en coordonnées géographiques
        /// </summary>
        /// <param name="address">Adresse à convertir</param>
        /// <returns>Coordonnées géographiques</returns>
        Task<GeoCoordinates> GeocodeAddressAsync(string address);

        /// <summary>
        /// Convertit des coordonnées en adresse
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>Détails de l'adresse</returns>
        Task<LocationAddress> ReverseGeocodeAsync(double latitude, double longitude);

        #endregion

        #region Calculs de Distance

        /// <summary>
        /// Calcule la distance entre deux points
        /// </summary>
        /// <param name="point1">Premier point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Distance en kilomètres</returns>
        Task<double> CalculateDistanceAsync(GeoCoordinates point1, GeoCoordinates point2);

        /// <summary>
        /// Trouve les points d'intérêt dans un rayon donné
        /// </summary>
        /// <param name="center">Point central</param>
        /// <param name="radiusKm">Rayon en kilomètres</param>
        /// <param name="type">Type de point d'intérêt (optionnel)</param>
        /// <returns>Points d'intérêt trouvés avec leur distance</returns>
        Task<IEnumerable<LocationWithDistance>> FindInRadiusAsync(
            GeoCoordinates center,
            double radiusKm,
            LocationType? type = null);

        /// <summary>
        /// Optimise un ensemble de points pour l'affichage sur la carte
        /// </summary>
        /// <param name="points">Points à optimiser</param>
        /// <param name="zoomLevel">Niveau de zoom</param>
        /// <returns>Points optimisés et regroupés</returns>
        Task<IEnumerable<ClusteredLocation>> ClusterLocationsAsync(
            IEnumerable<GeoLocation> points,
            int zoomLevel);

        #endregion

        #region Données Marine

        /// <summary>
        /// Récupère la profondeur approximative à une position donnée
        /// </summary>
        /// <param name="coordinates">Coordonnées du point</param>
        /// <returns>Profondeur en mètres</returns>
        Task<double?> GetApproximateDepthAsync(GeoCoordinates coordinates);

        /// <summary>
        /// Vérifie si une position est dans une zone protégée
        /// </summary>
        /// <param name="coordinates">Coordonnées à vérifier</param>
        /// <returns>Informations sur la zone protégée si applicable</returns>
        Task<ProtectedAreaInfo> CheckProtectedAreaAsync(GeoCoordinates coordinates);

        /// <summary>
        /// Récupère les points d'amarrage à proximité
        /// </summary>
        /// <param name="coordinates">Coordonnées du point</param>
        /// <param name="radiusKm">Rayon de recherche</param>
        /// <returns>Points d'amarrage trouvés</returns>
        Task<IEnumerable<MooringPoint>> GetNearbyMooringPointsAsync(
            GeoCoordinates coordinates,
            double radiusKm);

        #endregion

        #region Cache et Optimisation

        /// <summary>
        /// Met en cache les données de localisation pour une région
        /// </summary>
        /// <param name="boundingBox">Zone à mettre en cache</param>
        Task PreloadLocationDataAsync(BoundingBox boundingBox);

        /// <summary>
        /// Invalide le cache pour une région spécifique
        /// </summary>
        /// <param name="boundingBox">Zone à invalider</param>
        Task InvalidateLocationCacheAsync(BoundingBox boundingBox);

        #endregion

        #region Validation

        /// <summary>
        /// Valide si des coordonnées sont dans une zone maritime
        /// </summary>
        /// <param name="coordinates">Coordonnées à valider</param>
        /// <returns>Résultat de la validation avec détails</returns>
        Task<MaritimeValidationResult> ValidateMaritimeLocationAsync(GeoCoordinates coordinates);

        /// <summary>
        /// Vérifie l'accessibilité d'une position
        /// </summary>
        /// <param name="coordinates">Coordonnées à vérifier</param>
        /// <returns>Informations sur l'accessibilité</returns>
        Task<AccessibilityInfo> CheckLocationAccessibilityAsync(GeoCoordinates coordinates);

        #endregion
    }

    public class GeoCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Accuracy { get; set; }
    }

    public class LocationAddress
    {
        public string FormattedAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public Dictionary<string, string> ExtraComponents { get; set; }
    }

    public class BoundingBox
    {
        public GeoCoordinates NorthEast { get; set; }
        public GeoCoordinates SouthWest { get; set; }
    }

    public class LocationWithDistance
    {
        public GeoLocation Location { get; set; }
        public double DistanceKm { get; set; }
    }

    public class ClusteredLocation
    {
        public GeoCoordinates Center { get; set; }
        public int PointCount { get; set; }
        public List<GeoLocation> Points { get; set; }
        public Dictionary<LocationType, int> TypeCounts { get; set; }
    }

    public class ProtectedAreaInfo
    {
        public bool IsProtected { get; set; }
        public string AreaName { get; set; }
        public string ProtectionLevel { get; set; }
        public string Restrictions { get; set; }
        public Dictionary<string, string> AdditionalInfo { get; set; }
    }

    public class MooringPoint
    {
        public string Id { get; set; }
        public GeoCoordinates Location { get; set; }
        public string Type { get; set; }
        public int? MaxBoatLength { get; set; }
        public bool IsAvailable { get; set; }
        public string Condition { get; set; }
    }

    public class MaritimeValidationResult
    {
        public bool IsValid { get; set; }
        public bool IsInWater { get; set; }
        public string ValidationMessage { get; set; }
        public List<string> Warnings { get; set; }
        public Dictionary<string, object> ValidationDetails { get; set; }
    }

    public class AccessibilityInfo
    {
        public bool IsAccessible { get; set; }
        public string AccessType { get; set; }
        public List<string> Requirements { get; set; }
        public List<string> Restrictions { get; set; }
        public string ParkingInfo { get; set; }
    }

    public enum LocationType
    {
        DivingSite,
        SnorkelingSite,
        Beach,
        Marina,
        DiveShop,
        MooringPoint
    }
}
