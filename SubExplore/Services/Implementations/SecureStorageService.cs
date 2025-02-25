using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubExplore.Services.Interfaces;

namespace SubExplore.Services.Implementations
{
    /// <summary>
    /// Implémentation du service de stockage sécurisé utilisant MAUI SecureStorage
    /// </summary>
    public class SecureStorageService : ISecureStorageService
    {
        private readonly Dictionary<string, string> _memoryCache;
        private readonly SemaphoreSlim _semaphore;

        public SecureStorageService()
        {
            _memoryCache = new Dictionary<string, string>();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task SetAsync(string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);

            try
            {
                await _semaphore.WaitAsync();

                // Stocker en mémoire pour un accès rapide
                _memoryCache[key] = value;

                // Stocker dans le stockage sécurisé
                await SecureStorage.Default.SetAsync(key, value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SecureStorageService.SetAsync: {ex.Message}");
                throw new SecureStorageException("Erreur lors du stockage sécurisé", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<string> GetAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            try
            {
                await _semaphore.WaitAsync();

                // Vérifier d'abord le cache mémoire
                if (_memoryCache.TryGetValue(key, out string cachedValue))
                {
                    return cachedValue;
                }

                // Sinon, récupérer depuis le stockage sécurisé
                var value = await SecureStorage.Default.GetAsync(key);

                // Mettre en cache si la valeur existe
                if (value != null)
                {
                    _memoryCache[key] = value;
                }

                return value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SecureStorageService.GetAsync: {ex.Message}");
                throw new SecureStorageException("Erreur lors de la lecture du stockage sécurisé", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            try
            {
                await _semaphore.WaitAsync();

                // Supprimer du cache mémoire
                _memoryCache.Remove(key);

                // Supprimer du stockage sécurisé
                SecureStorage.Default.Remove(key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SecureStorageService.RemoveAsync: {ex.Message}");
                throw new SecureStorageException("Erreur lors de la suppression du stockage sécurisé", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            try
            {
                await _semaphore.WaitAsync();

                // Vérifier d'abord le cache mémoire
                if (_memoryCache.ContainsKey(key))
                {
                    return true;
                }

                // Sinon, vérifier le stockage sécurisé
                var value = await SecureStorage.Default.GetAsync(key);
                return value != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SecureStorageService.ContainsKeyAsync: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                await _semaphore.WaitAsync();

                // Vider le cache mémoire
                _memoryCache.Clear();

                // Vider le stockage sécurisé
                SecureStorage.Default.RemoveAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SecureStorageService.ClearAsync: {ex.Message}");
                throw new SecureStorageException("Erreur lors du nettoyage du stockage sécurisé", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    /// <summary>
    /// Exception spécifique pour les erreurs de stockage sécurisé
    /// </summary>
    public class SecureStorageException : Exception
    {
        public SecureStorageException(string message) : base(message) { }
        public SecureStorageException(string message, Exception innerException) : base(message, innerException) { }
    }
}
