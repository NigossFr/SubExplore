using System;

namespace SubExplore.Services.Interfaces
{
    public interface IConnectivityService
    {
        /// <summary>
        /// Vérifie si l'appareil est connecté à Internet
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Vérifie si l'appareil est connecté via Wi-Fi
        /// </summary>
        bool IsWifiConnected { get; }

        /// <summary>
        /// Vérifie si l'appareil est connecté via données cellulaires
        /// </summary>
        bool IsCellularConnected { get; }

        /// <summary>
        /// Événement déclenché lors d'un changement de connectivité
        /// </summary>
        event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

        /// <summary>
        /// Vérifie la connectivité vers un hôte spécifique
        /// </summary>
        /// <param name="host">Hôte à vérifier</param>
        /// <param name="timeout">Timeout en millisecondes</param>
        /// <returns>true si l'hôte est accessible</returns>
        Task<bool> IsHostReachableAsync(string host, int timeout = 5000);
    }

    /// <summary>
    /// Arguments pour l'événement de changement de connectivité
    /// </summary>
    public class ConnectivityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// État de la connectivité
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Type de connexion
        /// </summary>
        public string ConnectionType { get; set; } = string.Empty;
    }
}