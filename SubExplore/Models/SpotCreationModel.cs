using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubExplore.Models.DTOs;
using SubExplore.Models.Media;

namespace SubExplore.Models
{
    public class SpotCreationModel
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public SpotType Type { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string RequiredEquipment { get; set; }
        public string SafetyNotes { get; set; }
        public string BestConditions { get; set; }
        public int? MaxDepth { get; set; }
        public CurrentStrength? CurrentStrength { get; set; }
        public bool HasMooring { get; set; }
        public string BottomType { get; set; }
        public List<IMediaFile> Photos { get; set; } = new();

        public DTOs.SpotDto ToDto() => new()
        {
            Name = Name,
            Latitude = Latitude,
            Longitude = Longitude,
            Description = Description,
            Type = Type,
            DifficultyLevel = DifficultyLevel,
            RequiredEquipment = RequiredEquipment,
            SafetyNotes = SafetyNotes,
            BestConditions = BestConditions,
            MaxDepth = MaxDepth,
            CurrentStrength = CurrentStrength,
            HasMooring = HasMooring,
            BottomType = BottomType
        };
    }
}