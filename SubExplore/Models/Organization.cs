using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SubExplore.Models;
using SubExplore.Models.Enums;  

namespace SubExplore.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int TypeId { get; set; }

        [MaxLength(20)]
        public string? FederationNumber { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string ContactInfo { get; set; } = "{}";

        [Required]
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

        [Column(TypeName = "jsonb")]
        public string? SocialLinks { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Url]
        public string? LogoUrl { get; set; }

        public string? Address { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTime? LastVerificationDate { get; set; }

        [MaxLength(50)]
        public string? SiretNumber { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Certifications { get; set; }

        // Relations de navigation
        [ForeignKey("TypeId")]
        public virtual OrganizationType? Type { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<BusinessHours> BusinessHours { get; set; } = new List<BusinessHours>();
        public virtual ICollection<OrganizationDocument> Documents { get; set; } = new List<OrganizationDocument>();
        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }

    public class OrganizationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Requirements { get; set; }

        [Column(TypeName = "jsonb")]
        public string? VerificationProcess { get; set; }

        [Required]
        public bool RequiresAnnualVerification { get; set; }

        public string? RequiredDocuments { get; set; }

        // Navigation property
        public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
    }

    public class OrganizationDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        [MaxLength(500)]
        public string DocumentUrl { get; set; } = string.Empty;

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

        public string? ValidationNotes { get; set; }

        // Navigation property
        [ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; }
    }


    public class BusinessHours
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        [Range(1, 7)]
        public int DayOfWeek { get; set; }

        public TimeSpan? OpenTime { get; set; }

        public TimeSpan? CloseTime { get; set; }

        public bool IsClosed { get; set; }

        [Column(TypeName = "jsonb")]
        public string? SpecialHours { get; set; }

        // Navigation property
        [ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; }
    }

    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        public bool IsActive { get; set; } = true;

        [Column(TypeName = "jsonb")]
        public string? ServiceDetails { get; set; }

        // Navigation property
        [ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; }
    }
}
