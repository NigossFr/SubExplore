using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Models.DTOs
{
    /// <summary>
    /// Paramètres de recherche pour les spots
    /// </summary>
    public class SpotSearchParameters
    {
        public string? SearchTerm { get; set; }
        public IEnumerable<int>? ActivityTypes { get; set; }
        public int? MinDepth { get; set; }
        public int? MaxDepth { get; set; }
        public string? DifficultyLevel { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusInKm { get; set; }
        public bool? ValidatedOnly { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
