using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SubExplore.Services.Interfaces;

namespace SubExplore.Services.Implementations
{
    public class ConnectivityService : IConnectivityService
    {
        private readonly ILogger<ConnectivityService> _logger;
        private readonly IConnectivity _connectivity;

        public event EventHandler<SubExplore.Services.Interfaces.ConnectivityChangedEventArgs> ConnectivityChanged;

        public bool IsConnected => _connectivity.NetworkAccess == NetworkAccess.Internet;
        public bool IsWifiConnected => IsConnected && _connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
        public bool IsCellularConnected => IsConnected && _connectivity.ConnectionProfiles.Contains(ConnectionProfile.Cellular);

        public ConnectivityService(IConnectivity connectivity, ILogger<ConnectivityService> logger)
        {
            _connectivity = connectivity;
            _logger = logger;

            _connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        private void OnConnectivityChanged(object sender, Microsoft.Maui.Networking.ConnectivityChangedEventArgs e)
        {
            var connectionType = "None";
            if (_connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
                connectionType = "WiFi";
            else if (_connectivity.ConnectionProfiles.Contains(ConnectionProfile.Cellular))
                connectionType = "Cellular";

            var args = new SubExplore.Services.Interfaces.ConnectivityChangedEventArgs
            {
                IsConnected = IsConnected,
                ConnectionType = connectionType
            };

            ConnectivityChanged?.Invoke(this, args);
            _logger.LogInformation("Connectivity changed: Connected={Connected}, Type={Type}", args.IsConnected, args.ConnectionType);
        }

        public async Task<bool> IsHostReachableAsync(string host, int timeout = 5000)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host, timeout);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de la connectivité vers {Host}", host);
                return false;
            }
        }
    }
}