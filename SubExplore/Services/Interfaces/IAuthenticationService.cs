﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SubExplore.Services.Interfaces
{
    /// <summary>
    /// Interface du service d'authentification
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Événement déclenché lors des changements d'état d'authentification
        /// </summary>
        event EventHandler<AuthenticationEventArgs> AuthenticationStateChanged;

        /// <summary>
        /// Authentifie un utilisateur avec email et mot de passe
        /// </summary>
        /// <param name="email">Email de l'utilisateur</param>
        /// <param name="password">Mot de passe</param>
        /// <param name="cancellationToken">Token d'annulation</param>
        /// <returns>Token d'authentification et token de rafraîchissement</returns>
        Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authentifie un utilisateur via un fournisseur OAuth
        /// </summary>
        /// <param name="provider">Fournisseur OAuth (Google, Facebook)</param>
        /// <param name="token">Token OAuth</param>
        /// <param name="cancellationToken">Token d'annulation</param>
        /// <returns>Token d'authentification et token de rafraîchissement</returns>
        Task<AuthenticationResult> LoginWithOAuthAsync(string provider, string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Renouvelle les tokens d'authentification
        /// </summary>
        /// <param name="refreshToken">Token de rafraîchissement</param>
        /// <returns>Nouveaux tokens</returns>
        Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Déconnecte l'utilisateur
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Obtient l'utilisateur actuellement connecté
        /// </summary>
        Task<UserBasicInfo> GetCurrentUserAsync();

        /// <summary>
        /// Vérifie si un email existe dans la base de données
        /// </summary>
        /// <param name="email">Email à vérifier</param>
        /// <returns>true si l'email existe</returns>
        Task<bool> CheckEmailExistsAsync(string email);

        /// <summary>
        /// Révoque un token de rafraîchissement
        /// </summary>
        /// <param name="token">Token à révoquer</param>
        Task RevokeRefreshTokenAsync(string token);

        /// <summary>
        /// Vérifie si un token est valide
        /// </summary>
        /// <param name="token">Token à vérifier</param>
        /// <returns>true si le token est valide</returns>
        Task<bool> ValidateTokenAsync(string token);

        /// <summary>
        /// Génère un token de réinitialisation de mot de passe
        /// </summary>
        /// <param name="email">Email de l'utilisateur</param>
        /// <returns>Token de réinitialisation</returns>
        Task<string> GeneratePasswordResetTokenAsync(string email);

        /// <summary>
        /// Valide un token de réinitialisation de mot de passe
        /// </summary>
        /// <param name="token">Token de réinitialisation</param>
        /// <param name="newPassword">Nouveau mot de passe</param>
        Task<bool> ResetPasswordAsync(string token, string newPassword);

        /// <summary>
        /// Génère un token de validation d'email
        /// </summary>
        /// <param name="email">Email à valider</param>
        /// <returns>Token de validation</returns>
        Task<string> GenerateEmailValidationTokenAsync(string email);

        /// <summary>
        /// Valide l'email d'un utilisateur
        /// </summary>
        /// <param name="token">Token de validation</param>
        Task<bool> ValidateEmailAsync(string token);

        /// <summary>
        /// Vérifie si un utilisateur est dans un rôle spécifique
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="role">Rôle à vérifier</param>
        Task<bool> IsInRoleAsync(int userId, string role);

        /// <summary>
        /// Récupère les claims d'un token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Collection de claims</returns>
        Task<ClaimsPrincipal> GetClaimsFromTokenAsync(string token);
    }

    public class AuthenticationEventArgs : EventArgs
    {
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public UserBasicInfo User { get; set; }
    }

    public class AuthenticationResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        /// <summary>
        /// Date d'expiration du token d'accès
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }

        /// <summary>
        /// Type du token (Bearer)
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Informations basiques sur l'utilisateur
        /// </summary>
        public UserBasicInfo User { get; set; }

        /// <summary>
        /// Rôles de l'utilisateur
        /// </summary>
        public IEnumerable<string> Roles { get; set; }
    }

    /// <summary>
    /// Informations basiques sur l'utilisateur
    /// </summary>
    public class UserBasicInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class AuthenticationException : Exception
    {
        public string Code { get; }

        public AuthenticationException(string message, string code = null) : base(message)
        {
            Code = code;
        }
    }
}