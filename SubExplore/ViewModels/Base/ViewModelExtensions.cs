using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SubExplore.Models;
using SubExplore.Services.Interfaces;
#if WINDOWS
using Windows.Networking.Proximity;
#endif

namespace SubExplore.ViewModels.Base;

public static class ViewModelExtensions
{
    // Validation d'email
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Expression régulière pour la validation d'email
        var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return regex.IsMatch(email);
    }

    // Construction des paramètres de navigation
    public static IDictionary<string, object> BuildNavigationParameters(this ViewModelBase viewModel, params (string Key, object Value)[] parameters)
    {
        var dict = new Dictionary<string, object>();
        foreach (var (key, value) in parameters)
        {
            dict[key] = value;
        }
        return dict;
    }

    // Extension pour la gestion de la navigation
    public static async Task NavigateToAsync<T>(this ViewModelBase viewModel, INavigationService navigationService, params (string Key, object Value)[] parameters) where T : ViewModelBase
    {
        var dict = viewModel.BuildNavigationParameters(parameters);
        // Si vous avez une classe NavigationParameters qui implémente INavigationParameters
        var navParams = new NavigationParameters(dict);
        await navigationService.NavigateToAsync<T>(navParams);

        // OU si l'API accepte directement un dictionnaire
        // await navigationService.NavigateToAsync<T>(dict);
    }

    // Conversion des coordonnées
    public static (double Latitude, double Longitude) ToLatLong(this Location location)
    {
        return (location.Latitude, location.Longitude);
    }

    // Conversion d'un niveau de difficulté en texte
    public static string ToDisplayText(this DifficultyLevel level) => level switch
    {
        DifficultyLevel.Beginner => "Débutant",
        DifficultyLevel.Intermediate => "Intermédiaire",
        DifficultyLevel.Advanced => "Avancé",
        DifficultyLevel.Expert => "Expert",
        _ => "Non spécifié"
    };

    // Formatage des dates selon la locale
    public static string ToLocaleDateString(this DateTime date)
    {
        return date.ToString("d MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
    }

    // Validation de la force du mot de passe
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

    // Validation des URLs
    public static bool IsValidUrl(this string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    // Formatage des distances
    public static string FormatDistance(this double distanceInMeters)
    {
        if (distanceInMeters < 1000)
            return $"{Math.Round(distanceInMeters)}m";
        else
            return $"{Math.Round(distanceInMeters / 1000, 1)}km";
    }

    // Formatage des profondeurs
    public static string FormatDepth(this double depthInMeters)
    {
        return $"{Math.Round(depthInMeters)}m";
    }

    // Vérification des permissions
    public static bool HasPermission(this AccountType accountType, Permission permission)
    {
        return accountType switch
        {
            AccountType.Administrator => true,
            AccountType.Moderator => permission != Permission.ManageUsers && permission != Permission.ManageRoles,
            AccountType.Professional => permission == Permission.CreateSpot || permission == Permission.EditOwnContent,
            AccountType.Standard => permission == Permission.CreateSpot || permission == Permission.EditOwnContent,
            _ => false
        };
    }

    // Vérification de la taille des fichiers
    public static bool IsValidFileSize(this long sizeInBytes, long maxSizeInMB = 5)
    {
        return sizeInBytes <= maxSizeInMB * 1024 * 1024;
    }

    // Génération du texte de statut pour les spots
    public static (string Text, string Color) GetSpotStatusInfo(this SpotStatus status)
    {
        return status switch
        {
            SpotStatus.Draft => ("Brouillon", "#666666"),
            SpotStatus.PendingReview => ("En attente de validation", "#FF9F1C"),
            SpotStatus.Active => ("Actif", "#2EC4B6"),
            SpotStatus.Inactive => ("Inactif", "#DC2626"),
            SpotStatus.Rejected => ("Rejeté", "#DC2626"),
            _ => ("Inconnu", "#666666")
        };
    }
}
