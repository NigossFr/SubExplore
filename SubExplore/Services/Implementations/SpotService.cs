using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using SubExplore.Models;
using SubExplore.Services.Interfaces;

namespace SubExplore.Services.Implementations
{
    public class SpotService : ISpotService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocationService _locationService;
        private readonly IMediaService _mediaService;
        private readonly ICacheService _cacheService;
        private readonly IAuthenticationService _authService;

        public SpotService(
            IHttpClientFactory httpClientFactory,
            ILocationService locationService,
            IMediaService mediaService,
            ICacheService cacheService,
            IAuthenticationService authService)
        {
            _httpClient = httpClientFactory.CreateClient("SubExploreAPI");
            _locationService = locationService;
            _mediaService = mediaService;
            _cacheService = cacheService;
            _authService = authService;
        }

        #region Gestion des spots

        public async Task<SpotDto> GetByIdAsync(int id)
        {
            // Vérifier d'abord le cache
            var cacheKey = $"spot_{id}";
            var cachedSpot = await _cacheService.GetAsync<SpotDto>(cacheKey);
            if (cachedSpot != null)
                return cachedSpot;

            try
            {
                var spot = await _httpClient.GetFromJsonAsync<SpotDto>($"api/spots/{id}");
                if (spot != null)
                {
                    // Mettre en cache pour les futures requêtes
                    await _cacheService.SetAsync(cacheKey, spot, TimeSpan.FromMinutes(15));
                    return spot;
                }
                return null;
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Impossible de récupérer le spot");
            }
        }

        public async Task<IEnumerable<SpotDto>> SearchAsync(SpotSearchParameters searchParams)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
                    queryParams.Add($"search={Uri.EscapeDataString(searchParams.SearchTerm)}");

                if (searchParams.ActivityTypes?.Any() == true)
                    queryParams.Add($"activities={string.Join(",", searchParams.ActivityTypes)}");

                if (searchParams.MinDepth.HasValue)
                    queryParams.Add($"minDepth={searchParams.MinDepth.Value}");

                if (searchParams.MaxDepth.HasValue)
                    queryParams.Add($"maxDepth={searchParams.MaxDepth.Value}");

                var queryString = string.Join("&", queryParams);
                var url = $"api/spots/search?{queryString}";

