using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using SubExplore.Services.Interfaces;
using SubExplore.ViewModels.Base;

namespace SubExplore.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        private readonly ILogger<NavigationService> _logger;
        private readonly List<NavigationHistoryEntry> _history;
        private NavigationOptions _options;
        private Action<NavigationErrorEventArgs> _errorHandler;

        public event EventHandler<NavigationErrorEventArgs> NavigationFailed;

        public IReadOnlyList<NavigationHistoryEntry> NavigationHistory =>
            new ReadOnlyCollection<NavigationHistoryEntry>(_history);

        public bool CanGoBack => Shell.Current.Navigation.NavigationStack.Count > 1;

        public NavigationService(ILogger<NavigationService> logger)
        {
            _logger = logger;
            _history = new List<NavigationHistoryEntry>();
            _options = new NavigationOptions();
        }

        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters = null)
        {
            try
            {
                var navigationParameters = parameters != null ?
                    new NavigationParameters(parameters) : null;

                var queryString = BuildQueryString(parameters);
                var fullRoute = string.IsNullOrEmpty(queryString) ? route : $"{route}?{queryString}";

                await Shell.Current.GoToAsync(fullRoute);

                RecordNavigation(route, navigationParameters);
            }
            catch (Exception ex)
            {
                HandleNavigationError(ex, route, parameters);
            }
        }

        public async Task NavigateToAsync<TViewModel>(INavigationParameters parameters = null)
            where TViewModel : ViewModelBase
        {
            var route = GetRouteForViewModel<TViewModel>();
            if (string.IsNullOrEmpty(route))
            {
                throw new NavigationException($"Aucune route trouvée pour {typeof(TViewModel).Name}");
            }

            var paramDict = parameters?.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            );

            await NavigateToAsync(route, paramDict);
        }

        public async Task GoBackAsync(INavigationParameters parameters = null)
        {
            try
            {
                if (CanGoBack)
                {
                    var queryString = BuildQueryString(parameters?.ToDictionary(k => k.Key, v => v.Value));
                    await Shell.Current.GoToAsync(".." + (string.IsNullOrEmpty(queryString) ? "" : $"?{queryString}"));

                    if (_history.Any())
                        _history.RemoveAt(_history.Count - 1);
                }
            }
            catch (Exception ex)
            {
                HandleNavigationError(ex, "back", parameters?.ToDictionary(k => k.Key, v => v.Value));
            }
        }

        public async Task GoToRootAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("//");
                _history.Clear();
            }
            catch (Exception ex)
            {
                HandleNavigationError(ex, "root", null);
            }
        }

        public async Task ShowModalAsync<TViewModel>(INavigationParameters parameters = null)
            where TViewModel : ViewModelBase
        {
            var route = GetRouteForViewModel<TViewModel>();
            if (string.IsNullOrEmpty(route))
            {
                throw new NavigationException($"Aucune route trouvée pour {typeof(TViewModel).Name}");
            }

            await NavigateToAsync($"//{route}", parameters?.ToDictionary(k => k.Key, v => v.Value));
        }

        public async Task CloseModalAsync(object result = null)
        {
            try
            {
                await Shell.Current.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                HandleNavigationError(ex, "close-modal", null);
            }
        }

        public async Task NavigateToSpotDetailsAsync(int spotId, string highlightFeature = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", spotId }
            };

            if (!string.IsNullOrEmpty(highlightFeature))
            {
                parameters.Add("highlight", highlightFeature);
            }

            await NavigateToAsync("spot-details", parameters);
        }

        public async Task NavigateToSpotCreationAsync(GeoCoordinates initialCoordinates = null)
        {
            var parameters = initialCoordinates != null ? new Dictionary<string, object>
            {
                { "latitude", initialCoordinates.Latitude },
                { "longitude", initialCoordinates.Longitude }
            } : null;

            await NavigateToAsync("add-spot", parameters);
        }

        public async Task NavigateToUserProfileAsync(int userId, string section = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", userId }
            };

            if (!string.IsNullOrEmpty(section))
            {
                parameters.Add("section", section);
            }

            await NavigateToAsync("profile", parameters);
        }

        public async Task NavigateToOrganizationAsync(int organizationId)
        {
            await NavigateToAsync("structure-details", new Dictionary<string, object>
            {
                { "id", organizationId }
            });
        }

        public async Task<string> SaveNavigationStateAsync()
        {
            var state = new NavigationState
            {
                History = _history.ToList(),
                Timestamp = DateTime.UtcNow
            };

            var stateKey = Guid.NewGuid().ToString();
            // Vous devrez implémenter le stockage réel de l'état
            return stateKey;
        }

        public async Task RestoreNavigationStateAsync(string stateKey)
        {
            // Vous devrez implémenter la restauration réelle de l'état
        }

        public void ConfigureRoutes(IDictionary<string, Type> routes)
        {
            foreach (var route in routes)
            {
                Routing.RegisterRoute(route.Key, route.Value);
            }
        }

        public void ConfigureNavigation(NavigationOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void ConfigureTimeout(TimeSpan timeout)
        {
            _options.NavigationTimeout = timeout;
        }

        public void ConfigureErrorHandling(Action<NavigationErrorEventArgs> errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public async Task HandleDeepLinkAsync(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return;

            try
            {
                await Shell.Current.GoToAsync(uri);
            }
            catch (Exception ex)
            {
                HandleNavigationError(ex, uri, null);
            }
        }

        protected virtual void HandleNavigationError(Exception exception, string targetRoute, IDictionary<string, object> parameters)
        {
            _logger.LogError(exception, "Navigation error to {Route}", targetRoute);

            var args = new NavigationErrorEventArgs
            {
                Exception = exception,
                TargetRoute = targetRoute,
                Parameters = parameters
            };

            _errorHandler?.Invoke(args);

            if (!args.Handled)
            {
                NavigationFailed?.Invoke(this, args);
            }
        }

        private string BuildQueryString(IDictionary<string, object> parameters)
        {
            if (parameters == null || !parameters.Any())
                return string.Empty;

            var queryParams = parameters.Select(p =>
                $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value?.ToString() ?? "")}");

            return string.Join("&", queryParams);
        }

        private void RecordNavigation(string route, INavigationParameters parameters)
        {
            var entry = new NavigationHistoryEntry
            {
                Route = route,
                Parameters = parameters,
                Timestamp = DateTime.UtcNow
            };

            _history.Add(entry);

            if (_history.Count > _options.MaxHistoryEntries)
            {
                _history.RemoveAt(0);
            }
        }

        private string GetRouteForViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            // Vous pouvez implémenter une logique personnalisée pour mapper les ViewModels aux routes
            // Pour l'instant, on utilise une convention simple
            var viewModelName = typeof(TViewModel).Name;
            if (viewModelName.EndsWith("ViewModel"))
            {
                viewModelName = viewModelName.Substring(0, viewModelName.Length - "ViewModel".Length);
            }
            return viewModelName.ToLower();
        }
    }

    public class NavigationParameters : Dictionary<string, object>, INavigationParameters
    {
        public NavigationParameters() : base() { }

        public NavigationParameters(IDictionary<string, object> dictionary)
            : base(dictionary ?? new Dictionary<string, object>()) { }

        public T GetValue<T>(string key)
        {
            if (TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default;
                }
            }
            return default;
        }

        public void Add<T>(string key, T value)
        {
            base[key] = value;
        }
    }

    public class NavigationState
    {
        public List<NavigationHistoryEntry> History { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class NavigationException : Exception
    {
        public NavigationException(string message) : base(message) { }
        public NavigationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}