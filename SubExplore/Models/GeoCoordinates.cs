using System;

namespace SubExplore.Models
{
    /// <summary>
    /// Représente des coordonnées géographiques avec latitude, longitude et précision
    /// </summary>
    public class GeoCoordinates
    {
        /// <summary>
        /// Latitude du point géographique
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude du point géographique
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Précision des coordonnées (optionnel)
        /// </summary>
        public double? Accuracy { get; set; }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public GeoCoordinates() { }

        /// <summary>
        /// Constructeur avec paramètres
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="accuracy">Précision optionnelle</param>
        public GeoCoordinates(double latitude, double longitude, double? accuracy = null)
        {
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
        }

        /// <summary>
        /// Calcule la distance entre deux points géographiques
        /// </summary>
        /// <param name="other">Autres coordonnées</param>
        /// <returns>Distance en kilomètres</returns>
        public double DistanceTo(GeoCoordinates other)
        {
            const double earthRadius = 6371; // Rayon de la Terre en kilomètres

            var dLat = ToRadians(other.Latitude - Latitude);
            var dLon = ToRadians(other.Longitude - Longitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        /// <summary>
        /// Convertit des degrés en radians
        /// </summary>
        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// Représentation textuelle des coordonnées
        /// </summary>
        public override string ToString()
        {
            return $"Lat: {Latitude}, Lon: {Longitude}";
        }

        /// <summary>
        /// Vérifie l'égalité de deux coordonnées
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is GeoCoordinates coordinates &&
                   Latitude == coordinates.Latitude &&
                   Longitude == coordinates.Longitude;
        }

        /// <summary>
        /// Génère un code de hachage pour les coordonnées
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude);
        }
    }
}