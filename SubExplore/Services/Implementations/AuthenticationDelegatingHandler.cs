using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubExplore.Services.Interfaces;

namespace SubExplore.Services.Implementations
{
    /// <summary>
    /// Handler HTTP qui ajoute automatiquement le token d'authentification aux requêtes
    /// </summary>
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly ISecureStorageService _secureStorage;
        private readonly IAuthenticationService _authService;
        private const string _tokenKey = "auth_token";
        private const string _refreshTokenKey = "refresh_token";

        public AuthenticationDelegatingHandler(
            ISecureStorageService secureStorage,
            IAuthenticationService authService)
        {
            _secureStorage = secureStorage;
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Ne pas ajouter le token pour les requêtes d'authentification
            if (request.RequestUri?.AbsolutePath.StartsWith("/api/auth") ?? false)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var token = await _secureStorage.GetAsync(_tokenKey);

            // Si pas de token, continuer sans authentification
            if (string.IsNullOrEmpty(token))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            // Vérifier si le token est valide
            if (!await _authService.ValidateTokenAsync(token))
            {
                // Essayer de rafraîchir le token
                var refreshToken = await _secureStorage.GetAsync(_refreshTokenKey);
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    try
                    {
                        var result = await _authService.RefreshTokenAsync(refreshToken);
                        token = result.AccessToken;
                    }
                    catch
                    {
                        // Si le rafraîchissement échoue, continuer sans token
                        return await base.SendAsync(request, cancellationToken);
                    }
                }
            }

            // Ajouter le token à la requête
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Envoyer la requête
            var response = await base.SendAsync(request, cancellationToken);

            // Si unauthorized (401), essayer de rafraîchir le token et réessayer une fois
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshToken = await _secureStorage.GetAsync(_refreshTokenKey);
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    try
                    {
                        var result = await _authService.RefreshTokenAsync(refreshToken);

                        // Créer une nouvelle requête avec le nouveau token
                        var newRequest = await CloneHttpRequestMessageAsync(request);
                        newRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

                        return await base.SendAsync(newRequest, cancellationToken);
                    }
                    catch
                    {
                        // Si le rafraîchissement échoue, retourner la réponse unauthorized originale
                        return response;
                    }
                }
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);

            // Copier les headers
            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            // Copier le contenu si présent
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                clone.Content = new StringContent(content, System.Text.Encoding.UTF8, request.Content.Headers.ContentType?.MediaType);
            }

            return clone;
        }
    }
}
