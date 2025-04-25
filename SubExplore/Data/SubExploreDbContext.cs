using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubExplore.Models;
using SubExplore.Models.Enums;

namespace SubExplore.Data
{
    public class SubExploreDbContext : DbContext
    {
        public SubExploreDbContext(DbContextOptions<SubExploreDbContext> options)
            : base(options)
        {
        }

        // Définition des DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<SpotMedia> SpotMedia { get; set; }
        public DbSet<SpotType> SpotTypes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SpotRating> SpotRatings { get; set; }
        public DbSet<SpotValidation> SpotValidations { get; set; }
        public DbSet<SpotReport> SpotReports { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationType> OrganizationTypes { get; set; }
        public DbSet<OrganizationDocument> OrganizationDocuments { get; set; }
        public DbSet<BusinessHours> BusinessHours { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<ModeratorExpertise> ModeratorExpertises { get; set; }
        public DbSet<ModerationAction> ModerationActions { get; set; }
        public DbSet<ModerationStats> ModerationStats { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryCategory> StoryCategories { get; set; }
        public DbSet<StoryMedia> StoryMedia { get; set; }
        public DbSet<StoryComment> StoryComments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<SubscriptionGift> SubscriptionGifts { get; set; }
        public DbSet<Professional> Professionals { get; set; }
        public DbSet<ValidationRequirement> ValidationRequirements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();

                entity.HasOne(e => e.Preferences)
                      .WithOne(e => e.User)
                      .HasForeignKey<UserPreferences>(e => e.UserId);

                entity.HasMany(e => e.CreatedSpots)
                      .WithOne(e => e.Creator)
                      .HasForeignKey(e => e.CreatorId);

                entity.HasMany(e => e.Comments)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.Stories)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.SpotRatings)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.GivenRatings)
                      .WithOne(e => e.Rater)
                      .HasForeignKey(e => e.RaterId);

                entity.HasMany(e => e.ReceivedRatings)
                      .WithOne(e => e.RatedUser)
                      .HasForeignKey(e => e.RatedUserId);

                entity.HasMany(e => e.ModeratorExpertises)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.ModerationActions)
                      .WithOne(e => e.Moderator)
                      .HasForeignKey(e => e.ModeratorId);

                entity.HasMany(e => e.ModerationStats)
                      .WithOne(e => e.Moderator)
                      .HasForeignKey(e => e.ModeratorId);

                entity.HasMany(e => e.Memberships)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<Spot>(entity =>
            {
                entity.HasIndex(e => new { e.Latitude, e.Longitude });
                entity.HasIndex(e => e.TypeId);
                entity.HasIndex(e => e.ValidationStatus);

                entity.HasMany(e => e.Comments)
                      .WithOne(e => e.Spot)
                      .HasForeignKey(e => e.SpotId);

                entity.HasMany(e => e.Ratings)
                      .WithOne(e => e.Spot)
                      .HasForeignKey(e => e.SpotId);

                entity.HasMany(e => e.Media)
                      .WithOne(e => e.Spot)
                      .HasForeignKey(e => e.SpotId);

                entity.HasMany(e => e.Validations)
                      .WithOne(e => e.Spot)
                      .HasForeignKey(e => e.SpotId);

                entity.HasMany(e => e.Reports)
                      .WithOne(e => e.Spot)
                      .HasForeignKey(e => e.SpotId);
            });

            modelBuilder.Entity<SpotType>(entity =>
            {
                entity.HasMany(e => e.Spots)
                      .WithOne(e => e.Type)
                      .HasForeignKey(e => e.TypeId);

                entity.HasMany(e => e.ModeratorExpertises)
                      .WithOne(e => e.SpotType)
                      .HasForeignKey(e => e.SpotTypeId);

                entity.HasMany(e => e.ValidationRequirements)
                      .WithOne(e => e.SpotType)
                      .HasForeignKey(e => e.SpotTypeId);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasIndex(e => e.TypeId);
                entity.HasIndex(e => e.VerificationStatus);

                entity.HasMany(e => e.Services)
                      .WithOne(e => e.Organization)
                      .HasForeignKey(e => e.OrganizationId);

                entity.HasMany(e => e.BusinessHours)
                      .WithOne(e => e.Organization)
                      .HasForeignKey(e => e.OrganizationId);

                entity.HasMany(e => e.Documents)
                      .WithOne(e => e.Organization)
                      .HasForeignKey(e => e.OrganizationId);

                entity.HasMany(e => e.Memberships)
                      .WithOne(e => e.Organization)
                      .HasForeignKey(e => e.OrganizationId);
            });

            modelBuilder.Entity<Story>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CategoryId);

                entity.HasMany(e => e.Media)
                      .WithOne(e => e.Story)
                      .HasForeignKey(e => e.StoryId);

                entity.HasMany(e => e.Comments)
                      .WithOne(e => e.Story)
                      .HasForeignKey(e => e.StoryId);
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => new { e.UserId, e.OrganizationId }).IsUnique();
            });

            modelBuilder.Entity<SpotRating>(entity =>
            {
                entity.HasIndex(e => e.SpotId);
                entity.HasIndex(e => new { e.UserId, e.SpotId }).IsUnique();
            });

            modelBuilder.Entity<UserRating>(entity =>
            {
                entity.HasIndex(e => e.RatedUserId);
                entity.HasIndex(e => new { e.RaterId, e.RatedUserId }).IsUnique();
            });

            modelBuilder.Entity<ModeratorExpertise>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.SpotTypeId }).IsUnique();
            });

            // Configuration types JSON pour MySQL
            modelBuilder.Entity<User>()
                .Property(e => e.Certifications)
                .HasColumnType("json");

            modelBuilder.Entity<UserPreferences>()
                .Property(e => e.NotificationSettings)
                .HasColumnType("json");

            modelBuilder.Entity<Spot>()
                .Property(e => e.SafetyFlags)
                .HasColumnType("json");

            modelBuilder.Entity<Organization>()
                .Property(e => e.ContactInfo)
                .HasColumnType("json");

            modelBuilder.Entity<Organization>()
                .Property(e => e.SocialLinks)
                .HasColumnType("json");

            modelBuilder.Entity<Organization>()
                .Property(e => e.Certifications)
                .HasColumnType("json");

            modelBuilder.Entity<OrganizationType>()
                .Property(e => e.Requirements)
                .HasColumnType("json");

            modelBuilder.Entity<OrganizationType>()
                .Property(e => e.VerificationProcess)
                .HasColumnType("json");

            modelBuilder.Entity<BusinessHours>()
                .Property(e => e.SpecialHours)
                .HasColumnType("json");

            modelBuilder.Entity<Service>()
                .Property(e => e.ServiceDetails)
                .HasColumnType("json");

            modelBuilder.Entity<SpotType>()
                .Property(e => e.ValidationCriteria)
                .HasColumnType("json");

            modelBuilder.Entity<Subscription>()
                .Property(e => e.PaymentInfo)
                .HasColumnType("json");
        }
    }
}
