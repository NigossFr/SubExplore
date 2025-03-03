using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubExplore.Models.DTOs;

namespace SubExplore.Services.Interfaces
{
    /// <summary>
    /// Interface du service de gestion des spots
    /// </summary>
    public interface ISpotService
    {
        #region Gestion des spots

        /// <summary>
        /// Récupère un spot par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du spot</param>
        /// <returns>Le spot ou null</returns>
        Task<Models.DTOs.SpotDto> GetByIdAsync(int id);

        /// <summary>
        /// Recherche des spots par critères
        /// </summary>
        /// <param name="searchParams">Critères de recherche</param>
        /// <returns>Liste des spots correspondants</returns>
        Task<IEnumerable<Models.DTOs.SpotDto>> SearchAsync(Models.DTOs.SpotSearchParameters searchParams);

        /// <summary>
        /// Récupère les spots à proximité d'un point
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="radiusInKm">Rayon de recherche en kilomètres</param>
        /// <param name="maxResults">Nombre maximum de résultats</param>
        /// <returns>Liste des spots à proximité</returns>
        Task<IEnumerable<Models.DTOs.SpotDto>> GetNearbyAsync(
            double latitude,
            double longitude,
            double radiusInKm,
            int maxResults = 50);

        /// <summary>
        /// Récupère les spots filtrés depuis le cache local (mode hors-ligne)
        /// </summary>
        /// <param name="filter">Critères de filtrage</param>
        /// <returns>Liste des spots correspondants</returns>
        Task<IEnumerable<Models.DTOs.SpotDto>> GetCachedSpotsAsync(Models.DTOs.SpotFilter filter);

        /// <summary>
        /// Récupère les spots filtrés (version simplifiée)
        /// </summary>
        /// <param name="filter">Critères de filtrage</param>
        /// <returns>Liste des spots correspondants</returns>
        Task<IEnumerable<Models.DTOs.SpotDto>> GetSpotsAsync(Models.DTOs.SpotFilter filter);

        /// <summary>
        /// Crée un nouveau spot
        /// </summary>
        /// <param name="spotCreation">Données du spot</param>
        /// <param name="userId">ID de l'utilisateur créateur</param>
        /// <returns>Le spot créé</returns>
        Task<Models.DTOs.SpotDto> CreateAsync(Models.DTOs.SpotCreationDto spotCreation, int userId);

        /// <summary>
        /// Met à jour un spot existant
        /// </summary>
        /// <param name="id">ID du spot</param>
        /// <param name="spotUpdate">Nouvelles données</param>
        /// <param name="userId">ID de l'utilisateur effectuant la modification</param>
        /// <returns>Le spot mis à jour</returns>
        Task<Models.DTOs.SpotDto> UpdateAsync(int id, Models.DTOs.SpotUpdateDto spotUpdate, int userId);

        /// <summary>
        /// Supprime un spot
        /// </summary>
        /// <param name="id">ID du spot</param>
        /// <param name="userId">ID de l'utilisateur effectuant la suppression</param>
        Task<bool> DeleteAsync(int id, int userId);

        #endregion

        #region Gestion des médias

        /// <summary>
        /// Ajoute un média à un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="mediaUpload">Fichier média</param>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>Le média créé</returns>
        Task<Models.DTOs.SpotMediaDto> AddMediaAsync(int spotId, Models.DTOs.SpotMediaUploadDto mediaUpload, int userId);

        /// <summary>
        /// Récupère les médias d'un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <returns>Liste des médias</returns>
        Task<IEnumerable<Models.DTOs.SpotMediaDto>> GetMediaAsync(int spotId);

        /// <summary>
        /// Supprime un média
        /// </summary>
        /// <param name="mediaId">ID du média</param>
        /// <param name="userId">ID de l'utilisateur</param>
        Task<bool> DeleteMediaAsync(int mediaId, int userId);

        #endregion

        #region Modération et validation

        /// <summary>
        /// Soumet un spot pour validation
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <returns>true si la soumission est réussie</returns>
        Task<bool> SubmitForValidationAsync(int spotId);

        /// <summary>
        /// Valide un spot par un modérateur
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="moderatorId">ID du modérateur</param>
        /// <param name="validationDetails">Détails de la validation</param>
        /// <returns>true si la validation est réussie</returns>
        Task<bool> ValidateSpotAsync(int spotId, int moderatorId, Models.DTOs.SpotValidationDto validationDetails);

        /// <summary>
        /// Rejette un spot par un modérateur
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="moderatorId">ID du modérateur</param>
        /// <param name="rejectionDetails">Détails du rejet</param>
        /// <returns>true si le rejet est réussi</returns>
        Task<bool> RejectSpotAsync(int spotId, int moderatorId, Models.DTOs.SpotRejectionDto rejectionDetails);

        /// <summary>
        /// Signale un problème sur un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="reportDetails">Détails du signalement</param>
        /// <param name="userId">ID de l'utilisateur signalant</param>
        /// <returns>ID du rapport créé</returns>
        Task<int> ReportSpotAsync(int spotId, Models.DTOs.SpotReportDto reportDetails, int userId);

        #endregion

        #region Statistiques et évaluation

        /// <summary>
        /// Ajoute une évaluation pour un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="rating">Évaluation</param>
        /// <param name="userId">ID de l'utilisateur</param>
        Task<bool> RateSpotAsync(int spotId, Models.DTOs.SpotRatingDto rating, int userId);

        /// <summary>
        /// Récupère la note moyenne d'un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <returns>Note moyenne et nombre d'évaluations</returns>
        Task<Models.DTOs.SpotRatingStatsDto> GetRatingStatsAsync(int spotId);

        /// <summary>
        /// Récupère les statistiques de visite d'un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <returns>Statistiques de visite</returns>
        Task<Models.DTOs.SpotVisitStatsDto> GetVisitStatsAsync(int spotId);

        /// <summary>
        /// Enregistre une visite sur un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="userId">ID de l'utilisateur</param>
        Task LogVisitAsync(int spotId, int userId);

        #endregion

        #region Sécurité et conditions

        /// <summary>
        /// Met à jour les conditions actuelles d'un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="conditions">Nouvelles conditions</param>
        /// <param name="userId">ID de l'utilisateur</param>
        Task UpdateCurrentConditionsAsync(int spotId, Models.DTOs.SpotConditionsDto conditions, int userId);

        /// <summary>
        /// Récupère les conditions actuelles d'un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <returns>Conditions actuelles</returns>
        Task<Models.DTOs.SpotConditionsDto> GetCurrentConditionsAsync(int spotId);

        /// <summary>
        /// Met à jour le statut de sécurité d'un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="securityUpdate">Mise à jour de sécurité</param>
        /// <param name="moderatorId">ID du modérateur</param>
        Task UpdateSecurityStatusAsync(int spotId, Models.DTOs.SpotSecurityUpdateDto securityUpdate, int moderatorId);

        #endregion
    }
}