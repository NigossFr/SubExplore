using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SubExplore.Models;
using SubExplore.Models.Media;

namespace SubExplore.Services.Interfaces
{
    public enum MediaServiceType
    {
        Spot,
        Story,
        Profile,
        Organization
    }

    /// <summary>
    /// Interface du service de gestion des médias
    /// </summary>
    public interface IMediaService
    {
        #region Upload et Stockage

        /// <summary>
        /// Upload un fichier média
        /// </summary>
        /// <param name="file">Fichier à uploader</param>
        /// <param name="mediaType">Type de média (Spot, Story, Profile...)</param>
        /// <param name="ownerId">ID du propriétaire</param>
        /// <returns>Informations sur le média uploadé</returns>
        Task<MediaUploadResult> UploadAsync(Models.Media.IMediaFile file, MediaServiceType mediaType, int ownerId);

        /// <summary>
        /// Upload multiple fichiers médias
        /// </summary>
        /// <param name="files">Collection de fichiers</param>
        /// <param name="mediaType">Type de média</param>
        /// <param name="ownerId">ID du propriétaire</param>
        /// <returns>Liste des résultats d'upload</returns>
        Task<IEnumerable<MediaUploadResult>> UploadMultipleAsync(IEnumerable<Models.Media.IMediaFile> files, MediaServiceType mediaType, int ownerId);

        /// <summary>
        /// Supprime un média
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <param name="userId">ID de l'utilisateur effectuant la suppression</param>
        Task<bool> DeleteAsync(string mediaId, int userId);

        /// <summary>
        /// Déplace un média d'une location temporaire vers permanente
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        Task<bool> MoveToPermanentStorageAsync(string mediaId);

        #endregion

        #region Récupération et Gestion

        /// <summary>
        /// Récupère l'URL d'accès à un média
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <param name="variant">Variante souhaitée (thumbnail, medium, original)</param>
        /// <returns>URL d'accès</returns>
        Task<string> GetUrlAsync(string mediaId, MediaVariant variant = MediaVariant.Original);

        /// <summary>
        /// Récupère les métadonnées d'un média
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <returns>Métadonnées du média</returns>
        Task<MediaMetadata> GetMetadataAsync(string mediaId);

        /// <summary>
        /// Vérifie si un média existe
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <returns>true si le média existe</returns>
        Task<bool> ExistsAsync(string mediaId);

        /// <summary>
        /// Met à jour les métadonnées d'un média
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <param name="metadata">Nouvelles métadonnées</param>
        Task UpdateMetadataAsync(string mediaId, MediaMetadataUpdateDto metadata);

        #endregion

        #region Traitement d'Image

        /// <summary>
        /// Génère les différentes variantes d'une image
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <returns>URLs des variantes générées</returns>
        Task<Dictionary<MediaVariant, string>> GenerateVariantsAsync(string mediaId);

        /// <summary>
        /// Optimise une image pour le web
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <returns>URL de l'image optimisée</returns>
        Task<string> OptimizeForWebAsync(string mediaId);

        /// <summary>
        /// Vérifie si une image est appropriée (pas de contenu inapproprié)
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <returns>Résultat de la modération</returns>
        Task<ImageModerationResult> ModerateImageAsync(string mediaId);

        #endregion

        #region Validation et Sécurité

        /// <summary>
        /// Valide un fichier avant upload
        /// </summary>
        /// <param name="file">Fichier à valider</param>
        /// <returns>Résultat de la validation</returns>
        Task<MediaValidationResult> ValidateFileAsync(Models.Media.IMediaFile file);

        /// <summary>
        /// Vérifie les permissions d'accès à un média
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>true si l'accès est autorisé</returns>
        Task<bool> CheckAccessPermissionAsync(string mediaId, int userId);

        /// <summary>
        /// Nettoie les fichiers temporaires expirés
        /// </summary>
        Task CleanupTempFilesAsync();

        #endregion
    }

    public enum MediaType
    {
        Spot,
        Story,
        Profile,
        Organization
    }

    public enum MediaVariant
    {
        Original,
        Thumbnail,
        Medium,
        Large,
        Web
    }

    public class MediaUploadResult
    {
        public string MediaId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public Dictionary<MediaVariant, string> Urls { get; set; }
        public MediaMetadata Metadata { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class MediaMetadata
    {
        public string MediaId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime UploadedAt { get; set; }
        public MediaServiceType MediaType { get; set; } // Utiliser le nouveau type
        public int OwnerId { get; set; }
        public Dictionary<string, string> ExtraMetadata { get; set; }
    }

    public class ImageModerationResult
    {
        public bool IsApproved { get; set; }
        public List<string> DetectedIssues { get; set; }
        public double ConfidenceScore { get; set; }
    }

    public class MediaValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; }
        public Dictionary<string, string> ValidationDetails { get; set; }
    }

    // Classe DTO pour la mise à jour des métadonnées
    public class MediaMetadataUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> CustomMetadata { get; set; }
    }
}