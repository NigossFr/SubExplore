using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Services.Interfaces
{
    /// <summary>
    /// Interface définissant les opérations de cache
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Récupère une valeur du cache
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Stocke une valeur dans le cache
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Supprime une valeur du cache
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Vérifie si une clé existe dans le cache
        /// </summary>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Vide le cache
        /// </summary>
        Task ClearAsync();
    }
}
