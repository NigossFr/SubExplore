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
using System.Threading;
// Utilisez un alias pour résoudre l'ambiguïté avec AuthenticationException
using AuthException = System.Security.Authentication.AuthenticationException;
using SubExplore.Models.Auth;

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

        // Implémentation correcte de RegisterAsync qui correspond à l'interface
        public async Task<bool> RegisterAsync(Models.Auth.RegistrationRequest userCreation)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", userCreation);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                throw new AuthException("Erreur lors de l'inscription");
            }
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
                {
                    email,
                    password
                }, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new AuthException($"Échec de l'authentification: {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>(cancellationToken: cancellationToken);
                if (result == null)
                    throw new AuthException("Réponse d'authentification invalide");

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
                throw new AuthException("Erreur de connexion au service d'authentification", ex);
            }
        }

        public async Task<AuthenticationResult> LoginWithOAuthAsync(string provider, string token, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/auth/{provider}", new { token }, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new AuthException($"Échec de l'authentification {provider}");
                }

                var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>(cancellationToken: cancellationToken);
                if (result == null)
                    throw new AuthException("Réponse d'authentification invalide");

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
                throw new AuthException($"Erreur d'authentification {provider}", ex);
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", new { refreshToken });

                if (!response.IsSuccessStatusCode)
                {
                    throw new AuthException("Échec du rafraîchissement du token");
                }

                var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
                if (result == null)
                    throw new AuthException("Réponse de rafraîchissement invalide");

                await StoreTokensAsync(result.AccessToken, result.RefreshToken);
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new AuthException("Erreur lors du rafraîchissement du token", ex);
            }
        }

        public async Task LogoutAsync()
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
                throw new AuthException("Erreur lors de la génération du token de réinitialisation");

            var result = await response.Content.ReadFromJsonAsync<PasswordResetResult>();
            return result?.Token ?? throw new AuthException("Token de réinitialisation invalide");
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
                throw new AuthException("Erreur lors de la génération du token de validation");

            var result = await response.Content.ReadFromJsonAsync<EmailValidationResult>();
            return result?.Token ?? throw new AuthException("Token de validation invalide");
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
                    throw new AuthException("Token invalide");

                var claims = new ClaimsPrincipal(new ClaimsIdentity(jsonToken.Claims, "jwt"));
                return claims;
            }
            catch (Exception ex)
            {
                throw new AuthException("Erreur lors de l'analyse du token", ex);
            }
        }

        public async Task<UserBasicInfo?> GetCurrentUserAsync()
        {
            try
            {
                var token = await _secureStorage.GetAsync(_tokenKey);
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                // Vérifier si le token est valide
                if (!await ValidateTokenAsync(token))
                {
                    var refreshToken = await _secureStorage.GetAsync(_refreshTokenKey);
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        return null;
                    }

                    try
                    {
                        var result = await RefreshTokenAsync(refreshToken);
                        token = result.AccessToken;
                    }
                    catch
                    {
                        return null;
                    }
                }

                // Récupérer l'utilisateur courant
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("api/auth/me");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserBasicInfo>();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/auth/check-email?email={Uri.EscapeDataString(email)}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/auth/email-available?email={Uri.EscapeDataString(email)}");
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/auth/username-available?username={Uri.EscapeDataString(username)}");
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AreSocialProvidersAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/auth/social-providers");
                if (response.IsSuccessStatusCode)
                {
                    var providers = await response.Content.ReadFromJsonAsync<List<string>>();
                    return providers != null && providers.Any();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task StoreTokensAsync(string accessToken, string refreshToken)
        {
            await _secureStorage.SetAsync(_tokenKey, accessToken);
            await _secureStorage.SetAsync(_refreshTokenKey, refreshToken);
        }

        private async Task<IEnumerable<Claim>?> GetUserClaimsAsync(int userId)
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

    public class ClaimDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class PasswordResetResult
    {
        public string Token { get; set; } = string.Empty;
    }

    public class EmailValidationResult
    {
        public string Token { get; set; } = string.Empty;
    }
}