                var spots = await _httpClient.GetFromJsonAsync<List<SpotDto>>(url);
                return spots ?? new List<SpotDto>();
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la recherche des spots");
            }
        }

        public async Task<IEnumerable<SpotDto>> GetNearbyAsync(double latitude, double longitude, double radiusInKm, int maxResults = 50)
        {
            try
            {
                var url = $"api/spots/nearby?lat={latitude}&lon={longitude}&radius={radiusInKm}&max={maxResults}";
                var spots = await _httpClient.GetFromJsonAsync<List<SpotDto>>(url);
                return spots ?? new List<SpotDto>();
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la récupération des spots à proximité");
            }
        }

        public async Task<SpotDto> CreateAsync(SpotCreationDto spotCreation, int userId)
        {
            try
            {
                // Valider la localisation
                var locationValidation = await _locationService.ValidateMaritimeLocationAsync(
                    new GeoCoordinates
                    {
                        Latitude = spotCreation.Latitude,
                        Longitude = spotCreation.Longitude
                    });

                if (!locationValidation.IsValid)
                    throw new ValidationException("La position n'est pas valide pour un spot sous-marin");

                // Vérifier si le spot n'est pas un doublon
                var nearbySpots = await GetNearbyAsync(
                    spotCreation.Latitude,
                    spotCreation.Longitude,
                    0.1 // 100 mètres
                );

                if (nearbySpots.Any())
                    throw new ValidationException("Un spot existe déjà à proximité");

                var response = await _httpClient.PostAsJsonAsync("api/spots", spotCreation);
                if (!response.IsSuccessStatusCode)
                    throw new ServiceException("Erreur lors de la création du spot");

                var spot = await response.Content.ReadFromJsonAsync<SpotDto>();
                return spot;
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la création du spot");
            }
        }

        public async Task<SpotDto> UpdateAsync(int id, SpotUpdateDto spotUpdate, int userId)
        {
            try
            {
                // Vérifier les permissions
                var spot = await GetByIdAsync(id);
                if (spot == null)
                    throw new NotFoundException("Spot non trouvé");

                if (spot.CreatorId != userId && !await _authService.IsInRoleAsync(userId, "Moderator"))
                    throw new UnauthorizedException("Vous n'avez pas les droits pour modifier ce spot");

                var response = await _httpClient.PutAsJsonAsync($"api/spots/{id}", spotUpdate);
                if (!response.IsSuccessStatusCode)
                    throw new ServiceException("Erreur lors de la mise à jour du spot");

                // Invalider le cache
                await _cacheService.RemoveAsync($"spot_{id}");

                return await response.Content.ReadFromJsonAsync<SpotDto>();
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la mise à jour du spot");
            }
        }

        #endregion

        #region Modération et validation

        public async Task<bool> SubmitForValidationAsync(int spotId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/spots/{spotId}/submit", null);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la soumission pour validation");
            }
        }

        public async Task<bool> ValidateSpotAsync(int spotId, int moderatorId, SpotValidationDto validationDetails)
        {
            try
            {
                // Vérifier que le modérateur a l'expertise requise
                var spot = await GetByIdAsync(spotId);
                if (spot == null)
                    throw new NotFoundException("Spot non trouvé");

                var response = await _httpClient.PostAsJsonAsync(
                    $"api/spots/{spotId}/validate",
                    validationDetails
                );

                if (response.IsSuccessStatusCode)
                {
                    // Invalider le cache
                    await _cacheService.RemoveAsync($"spot_{spotId}");
                    return true;
                }

                return false;
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la validation du spot");
            }
        }

        public async Task<bool> RejectSpotAsync(int spotId, int moderatorId, SpotRejectionDto rejectionDetails)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"api/spots/{spotId}/reject",
                    rejectionDetails
                );

                if (response.IsSuccessStatusCode)
                {
                    // Invalider le cache
                    await _cacheService.RemoveAsync($"spot_{spotId}");
                    return true;
                }

                return false;
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors du rejet du spot");
            }
        }

        public async Task<int> ReportSpotAsync(int spotId, SpotReportDto reportDetails, int userId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"api/spots/{spotId}/reports",
                    reportDetails
                );

                if (!response.IsSuccessStatusCode)
                    throw new ServiceException("Erreur lors du signalement du spot");

                var result = await response.Content.ReadFromJsonAsync<ReportResult>();
                return result.ReportId;
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors du signalement du spot");
            }
        }

        #endregion

        #region Médias

        public async Task<SpotMediaDto> AddMediaAsync(int spotId, SpotMediaUploadDto mediaUpload, int userId)
        {
            // Vérifier le nombre de médias existants
            var existingMedia = await GetMediaAsync(spotId);
            if (existingMedia.Count() >= MediaValidationConfig.MaxPhotosPerSpot)
                throw new ValidationException($"Nombre maximum de photos atteint ({MediaValidationConfig.MaxPhotosPerSpot})");

            // Upload du média
            var uploadResult = await _mediaService.UploadAsync(mediaUpload.File, MediaType.Spot, userId);
            if (!uploadResult.Success)
                throw new ServiceException($"Erreur lors de l'upload : {uploadResult.ErrorMessage}");

            try
            {
                var mediaDto = new SpotMediaDto
                {
                    SpotId = spotId,
                    MediaType = MediaType.Photo,
                    MediaUrl = uploadResult.Urls[MediaVariant.Original],
                    IsPrimary = !existingMedia.Any(),
                    Width = uploadResult.Metadata.Width,
                    Height = uploadResult.Metadata.Height,
                    FileSize = uploadResult.FileSize
                };

                var response = await _httpClient.PostAsJsonAsync($"api/spots/{spotId}/media", mediaDto);
                if (!response.IsSuccessStatusCode)
                {
                    // En cas d'échec, supprimer le média uploadé
                    await _mediaService.DeleteAsync(uploadResult.MediaId, userId);
                    throw new ServiceException("Erreur lors de l'ajout du média");
                }

                return await response.Content.ReadFromJsonAsync<SpotMediaDto>();
            }
            catch (Exception)
            {
                // Nettoyage en cas d'erreur
                await _mediaService.DeleteAsync(uploadResult.MediaId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<SpotMediaDto>> GetMediaAsync(int spotId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<SpotMediaDto>>($"api/spots/{spotId}/media");
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la récupération des médias");
            }
        }

        public async Task<bool> DeleteMediaAsync(int mediaId, int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/media/{mediaId}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                throw new ServiceException("Erreur lors de la suppression du média");
            }
        }

        #endregion
    }

    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class ReportResult
    {
        public int ReportId { get; set; }
    }
}
