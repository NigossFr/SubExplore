using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices.Sensors;
using SubExplore.Services.Interfaces;
using System.Net.Http.Json;

namespace SubExplore.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly IGeolocation _geolocation;
        private readonly IConnectivity _connectivity;
        private readonly ICacheService _cacheService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationService> _logger;

        public LocationService(
            IGeolocation geolocation,
            IConnectivity connectivity,
            ICacheService cacheService,
            IHttpClientFactory httpClientFactory,
            ILogger<LocationService> logger)
        {
            _geolocation = geolocation;
            _connectivity = connectivity;
            _cacheService = cacheService;
            _httpClient = httpClientFactory.CreateClient("SubExploreAPI");
            _logger = logger;
        }

        public async Task<GeoCoordinates> GeocodeAddressAsync(string address)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();

                if (location != null)
                {
                    return new GeoCoordinates
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Accuracy = null // MAUI Geocoding ne fournit pas la précision
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du géocodage de l'adresse: {Address}", address);
                throw new LocationException("Erreur lors du géocodage de l'adresse", ex);
            }
        }

        public async Task<LocationAddress> ReverseGeocodeAsync(double latitude, double longitude)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark == null) return null;

                return new LocationAddress
                {
                    FormattedAddress = $"{placemark.SubThoroughfare} {placemark.Thoroughfare}, {placemark.Locality}",
                    City = placemark.Locality,
                    Region = placemark.AdminArea,
                    Country = placemark.CountryName,
                    PostalCode = placemark.PostalCode,
                    ExtraComponents = new Dictionary<string, string>
                    {
                        { "SubLocality", placemark.SubLocality },
                        { "AdminArea", placemark.AdminArea },
                        { "SubAdminArea", placemark.SubAdminArea }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du géocodage inverse: {Lat}, {Lon}", latitude, longitude);
                throw new LocationException("Erreur lors du géocodage inverse", ex);
            }
        }

        public async Task<double> CalculateDistanceAsync(GeoCoordinates point1, GeoCoordinates point2)
        {
            try
            {
                var distance = Location.CalculateDistance(
                    point1.Latitude, point1.Longitude,
                    point2.Latitude, point2.Longitude,
                    DistanceUnits.Kilometers);

                return distance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du calcul de distance");
                throw new LocationException("Erreur lors du calcul de distance", ex);
            }
        }

        public async Task<IEnumerable<LocationWithDistance>> FindInRadiusAsync(
            GeoCoordinates center,
            double radiusKm,
            LocationType? type = null)
        {
            try
            {
                // Construire l'URL de l'API avec les paramètres
                var url = $"api/locations/nearby?lat={center.Latitude}&lon={center.Longitude}&radius={radiusKm}";
                if (type.HasValue)
                    url += $"&type={type.Value}";

                var response = await _httpClient.GetFromJsonAsync<List<LocationWithDistance>>(url);
                return response ?? new List<LocationWithDistance>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche des points d'intérêt");
                throw new LocationException("Erreur lors de la recherche des points d'intérêt", ex);
            }
        }

        public async Task<IEnumerable<ClusteredLocation>> ClusterLocationsAsync(
            IEnumerable<GeoLocation> points,
            int zoomLevel)
        {
            // Implémentation de l'algorithme de clustering
            try
            {
                // Logique de clustering basique - à améliorer selon vos besoins
                var clusters = new List<ClusteredLocation>();
                var gridSize = 0.1 / Math.Pow(2, zoomLevel); // Ajuster la taille de la grille selon le zoom

                var grouped = points.GroupBy(p => (
                    lat: Math.Floor(p.Coordinates.Latitude / gridSize) * gridSize,
                    lon: Math.Floor(p.Coordinates.Longitude / gridSize) * gridSize
                ));

                foreach (var group in grouped)
                {
                    var pointsList = group.ToList();
                    clusters.Add(new ClusteredLocation
                    {
                        Center = new GeoCoordinates
                        {
                            Latitude = pointsList.Average(p => p.Coordinates.Latitude),
                            Longitude = pointsList.Average(p => p.Coordinates.Longitude)
                        },
                        PointCount = pointsList.Count,
                        Points = pointsList,
                        TypeCounts = pointsList
                            .GroupBy(p => p.Type)
                            .ToDictionary(g => g.Key, g => g.Count())
                    });
                }

                return clusters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du clustering des locations");
                throw new LocationException("Erreur lors du clustering des locations", ex);
            }
        }

        public async Task<double?> GetApproximateDepthAsync(GeoCoordinates coordinates)
        {
            try
            {
                var cacheKey = $"depth_{coordinates.Latitude}_{coordinates.Longitude}";
                var cachedDepth = await _cacheService.GetAsync<double?>(cacheKey);

                if (cachedDepth.HasValue)
                    return cachedDepth;

                var response = await _httpClient.GetFromJsonAsync<double?>($"api/marine/depth?lat={coordinates.Latitude}&lon={coordinates.Longitude}");

                if (response.HasValue)
                    await _cacheService.SetAsync(cacheKey, response.Value, TimeSpan.FromDays(30));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la profondeur");
                return null;
            }
        }

        public async Task<ProtectedAreaInfo> CheckProtectedAreaAsync(GeoCoordinates coordinates)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProtectedAreaInfo>(
                    $"api/marine/protected-areas?lat={coordinates.Latitude}&lon={coordinates.Longitude}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de la zone protégée");
                return new ProtectedAreaInfo { IsProtected = false };
            }
        }

        public async Task<IEnumerable<MooringPoint>> GetNearbyMooringPointsAsync(
            GeoCoordinates coordinates,
            double radiusKm)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<MooringPoint>>(
                    $"api/marine/mooring-points?lat={coordinates.Latitude}&lon={coordinates.Longitude}&radius={radiusKm}");
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
                var data = await _httpClient.GetFromJsonAsync<object>(
                    $"api/locations/preload?neLat={boundingBox.NorthEast.Latitude}&neLon={boundingBox.NorthEast.Longitude}" +
                    $"&swLat={boundingBox.SouthWest.Latitude}&swLon={boundingBox.SouthWest.Longitude}");

                // Stocker les données dans le cache
                var cacheKey = $"location_data_{boundingBox.NorthEast.Latitude}_{boundingBox.NorthEast.Longitude}_" +
                             $"{boundingBox.SouthWest.Latitude}_{boundingBox.SouthWest.Longitude}";

                await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
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
                var cacheKey = $"location_data_{boundingBox.NorthEast.Latitude}_{boundingBox.NorthEast.Longitude}_" +
                             $"{boundingBox.SouthWest.Latitude}_{boundingBox.SouthWest.Longitude}";

                await _cacheService.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'invalidation du cache de localisation");
            }
        }

        public async Task<MaritimeValidationResult> ValidateMaritimeLocationAsync(GeoCoordinates coordinates)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MaritimeValidationResult>(
                    $"api/marine/validate?lat={coordinates.Latitude}&lon={coordinates.Longitude}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la validation maritime");
                return new MaritimeValidationResult
                {
                    IsValid = false,
                    ValidationMessage = "Erreur lors de la validation"
                };
            }
        }

        public async Task<AccessibilityInfo> CheckLocationAccessibilityAsync(GeoCoordinates coordinates)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<AccessibilityInfo>(
                    $"api/locations/accessibility?lat={coordinates.Latitude}&lon={coordinates.Longitude}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de l'accessibilité");
                return new AccessibilityInfo
                {
                    IsAccessible = false,
                    AccessType = "unknown"
                };
            }
        }
    }

    public class LocationException : Exception
    {
        public LocationException(string message) : base(message) { }
        public LocationException(string message, Exception innerException) : base(message, innerException) { }
    }
}