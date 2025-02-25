using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using SubExplore.Services.Implementations;
using SubExplore.Services.Interfaces;

namespace SubExplore.Services.Extensions
{
    /// <summary>
    /// Extensions pour la configuration des services
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configure le service de cache
        /// </summary>
        public static IServiceCollection AddCacheService(this IServiceCollection services)
        {
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024; // 1GB
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
            });
            services.AddSingleton<ICacheService, MemoryCacheService>();
            return services;
        }

        /// <summary>
        /// Configure le service de stockage sécurisé
        /// </summary>
        public static IServiceCollection AddSecureStorage(this IServiceCollection services)
        {
            services.AddSingleton<ISecureStorageService, SecureStorageService>();
            return services;
        }

        // Autres extensions de configuration des services peuvent être ajoutées ici
    }
}
