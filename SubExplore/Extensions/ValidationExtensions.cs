using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SubExplore.Extensions
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Vérifie si une chaîne est une adresse email valide
        /// </summary>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Expression régulière pour la validation d'email
                var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                return regex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Vérifie si un nom d'utilisateur est valide (alphanumérique + _ -)
        /// </summary>
        public static bool IsValidUsername(this string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            // 3-30 caractères, alphanumérique + tiret/underscore
            var regex = new Regex(@"^[a-zA-Z0-9_-]{3,30}$");
            return regex.IsMatch(username);
        }

        /// <summary>
        /// Valide la force du mot de passe
        /// </summary>
        public static (bool IsValid, string Message) ValidatePassword(this string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
                return (false, "Le mot de passe est requis");

            if (password.Length < 8)
                errors.Add("8 caractères minimum");

            if (!password.Any(char.IsUpper))
                errors.Add("une majuscule");

            if (!password.Any(char.IsLower))
                errors.Add("une minuscule");

            if (!password.Any(char.IsDigit))
                errors.Add("un chiffre");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                errors.Add("un caractère spécial");

            return errors.Count == 0
                ? (true, "Mot de passe valide")
                : (false, $"Le mot de passe doit contenir : {string.Join(", ", errors)}");
        }

        /// <summary>
        /// Vérifie si le mot de passe répond aux exigences de sécurité minimales
        /// </summary>
        public static (bool IsValid, string Strength) GetPasswordStrength(this string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, "Faible");

            int score = 0;

            // Longueur minimale
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            // Complexité
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(c => !char.IsLetterOrDigit(c))) score++;

            // Pas de répétitions de caractères consécutifs
            bool hasRepetition = false;
            for (int i = 0; i < password.Length - 1; i++)
            {
                if (password[i] == password[i + 1])
                {
                    hasRepetition = true;
                    break;
                }
            }
            if (!hasRepetition) score++;

            return score switch
            {
                0 or 1 or 2 => (false, "Faible"),
                3 or 4 => (true, "Moyen"),
                _ => (true, "Fort")
            };
        }

        /// <summary>
        /// Vérifie si une URL est valide
        /// </summary>
        public static bool IsValidUrl(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
