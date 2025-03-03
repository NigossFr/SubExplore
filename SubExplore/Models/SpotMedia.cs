using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SubExplore.Models.Enums;

namespace SubExplore.Models
{
    public class SpotMedia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SpotId { get; set; }

        [Required]
        public Enums.MediaType MediaType { get; set; }

        [Required]
        [MaxLength(500)]
        [Url]
        public string MediaUrl { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public MediaStatus Status { get; set; } = MediaStatus.Pending;

        public string? Caption { get; set; }

        [Required]
        public bool IsPrimary { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        [Range(0, 5242880)] // 5MB en bytes
        public long? FileSize { get; set; }

        public string? ContentType { get; set; }

        public DateTime? LastValidatedAt { get; set; }

        public int? ValidatedByUserId { get; set; }

        // Métadonnées d'upload
        public string? OriginalFileName { get; set; }
        public string? StorageContainer { get; set; }
        public string? StoragePath { get; set; }

        // Navigation properties
        [ForeignKey("SpotId")]
        public virtual Spot? Spot { get; set; }

        [ForeignKey("ValidatedByUserId")]
        public virtual User? ValidatedBy { get; set; }
    }


    // Configurations supplémentaires pour la validation des médias
    public static class MediaValidationConfig
    {
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        public static readonly string[] AllowedVideoExtensions = { ".mp4", ".mov" };
        public static readonly int MaxFileSize = 5 * 1024 * 1024; // 5MB
        public static readonly int MaxPhotosPerSpot = 3;
        public static readonly int MinImageWidth = 800;
        public static readonly int MinImageHeight = 600;
        public static readonly int MaxImageWidth = 4096;
        public static readonly int MaxImageHeight = 4096;

        public static bool IsValidImageExtension(string extension)
        {
            return AllowedImageExtensions.Contains(extension.ToLower());
        }

        public static bool IsValidVideoExtension(string extension)
        {
            return AllowedVideoExtensions.Contains(extension.ToLower());
        }

        public static bool IsValidDimensions(int width, int height)
        {
            return width >= MinImageWidth && width <= MaxImageWidth &&
                   height >= MinImageHeight && height <= MaxImageHeight;
        }
    }
}
