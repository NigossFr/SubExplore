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
    public class Spot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        [Column(TypeName = "decimal(10,8)")]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        [Column(TypeName = "decimal(11,8)")]
        public decimal Longitude { get; set; }

        [Required]
        public DifficultyLevel DifficultyLevel { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public string RequiredEquipment { get; set; } = string.Empty;

        [Required]
        public string SafetyNotes { get; set; } = string.Empty;

        [Required]
        public string BestConditions { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public SpotValidationStatus ValidationStatus { get; set; } = SpotValidationStatus.Pending;

        public DateTime? LastSafetyReview { get; set; }

        [Column(TypeName = "jsonb")]
        public string? SafetyFlags { get; set; }

        // Caractéristiques spécifiques aux activités
        [Range(0, 200)]
        public int? MaxDepth { get; set; }

        public CurrentStrength? CurrentStrength { get; set; }

        public bool? HasMooring { get; set; }

        [MaxLength(100)]
        public string? BottomType { get; set; }

        // Navigation properties
        [ForeignKey("CreatorId")]
        public virtual User? Creator { get; set; }

        [ForeignKey("TypeId")]
        public virtual SpotType? Type { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<SpotRating> Ratings { get; set; } = new List<SpotRating>();
        public virtual ICollection<SpotMedia> Media { get; set; } = new List<SpotMedia>();
        public virtual ICollection<SpotValidation> Validations { get; set; } = new List<SpotValidation>();
        public virtual ICollection<SpotReport> Reports { get; set; } = new List<SpotReport>();
    }

    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SpotId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("SpotId")]
        public virtual Spot? Spot { get; set; }
    }

    public class SpotRating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SpotId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("SpotId")]
        public virtual Spot? Spot { get; set; }
    }

    public class SpotValidation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SpotId { get; set; }

        [Required]
        public int ModeratorId { get; set; }

        [Required]
        public ValidationStatus Status { get; set; }

        [Required]
        public DateTime ValidationDate { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("SpotId")]
        public virtual Spot? Spot { get; set; }

        [ForeignKey("ModeratorId")]
        public virtual User? Moderator { get; set; }
    }

    public class SpotReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SpotId { get; set; }

        [Required]
        public int ReporterId { get; set; }

        [Required]
        public ReportReason Reason { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public int? ModeratorId { get; set; }

        public DateTime? ResolvedAt { get; set; }

        // Navigation properties
        [ForeignKey("SpotId")]
        public virtual Spot? Spot { get; set; }

        [ForeignKey("ReporterId")]
        public virtual User? Reporter { get; set; }

        [ForeignKey("ModeratorId")]
        public virtual User? Moderator { get; set; }
    }

   
}
