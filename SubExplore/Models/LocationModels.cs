using System;
using System.Collections.Generic;

namespace SubExplore.Models
{
    // Type de points d'intérêt
    public enum LocationType
    {
        DivingSite,
        SnorkelingSite,
        Beach,
        Marina,
        DiveShop,
        MooringPoint
    }

    // Classe pour les résultats de géocodage inverse
    public class LocationAddress
    {
        public string FormattedAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public Dictionary<string, string> ExtraComponents { get; set; } = new Dictionary<string, string>();
    }

    public class GeoLocation
    {
        public GeoCoordinates Coordinates { get; set; } = new GeoCoordinates();
        public LocationType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Classe pour les points avec distance
    public class LocationWithDistance
    {
        public GeoLocation Location { get; set; } = new GeoLocation();
        public double DistanceKm { get; set; }
    }

    // Classe pour les emplacements regroupés (clusters)
    public class ClusteredLocation
    {
        public GeoCoordinates Center { get; set; } = new GeoCoordinates();
        public int PointCount { get; set; }
        public List<GeoLocation> Points { get; set; } = new List<GeoLocation>();
        public Dictionary<LocationType, int> TypeCounts { get; set; } = new Dictionary<LocationType, int>();
    }

    // Classe pour les zones protégées
    public class ProtectedAreaInfo
    {
        public bool IsProtected { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string ProtectionLevel { get; set; } = string.Empty;
        public string Restrictions { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalInfo { get; set; } = new Dictionary<string, string>();
    }

    // Classe pour les points d'amarrage
    public class MooringPoint
    {
        public string Id { get; set; } = string.Empty;
        public GeoCoordinates Location { get; set; } = new GeoCoordinates();
        public string Type { get; set; } = string.Empty;
        public int? MaxBoatLength { get; set; }
        public bool IsAvailable { get; set; }
        public string Condition { get; set; } = string.Empty;
    }

    // Classe pour la zone géographique (bounding box)
    public class BoundingBox
    {
        public GeoCoordinates NorthEast { get; set; } = new GeoCoordinates();
        public GeoCoordinates SouthWest { get; set; } = new GeoCoordinates();
    }

    // Classe pour validation maritime
    public class MaritimeValidationResult
    {
        public bool IsValid { get; set; }
        public bool IsInWater { get; set; }
        public string ValidationMessage { get; set; } = string.Empty;
        public List<string>? Warnings { get; set; }
        public Dictionary<string, object>? ValidationDetails { get; set; }
    }

    // Classe pour l'accessibilité
    public class AccessibilityInfo
    {
        public bool IsAccessible { get; set; }
        public string AccessType { get; set; } = string.Empty;
        public List<string>? Requirements { get; set; }
        public List<string>? Restrictions { get; set; }
        public string ParkingInfo { get; set; } = string.Empty;
    }
}