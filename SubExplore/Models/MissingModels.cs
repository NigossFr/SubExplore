using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SubExplore.Models.Media;

namespace SubExplore.Models
{

    public class MediaFile : IMediaFile
    {
        private Stream _stream;

        public string Path { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public bool IsPrimary { get; set; }
        public string Name { get; set; }

        // Ces propriétés sont requises par l'interface IMediaFile
        public string FileName => Name;
        public string ContentType => MimeType;
        public long Length => Size;

        public MediaFile()
        {
            Path = string.Empty;
            MimeType = "image/jpeg";
            Name = string.Empty;
            Size = 0;
            _stream = Stream.Null;
        }

        public Stream OpenReadStream()
        {
            if (_stream != null && _stream != Stream.Null)
            {
                _stream.Position = 0;
                return _stream;
            }

            // Si un chemin de fichier est spécifié, on tente de l'ouvrir
            if (!string.IsNullOrEmpty(Path) && File.Exists(Path))
            {
                return File.OpenRead(Path);
            }

            return Stream.Null;
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            using (var stream = OpenReadStream())
            {
                if (stream == Stream.Null)
                    return;

                await stream.CopyToAsync(target, 81920, cancellationToken);
            }
        }
    }

    // Classe pour les filtres de spots
    public class SpotFilter
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<int> ActivityTypes { get; set; } = new List<int>();
        public int? MinDepth { get; set; }
        public int? MaxDepth { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusInKm { get; set; }
        public bool? ValidatedOnly { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // Modèles supplémentaires pour MediaService
    public class SpotMediaUploadDto
    {
        public DTOs.IFormFile File { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public string Caption { get; set; } = string.Empty;
    }

    public class SpotMediaDto
    {
        public int Id { get; set; }
        public int SpotId { get; set; }
        public Enums.MediaType MediaType { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public long? FileSize { get; set; }
    }

    public class FormFile : DTOs.IFormFile
    {
        private readonly Stream _content;

        public FormFile(string fileName, string contentType, Stream content)
        {
            FileName = fileName;
            ContentType = contentType;
            _content = content;
            Length = content.Length;
        }

        public string FileName { get; }
        public string ContentType { get; }
        public long Length { get; }

        public Stream OpenReadStream() => _content;
    }

    // Enum manquants
    public enum SpotStatus
    {
        Draft,
        PendingReview,
        Active,
        Inactive,
        Rejected
    }

    // Classes pour la gestion des rôles d'utilisateur
    public enum UserRole
    {
        Standard,
        Premium,
        Moderator,
        Admin
    }

    public enum Permission
    {
        CreateSpot,
        EditOwnContent,
        ModerateContent,
        ManageUsers,
        ManageRoles,
        ViewAnalytics
    }

    // Classe pour la mise à jour des métadonnées
    public class MediaMetadataUpdateDto
    {
        public string Caption { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public Dictionary<string, string> CustomMetadata { get; set; } = new Dictionary<string, string>();
    }
}