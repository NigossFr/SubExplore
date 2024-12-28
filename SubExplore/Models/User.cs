using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SubExplore.Models.Enums;

namespace SubExplore.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Le pseudo ne peut contenir que des lettres, chiffres, tirets et underscores")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Url]
        public string? AvatarUrl { get; set; }

        [Required]
        public AccountType AccountType { get; set; } = AccountType.Standard;

        [Required]
        public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Free;

        public ExpertiseLevel? ExpertiseLevel { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Certifications { get; set; }

        [Required]
        public int ReputationScore { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLogin { get; set; }

        // Relations
        public virtual UserPreferences? Preferences { get; set; }
        public virtual ICollection<Spot> CreatedSpots { get; set; } = new List<Spot>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Story> Stories { get; set; } = new List<Story>();
        public virtual ICollection<SpotRating> SpotRatings { get; set; } = new List<SpotRating>();
        public virtual ICollection<UserRating> GivenRatings { get; set; } = new List<UserRating>();
        public virtual ICollection<UserRating> ReceivedRatings { get; set; } = new List<UserRating>();
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<ModeratorExpertise> ModeratorExpertises { get; set; } = new List<ModeratorExpertise>();
        public virtual Professional? Professional { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public virtual ICollection<ModerationAction> ModerationActions { get; set; } = new List<ModerationAction>();
        public virtual ICollection<ModerationStats> ModerationStats { get; set; } = new List<ModerationStats>();
    }

    public class UserPreferences
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string Theme { get; set; } = "light";

        public string DisplayNamePreference { get; set; } = "username";

        [Column(TypeName = "jsonb")]
        public string NotificationSettings { get; set; } = "{}";

        public string Language { get; set; } = "fr";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }

    public class Professional
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        public BusinessType BusinessType { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        [Required]
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

        [Column(TypeName = "jsonb")]
        public string ContactInfo { get; set; } = "{}";

        public DateTime? FeaturedUntil { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }

    public class Story
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public StoryStatus Status { get; set; } = StoryStatus.Draft;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }

    public class UserRating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RaterId { get; set; }

        [Required]
        public int RatedUserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RaterId")]
        public virtual User? Rater { get; set; }

        [ForeignKey("RatedUserId")]
        public virtual User? RatedUser { get; set; }
    }

    public class ModerationAction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModeratorId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public int EntityId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        [Required]
        public ModerationActionStatus Status { get; set; } = ModerationActionStatus.Pending;

        [ForeignKey("ModeratorId")]
        public virtual User? Moderator { get; set; }
    }

    public class ModerationStats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModeratorId { get; set; }

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        [Required]
        public int SpotsValidated { get; set; }

        [Required]
        public int ReportsHandled { get; set; }

        [Required]
        public int ResponseTimeAvg { get; set; }

        [Required]
        [Range(0, 100)]
        public float QualityScore { get; set; }

        [ForeignKey("ModeratorId")]
        public virtual User? Moderator { get; set; }
    }

    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public SubscriptionPlan PlanType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public SubscriptionStatus Status { get; set; }

        [Column(TypeName = "jsonb")]
        public string? PaymentInfo { get; set; }

        public bool IsGifted { get; set; }

        public int? GiftedById { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("GiftedById")]
        public virtual User? GiftedBy { get; set; }
    }

}