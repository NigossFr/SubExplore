using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices.Sensors;
using SubExplore.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubExplore.Models;

namespace SubExplore.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly IGeolocation _geolocation;
        private readonly ICacheService _cacheService;
        private readonly ILogger<LocationService> _logger;
        private readonly HttpClient _httpClient;

        public LocationService(
            IGeolocation geolocation,
            ICacheService cacheService,
            ILogger<LocationService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _geolocation = geolocation;
            _cacheService = cacheService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("SubExploreAPI");
        }

        public async Task<SubExplore.Models.GeoCoordinates> GeocodeAddressAsync(string address)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();

                if (location == null)
                    throw new Exception("Aucune localisation trouvée");

                return new SubExplore.Models.GeoCoordinates
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Accuracy = location.Accuracy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du géocodage de l'adresse: {Address}", address);
                throw;
            }
        }

        public async Task<LocationAddress> ReverseGeocodeAsync(double latitude, double longitude)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark == null)
                    throw new Exception("Aucune adresse trouvée");

                return new LocationAddress
                {
                    FormattedAddress = $"{placemark.SubThoroughfare ?? ""} {placemark.Thoroughfare ?? ""}, {placemark.Locality ?? ""}".Trim(),
                    City = placemark.Locality ?? string.Empty,
                    Region = placemark.AdminArea ?? string.Empty,
                    Country = placemark.CountryName ?? string.Empty,
                    PostalCode = placemark.PostalCode ?? string.Empty,
                    ExtraComponents = new Dictionary<string, string>
                    {
                        { "SubLocality", placemark.SubLocality ?? string.Empty },
                        { "AdminArea", placemark.AdminArea ?? string.Empty },
                        { "SubAdminArea", placemark.SubAdminArea ?? string.Empty }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du géocodage inverse: {Lat}, {Lon}", latitude, longitude);
                throw;
            }
        }

        public Task<double> CalculateDistanceAsync(SubExplore.Models.GeoCoordinates point1, GeoCoordinates point2)
        {
            var distance = Location.CalculateDistance(
                point1.Latitude, point1.Longitude,
                point2.Latitude, point2.Longitude,
                DistanceUnits.Kilometers);

            return Task.FromResult(distance);
        }

        public async Task<IEnumerable<LocationWithDistance>> FindInRadiusAsync(
            SubExplore.Models.GeoCoordinates center,
            double radiusKm,
            LocationType? type = null)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<LocationWithDistance>>(
                    $"api/locations/nearby?lat={center.Latitude}&lon={center.Longitude}&radius={radiusKm}&type={type}");

                return response ?? new List<LocationWithDistance>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche de points d'intérêt");
                return Enumerable.Empty<LocationWithDistance>();
            }
        }

        public async Task<IEnumerable<ClusteredLocation>> ClusterLocationsAsync(
    IEnumerable<GeoCoordinates> points,
    int zoomLevel)
        {
            try
            {
                // Convertir points en format approprié pour l'API si nécessaire
                var pointsData = points.Select(p => new
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                });

                var response = await _httpClient.PostAsJsonAsync(
                    "api/locations/cluster",
                    new { Points = pointsData, ZoomLevel = zoomLevel });

                return response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<List<ClusteredLocation>>()
                    : new List<ClusteredLocation>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du clustering des locations");
                return Enumerable.Empty<ClusteredLocation>();
            }
        }

        public async Task<double?> GetApproximateDepthAsync(SubExplore.Models.GeoCoordinates coordinates)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<double?>(
                    $"api/marine/depth?lat={coordinates.Latitude}&lon={coordinates.Longitude}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la profondeur");
                return null;
            }
        }

        public async Task<ProtectedAreaInfo> CheckProtectedAreaAsync(SubExplore.Models.GeoCoordinates coordinates)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ProtectedAreaInfo>(
                    $"api/marine/protected-areas?lat={coordinates.Latitude}&lon={coordinates.Longitude}");

                return response ?? new ProtectedAreaInfo { IsProtected = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification des zones protégées");
                return new ProtectedAreaInfo { IsProtected = false };
            }
        }

        public async Task<IEnumerable<MooringPoint>> GetNearbyMooringPointsAsync(
            SubExplore.Models.GeoCoordinates coordinates,
            double radiusKm)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<MooringPoint>>(
                    $"api/marine/mooring-points?lat={coordinates.Latitude}&lon={coordinates.Longitude}&radius={radiusKm}");

                return response ?? Enumerable.Empty<MooringPoint>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des points d'amarrage");
                return Enumerable.Empty<MooringPoint>();
            }
        }

        public async Task PreloadLocationDataAsync(BoundingBox boundingBox)
        {
            try
            {
                await _httpClient.PostAsJsonAsync("api/locations/preload", boundingBox);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du préchargement des données de localisation");
            }
        }

        public async Task InvalidateLocationCacheAsync(BoundingBox boundingBox)
        {
            try
            {
                await _httpClient.PostAsJsonAsync("api/locations/invalidate-cache", boundingBox);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'invalidation du cache de localisation");
            }
        }

        public async Task<MaritimeValidationResult> ValidateMaritimeLocationAsync(SubExplore.Models.GeoCoordinates coordinates)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<MaritimeValidationResult>(
                    $"api/marine/validate?lat={coordinates.Latitude}&lon={coordinates.Longitude}");

                return response ?? new MaritimeValidationResult { IsValid = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la validation maritime");
                return new MaritimeValidationResult { IsValid = false };
            }
        }

        public async Task<AccessibilityInfo> CheckLocationAccessibilityAsync(SubExplore.Models.GeoCoordinates coordinates)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AccessibilityInfo>(
                    $"api/locations/accessibility?lat={coordinates.Latitude}&lon={coordinates.Longitude}");

                return response ?? new AccessibilityInfo { IsAccessible = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de l'accessibilité");
                return new AccessibilityInfo { IsAccessible = false };
            }
        }
    }
}