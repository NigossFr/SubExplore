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
    public class SpotType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? IconPath { get; set; }

        [MaxLength(7)]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$",
            ErrorMessage = "Le code couleur doit être au format hexadécimal (ex: #FF0000)")]
        public string? ColorCode { get; set; }

        [Required]
        public bool RequiresExpertValidation { get; set; }

        [Column(TypeName = "jsonb")]
        public string? ValidationCriteria { get; set; }

        [Required]
        public ActivityCategory Category { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Spot> Spots { get; set; } = new List<Spot>();
        public virtual ICollection<ModeratorExpertise> ModeratorExpertises { get; set; } = new List<ModeratorExpertise>();
        public virtual ICollection<ValidationRequirement> ValidationRequirements { get; set; } = new List<ValidationRequirement>();
    }

    public class ModeratorExpertise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SpotTypeId { get; set; }

        [Required]
        public ExpertiseLevel ExpertiseLevel { get; set; }

        public string? CertificationProof { get; set; }

        public DateTime? ValidatedAt { get; set; }

        public int? ValidatedById { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("SpotTypeId")]
        public virtual SpotType? SpotType { get; set; }

        [ForeignKey("ValidatedById")]
        public virtual User? ValidatedBy { get; set; }
    }


    public class ValidationRequirement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SpotTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public bool IsMandatory { get; set; }

        public string? ValidationInstructions { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        [ForeignKey("SpotTypeId")]
        public virtual SpotType? SpotType { get; set; }
    }
}
