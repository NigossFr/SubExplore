using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubExplore.Models;

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
        Task<SubExplore.Models.GeoCoordinates> GeocodeAddressAsync(string address);

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
        Task<double> CalculateDistanceAsync(SubExplore.Models.GeoCoordinates point1, SubExplore.Models.GeoCoordinates point2);

        /// <summary>
        /// Trouve les points d'intérêt dans un rayon donné
        /// </summary>
        /// <param name="center">Point central</param>
        /// <param name="radiusKm">Rayon en kilomètres</param>
        /// <param name="type">Type de point d'intérêt (optionnel)</param>
        /// <returns>Points d'intérêt trouvés avec leur distance</returns>
        Task<IEnumerable<LocationWithDistance>> FindInRadiusAsync(
            SubExplore.Models.GeoCoordinates center,
            double radiusKm,
            LocationType? type = null);

        /// <summary>
        /// Optimise un ensemble de points pour l'affichage sur la carte
        /// </summary>
        /// <param name="points">Points à optimiser</param>
        /// <param name="zoomLevel">Niveau de zoom</param>
        /// <returns>Points optimisés et regroupés</returns>
        Task<IEnumerable<ClusteredLocation>> ClusterLocationsAsync(
            IEnumerable<SubExplore.Models.GeoCoordinates> points,
            int zoomLevel);

        #endregion

        #region Données Marine

        /// <summary>
        /// Récupère la profondeur approximative à une position donnée
        /// </summary>
        /// <param name="coordinates">Coordonnées du point</param>
        /// <returns>Profondeur en mètres</returns>
        Task<double?> GetApproximateDepthAsync(SubExplore.Models.GeoCoordinates coordinates);

        /// <summary>
        /// Vérifie si une position est dans une zone protégée
        /// </summary>
        /// <param name="coordinates">Coordonnées à vérifier</param>
        /// <returns>Informations sur la zone protégée si applicable</returns>
        Task<ProtectedAreaInfo> CheckProtectedAreaAsync(SubExplore.Models.GeoCoordinates coordinates);

        /// <summary>
        /// Récupère les points d'amarrage à proximité
        /// </summary>
        /// <param name="coordinates">Coordonnées du point</param>
        /// <param name="radiusKm">Rayon de recherche</param>
        /// <returns>Points d'amarrage trouvés</returns>
        Task<IEnumerable<MooringPoint>> GetNearbyMooringPointsAsync(
            SubExplore.Models.GeoCoordinates coordinates,
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
        Task<MaritimeValidationResult> ValidateMaritimeLocationAsync(SubExplore.Models.GeoCoordinates coordinates);

        /// <summary>
        /// Vérifie l'accessibilité d'une position
        /// </summary>
        /// <param name="coordinates">Coordonnées à vérifier</param>
        /// <returns>Informations sur l'accessibilité</returns>
        Task<AccessibilityInfo> CheckLocationAccessibilityAsync(SubExplore.Models.GeoCoordinates coordinates);

        #endregion
    }

    // Les autres classes restent identiques à votre fichier original
    // (LocationAddress, BoundingBox, LocationWithDistance, etc.)
}