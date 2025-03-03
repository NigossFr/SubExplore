using System;
using System.ComponentModel.DataAnnotations;

namespace SubExplore.Models.Auth
{
    /// <summary>
    /// Requête d'inscription utilisateur pour l'API
    /// </summary>

    // Autres modèles d'authentification
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class PasswordResetRequest
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserBasicInfo User { get; set; }
    }
}
