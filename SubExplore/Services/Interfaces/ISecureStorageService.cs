using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Services.Interfaces
{
    /// <summary>
    /// Interface pour le stockage sécurisé des données sensibles
    /// </summary>
    public interface ISecureStorageService
    {
        /// <summary>
        /// Stocke une valeur de manière sécurisée
        /// </summary>
        /// <param name="key">Clé d'accès</param>
        /// <param name="value">Valeur à stocker</param>
        Task SetAsync(string key, string value);

        /// <summary>
        /// Récupère une valeur stockée
        /// </summary>
        /// <param name="key">Clé d'accès</param>
        /// <returns>La valeur stockée ou null si non trouvée</returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// Supprime une valeur stockée
        /// </summary>
        /// <param name="key">Clé d'accès</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Vérifie si une clé existe
        /// </summary>
        /// <param name="key">Clé à vérifier</param>
        Task<bool> ContainsKeyAsync(string key);

        /// <summary>
        /// Supprime toutes les valeurs stockées
        /// </summary>
        Task ClearAsync();
    }
}
