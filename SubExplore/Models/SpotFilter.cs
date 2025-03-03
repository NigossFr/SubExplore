using System.Collections.Generic;
using SubExplore.Models.Enums;

namespace SubExplore.Models.DTOs
{
    /// <summary>
    /// Critères de filtrage pour la recherche de spots
    /// </summary>
    public class SpotFilter
    {
        /// <summary>
        /// Terme de recherche (nom, description)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Liste des types d'activité à inclure
        /// </summary>
        public IEnumerable<int>? ActivityTypes { get; set; }

        /// <summary>
        /// Profondeur minimale en mètres
        /// </summary>
        public int? MinDepth { get; set; }

        /// <summary>
        /// Profondeur maximale en mètres
        /// </summary>
        public int? MaxDepth { get; set; }

        /// <summary>
        /// Niveau de difficulté
        /// </summary>
        public string? DifficultyLevel { get; set; }

        /// <summary>
        /// Latitude du centre de recherche
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude du centre de recherche
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Rayon de recherche en kilomètres
        /// </summary>
        public double? RadiusInKm { get; set; }

        /// <summary>
        /// Si vrai, ne retourne que les spots validés
        /// </summary>
        public bool? ValidatedOnly { get; set; } = true;

        /// <summary>
        /// Si vrai, recherche dans la zone actuelle (position utilisateur)
        /// </summary>
        public bool WithinCurrentArea { get; set; } = false;

        /// <summary>
        /// Si vrai, recherche dans les endroits favoris
        /// </summary>
        public bool FavoritesOnly { get; set; } = false;

        /// <summary>
        /// Si vrai, recherche dans les endroits visités
        /// </summary>
        public bool VisitedOnly { get; set; } = false;

        /// <summary>
        /// Type d'ordre pour les résultats
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Si vrai, ordre descendant
        /// </summary>
        public bool SortDescending { get; set; }

        /// <summary>
        /// Numéro de la page de résultats
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Nombre d'éléments par page
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Réinitialise tous les critères de filtre
        /// </summary>
        public void Reset()
        {
            SearchTerm = null;
            ActivityTypes = null;
            MinDepth = null;
            MaxDepth = null;
            DifficultyLevel = null;
            Latitude = null;
            Longitude = null;
            RadiusInKm = null;
            ValidatedOnly = true;
            WithinCurrentArea = false;
            FavoritesOnly = false;
            VisitedOnly = false;
            SortBy = null;
            SortDescending = false;
            PageNumber = 1;
            PageSize = 20;
        }
    }
}