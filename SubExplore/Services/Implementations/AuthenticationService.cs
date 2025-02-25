using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using SubExplore.Services.Interfaces;
using SubExplore.Models;

namespace SubExplore.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cacheService;
        private readonly ISecureStorageService _secureStorage;
        private readonly string _tokenKey = "auth_token";
        private readonly string _refreshTokenKey = "refresh_token";

        public event EventHandler<AuthenticationEventArgs> AuthenticationStateChanged;

        public AuthenticationService(
            IHttpClientFactory httpClientFactory,
            ICacheService cacheService,
            ISecureStorageService secureStorage)
        {
            _httpClient = httpClientFactory.CreateClient("SubExploreAPI");
            _cacheService = cacheService;
            _secureStorage = secureStorage;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
                {
                    email,
                    password
                });

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new AuthenticationException($"Échec de l'authentification: {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
                if (result == null)
                    throw new AuthenticationException("Réponse d'authentification invalide");

                // Stocker les tokens
                await StoreTokensAsync(result.AccessToken, result.RefreshToken);

                // Notifier du changement d'état
                OnAuthenticationStateChanged(new AuthenticationEventArgs
                {
                    IsAuthenticated = true,
                    Username = result.User?.Username
                });

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new AuthenticationException("Erreur de connexion au service d'authentification", ex);
            }
        }

        public async Task<AuthenticationResult> LoginWithOAuthAsync(string provider, string token)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/auth/{provider}", new { token });

                if (!response.IsSuccessStatusCode)
                {
                    throw new AuthenticationException($"Échec de l'authentification {provider}");
                }

                var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
                if (result == null)
                    throw new AuthenticationException("Réponse d'authentification invalide");

                await StoreTokensAsync(result.AccessToken, result.RefreshToken);

                OnAuthenticationStateChanged(new AuthenticationEventArgs
                {
                    IsAuthenticated = true,
                    Username = result.User?.Username
                });

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new AuthenticationException($"Erreur d'authentification {provider}", ex);
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", new { refreshToken });

                if (!response.IsSuccessStatusCode)
                {
                    throw new AuthenticationException("Échec du rafraîchissement du token");
                }

                var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
                if (result == null)
                    throw new AuthenticationException("Réponse de rafraîchissement invalide");

                await StoreTokensAsync(result.AccessToken, result.RefreshToken);
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new AuthenticationException("Erreur lors du rafraîchissement du token", ex);
            }
        }

        public async Task LogoutAsync(int userId)
        {
            try
            {
                // Révoquer le refresh token côté serveur
                var refreshToken = await _secureStorage.GetAsync(_refreshTokenKey);
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await RevokeRefreshTokenAsync(refreshToken);
                }

                // Nettoyer le stockage local
                await _secureStorage.RemoveAsync(_tokenKey);
                await _secureStorage.RemoveAsync(_refreshTokenKey);
                await _cacheService.ClearAsync();

                // Notifier du changement d'état
                OnAuthenticationStateChanged(new AuthenticationEventArgs
                {
                    IsAuthenticated = false
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la déconnexion: {ex.Message}");
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                    return false;

                // Vérifier l'expiration
                if (jsonToken.ValidTo < DateTime.UtcNow)
                    return false;

                // Vérifier avec le serveur
                var response = await _httpClient.PostAsJsonAsync("api/auth/validate", new { token });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            try
            {
                await _httpClient.PostAsJsonAsync("api/auth/revoke", new { token });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la révocation du token: {ex.Message}");
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", new { email });
            if (!response.IsSuccessStatusCode)
                throw new AuthenticationException("Erreur lors de la génération du token de réinitialisation");

            var result = await response.Content.ReadFromJsonAsync<PasswordResetResult>();
            return result?.Token ?? throw new AuthenticationException("Token de réinitialisation invalide");
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password/confirm", new
            {
                token,
                newPassword
            });

            return response.IsSuccessStatusCode;
        }

        public async Task<string> GenerateEmailValidationTokenAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/validate-email", new { email });
            if (!response.IsSuccessStatusCode)
                throw new AuthenticationException("Erreur lors de la génération du token de validation");

            var result = await response.Content.ReadFromJsonAsync<EmailValidationResult>();
            return result?.Token ?? throw new AuthenticationException("Token de validation invalide");
        }

        public async Task<bool> ValidateEmailAsync(string token)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/validate-email/confirm", new { token });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsInRoleAsync(int userId, string role)
        {
            var claims = await GetUserClaimsAsync(userId);
            return claims?.Any(c => c.Type == ClaimTypes.Role && c.Value == role) ?? false;
        }

        public async Task<ClaimsPrincipal> GetClaimsFromTokenAsync(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                    throw new AuthenticationException("Token invalide");

                var claims = new ClaimsPrincipal(new ClaimsIdentity(jsonToken.Claims, "jwt"));
                return claims;
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Erreur lors de l'analyse du token", ex);
            }
        }

        private async Task StoreTokensAsync(string accessToken, string refreshToken)
        {
            await _secureStorage.SetAsync(_tokenKey, accessToken);
            await _secureStorage.SetAsync(_refreshTokenKey, refreshToken);
        }

        private async Task<IEnumerable<Claim>> GetUserClaimsAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}/claims");
            if (!response.IsSuccessStatusCode)
                return null;

            var claims = await response.Content.ReadFromJsonAsync<List<ClaimDto>>();
            return claims?.Select(c => new Claim(c.Type, c.Value));
        }

        protected virtual void OnAuthenticationStateChanged(AuthenticationEventArgs e)
        {
            AuthenticationStateChanged?.Invoke(this, e);
        }
    }

    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class PasswordResetResult
    {
        public string Token { get; set; }
    }

    public class EmailValidationResult
    {
        public string Token { get; set; }
    }

    public class ClaimDto
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
