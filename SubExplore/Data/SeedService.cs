using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SubExplore.Data;
using SubExplore.Models;
using SubExplore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SubExplore.Data
{
    /// <summary>
    /// Service responsable de l'initialisation des données dans la base de données
    /// </summary>
    public class SeedService
    {
        private readonly SubExploreDbContext _context;
        private readonly ILogger<SeedService> _logger;

        public SeedService(SubExploreDbContext context, ILogger<SeedService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Initialise la base de données avec des données essentielles
        /// </summary>
        public async Task SeedDatabaseAsync()
        {
            try
            {
                // Vérifier si la migration a déjà été appliquée
                // Si la table __EFMigrationsHistory existe et contient des entrées, nous supposons que la migration a été appliquée
                // Sinon, nous créons la base de données
                bool databaseExists = await _context.Database.CanConnectAsync();

                if (!databaseExists)
                {
                    _logger.LogInformation("La base de données n'existe pas. Création de la base de données...");
                    await _context.Database.EnsureCreatedAsync();
                    _logger.LogInformation("Base de données créée avec succès.");
                }
                else
                {
                    _logger.LogInformation("Connexion à la base de données réussie.");
                }

                // Vérifier si des données existent déjà
                if (await _context.SpotTypes.AnyAsync())
                {
                    _logger.LogInformation("Des données existent déjà dans la base de données. Initialisation ignorée.");
                    return;
                }

                _logger.LogInformation("Initialisation des données de base...");

                // Ajouter les types de spots
                await SeedSpotTypesAsync();

                // Ajouter les types d'organisations
                await SeedOrganizationTypesAsync();

                // Ajouter un compte administrateur par défaut
                await SeedDefaultAdminAsync();

                // Ajouter des catégories d'articles
                await SeedStoryCategoriesAsync();

                _logger.LogInformation("Initialisation des données terminée avec succès.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation des données.");
                throw; // Propager l'exception pour permettre à l'application de réagir
            }
        }

        /// <summary>
        /// Initialise les types de spots
        /// </summary>
        private async Task SeedSpotTypesAsync()
        {
            var spotTypes = new List<SpotType>
            {
                new SpotType
                {
                    Name = "Plongée",
                    IconPath = "marker_diving.png",
                    ColorCode = "#006994",
                    Category = ActivityCategory.Diving,
                    Description = "Sites de plongée sous-marine",
                    RequiresExpertValidation = true,
                    ValidationCriteria = JsonSerializer.Serialize(new
                    {
                        minDepth = 0,
                        maxDepth = 100,
                        requiresCertification = true,
                        safetyChecks = new[] { "accessibilité", "courant", "visibilité" }
                    }),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new SpotType
                {
                    Name = "Apnée",
                    IconPath = "marker_freediving.png",
                    ColorCode = "#00B4D8",
                    Category = ActivityCategory.Freediving,
                    Description = "Sites de plongée en apnée",
                    RequiresExpertValidation = true,
                    ValidationCriteria = JsonSerializer.Serialize(new
                    {
                        minDepth = 0,
                        maxDepth = 40,
                        requiresCertification = false,
                        safetyChecks = new[] { "accessibilité", "courant", "profondeur" }
                    }),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new SpotType
                {
                    Name = "Randonnée",
                    IconPath = "marker_snorkeling.png",
                    ColorCode = "#48CAE4",
                    Category = ActivityCategory.Snorkeling,
                    Description = "Sites de randonnée palmée",
                    RequiresExpertValidation = false,
                    ValidationCriteria = JsonSerializer.Serialize(new
                    {
                        minDepth = 0,
                        maxDepth = 10,
                        requiresCertification = false,
                        safetyChecks = new[] { "accessibilité", "courant" }
                    }),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.SpotTypes.AddRangeAsync(spotTypes);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Types de spots initialisés avec succès.");
        }

        /// <summary>
        /// Initialise les types d'organisations
        /// </summary>
        private async Task SeedOrganizationTypesAsync()
        {
            var organizationTypes = new List<OrganizationType>
            {
                new OrganizationType
                {
                    Name = "Club FFESSM",
                    Description = "Club associatif affilié à la FFESSM",
                    RequiresAnnualVerification = true,
                    Requirements = JsonSerializer.Serialize(new
                    {
                        affiliationNumber = true,
                        insurance = true,
                        annualReport = true
                    }),
                    VerificationProcess = JsonSerializer.Serialize(new
                    {
                        documentCheck = true,
                        adminApproval = true,
                        annualRenewal = true
                    }),
                    RequiredDocuments = "Attestation d'affiliation FFESSM, Attestation d'assurance"
                },
                new OrganizationType
                {
                    Name = "Base Fédérale",
                    Description = "Structure permanente fédérale",
                    RequiresAnnualVerification = true,
                    Requirements = JsonSerializer.Serialize(new
                    {
                        federalCertification = true,
                        insurance = true,
                        facilities = true
                    }),
                    VerificationProcess = JsonSerializer.Serialize(new
                    {
                        documentCheck = true,
                        adminApproval = true,
                        annualRenewal = true
                    }),
                    RequiredDocuments = "Certificat d'agrément fédéral, Attestation d'assurance, Inventaire des équipements"
                },
                new OrganizationType
                {
                    Name = "Professionnel",
                    Description = "Structure commerciale de plongée",
                    RequiresAnnualVerification = true,
                    Requirements = JsonSerializer.Serialize(new
                    {
                        businessRegistration = true,
                        professionalInsurance = true,
                        certifications = true
                    }),
                    VerificationProcess = JsonSerializer.Serialize(new
                    {
                        documentCheck = true,
                        adminApproval = true,
                        annualRenewal = true
                    }),
                    RequiredDocuments = "SIRET, Assurance RC Pro, Diplômes des moniteurs, Déclaration en préfecture"
                }
            };

            await _context.OrganizationTypes.AddRangeAsync(organizationTypes);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Types d'organisations initialisés avec succès.");
        }

        /// <summary>
        /// Crée un compte administrateur par défaut
        /// </summary>
        private async Task SeedDefaultAdminAsync()
        {
            // Utilisation de BCrypt pour hacher le mot de passe
            // Assurez-vous d'avoir installé le package BCrypt.Net-Next
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");

            var adminUser = new User
            {
                Email = "admin@subexplore.com",
                Username = "admin",
                PasswordHash = passwordHash,
                FirstName = "Admin",
                LastName = "SubExplore",
                AccountType = AccountType.Administrator,
                SubscriptionStatus = SubscriptionStatus.Premium,
                ExpertiseLevel = ExpertiseLevel.Expert,
                Certifications = JsonSerializer.Serialize(new[] { "Admin", "Modérateur" }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            // Ajouter les préférences de l'utilisateur admin
            var userPreferences = new UserPreferences
            {
                User = adminUser,
                Theme = "light",
                DisplayNamePreference = "username",
                NotificationSettings = JsonSerializer.Serialize(new
                {
                    email = true,
                    push = true,
                    spotValidation = true,
                    comments = true
                }),
                Language = "fr",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(adminUser);
            await _context.UserPreferences.AddAsync(userPreferences);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Compte administrateur créé avec succès.");
        }

        /// <summary>
        /// Initialise les catégories d'articles pour le magazine
        /// </summary>
        private async Task SeedStoryCategoriesAsync()
        {
            var categories = new List<StoryCategory>
            {
                new StoryCategory { Name = "Expériences", Description = "Récits d'expériences de plongée" },
                new StoryCategory { Name = "Technique", Description = "Articles techniques sur la plongée" },
                new StoryCategory { Name = "Matériel", Description = "Tests et avis sur le matériel" },
                new StoryCategory { Name = "Biologie", Description = "Découverte de la faune et flore sous-marine" },
                new StoryCategory { Name = "Voyage", Description = "Destinations et voyages de plongée" },
                new StoryCategory { Name = "Environnement", Description = "Protection et préservation des océans" }
            };

            await _context.StoryCategories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Catégories d'articles initialisées avec succès.");
        }
    }
}
