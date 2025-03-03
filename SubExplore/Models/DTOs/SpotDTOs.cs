using SubExplore.Models.Enums;
using SubExplore.Models.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SubExplore.Models.DTOs
{
    // Classes DTO pour les spots
    public class SpotDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propriétés additionnelles nécessaires selon SpotCreationModel
        public SpotType Type { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string RequiredEquipment { get; set; } = string.Empty;
        public string SafetyNotes { get; set; } = string.Empty;
        public string BestConditions { get; set; } = string.Empty;
        public int? MaxDepth { get; set; }
        public CurrentStrength? CurrentStrength { get; set; }
        public bool HasMooring { get; set; }
        public string BottomType { get; set; } = string.Empty;
    }

    public class SpotCreationDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Propriétés additionnelles selon SpotCreationModel
        public SpotType Type { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string RequiredEquipment { get; set; } = string.Empty;
        public string SafetyNotes { get; set; } = string.Empty;
        public string BestConditions { get; set; } = string.Empty;
        public int? MaxDepth { get; set; }
        public CurrentStrength? CurrentStrength { get; set; }
        public bool HasMooring { get; set; }
        public string BottomType { get; set; } = string.Empty;
    }

    // Le reste de vos DTOs reste inchangé
    public class SpotUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class SpotValidationDto
    {
        public bool IsApproved { get; set; }
        public string? Comments { get; set; }
    }

    public class SpotRejectionDto
    {
        public string Reason { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }

    public class SpotReportDto
    {
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class SpotMediaDto
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
    }

    public class SpotMediaUploadDto
    {
        public IMediaFile File { get; set; } = null!;
    }

    public class SpotRatingDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class SpotRatingStatsDto
    {
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

    public class SpotVisitStatsDto
    {
        public int TotalVisits { get; set; }
        public int UniqueVisitors { get; set; }
    }

    public class SpotConditionsDto
    {
        public string? CurrentStrength { get; set; }
        public int? Visibility { get; set; }
        public string? Notes { get; set; }
    }

    public class SpotSecurityUpdateDto
    {
        public bool IsSafe { get; set; }
        public string? SecurityNotes { get; set; }
    }

    // Adaptateur complet qui implémente tous les membres requis
    public class FormFileAdapter : IMediaFile
    {
        private readonly Stream _stream;

        public FormFileAdapter(Stream stream, string fileName, string contentType, long length, string path = "", bool isPrimary = false)
        {
            _stream = stream;
            FileName = fileName;
            ContentType = contentType;
            Length = length;
            Path = path;
            MimeType = contentType;
            Size = length;
            IsPrimary = isPrimary;
        }

        public string FileName { get; }
        public string ContentType { get; }
        public long Length { get; }
        public string Path { get; }
        public string MimeType { get; }
        public long Size { get; }
        public bool IsPrimary { get; set; } // Changé de { get; } à { get; set; }

        public Stream OpenReadStream() => _stream;

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            await _stream.CopyToAsync(target, cancellationToken);
        }
    }
}