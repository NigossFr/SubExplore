using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Services.Interfaces
{
    /// <summary>
    /// Interface du service de navigation
    /// </summary>
    public interface INavigationService
    {
        #region Navigation de Base

        /// <summary>
        /// Navigue vers une page
        /// </summary>
        /// <typeparam name="TViewModel">Type du ViewModel de destination</typeparam>
        /// <param name="parameters">Paramètres de navigation (optionnel)</param>
        Task NavigateToAsync<TViewModel>(INavigationParameters parameters = null) where TViewModel : BaseViewModel;

        /// <summary>
        /// Navigue vers une page avec un URI
        /// </summary>
        /// <param name="uri">URI de navigation</param>
        /// <param name="parameters">Paramètres de navigation (optionnel)</param>
        Task NavigateToUriAsync(string uri, INavigationParameters parameters = null);

        /// <summary>
        /// Retourne à la page précédente
        /// </summary>
        /// <param name="parameters">Paramètres à passer à la page précédente</param>
        Task GoBackAsync(INavigationParameters parameters = null);

        /// <summary>
        /// Retourne à la racine de la navigation
        /// </summary>
        Task GoToRootAsync();

        #endregion

        #region Gestion de la Navigation Modale

        /// <summary>
        /// Affiche une page en modal
        /// </summary>
        /// <typeparam name="TViewModel">Type du ViewModel de la modal</typeparam>
        /// <param name="parameters">Paramètres de navigation (optionnel)</param>
        Task ShowModalAsync<TViewModel>(INavigationParameters parameters = null) where TViewModel : BaseViewModel;

        /// <summary>
        /// Ferme la modal courante
        /// </summary>
        /// <param name="result">Résultat à retourner (optionnel)</param>
        Task CloseModalAsync(object result = null);

        #endregion

        #region Navigation Spécifique à l'Application

        /// <summary>
        /// Navigue vers la page de détails d'un spot
        /// </summary>
        /// <param name="spotId">ID du spot</param>
        /// <param name="highlightFeature">Feature à mettre en avant (optionnel)</param>
        Task NavigateToSpotDetailsAsync(int spotId, string highlightFeature = null);

        /// <summary>
        /// Navigue vers la page de création de spot
        /// </summary>
        /// <param name="initialCoordinates">Coordonnées initiales (optionnel)</param>
        Task NavigateToSpotCreationAsync(GeoCoordinates initialCoordinates = null);

        /// <summary>
        /// Navigue vers le profil d'un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="section">Section à afficher (optionnel)</param>
        Task NavigateToUserProfileAsync(int userId, string section = null);

        /// <summary>
        /// Navigue vers une structure
        /// </summary>
        /// <param name="organizationId">ID de la structure</param>
        Task NavigateToOrganizationAsync(int organizationId);

        #endregion

        #region Gestion de l'État

        /// <summary>
        /// Enregistre l'état de navigation actuel
        /// </summary>
        /// <returns>Clé de l'état sauvegardé</returns>
        Task<string> SaveNavigationStateAsync();

        /// <summary>
        /// Restaure un état de navigation
        /// </summary>
        /// <param name="stateKey">Clé de l'état à restaurer</param>
        Task RestoreNavigationStateAsync(string stateKey);

        /// <summary>
        /// Obtient l'historique de navigation
        /// </summary>
        IReadOnlyList<NavigationHistoryEntry> NavigationHistory { get; }

        /// <summary>
        /// Vérifie si la navigation arrière est possible
        /// </summary>
        bool CanGoBack { get; }

        #endregion

        #region Configuration et Middleware

        /// <summary>
        /// Ajoute un middleware de navigation
        /// </summary>
        /// <param name="middleware">Middleware à ajouter</param>
        void AddNavigationMiddleware(INavigationMiddleware middleware);

        /// <summary>
        /// Configure les routes de l'application
        /// </summary>
        /// <param name="routes">Configuration des routes</param>
        void ConfigureRoutes(IDictionary<string, Type> routes);

        /// <summary>
        /// Configure le comportement de la navigation
        /// </summary>
        /// <param name="options">Options de navigation</param>
        void ConfigureNavigation(NavigationOptions options);

        #endregion
    }

    /// <summary>
    /// Paramètres de navigation
    /// </summary>
    public interface INavigationParameters : IDictionary<string, object>
    {
        /// <summary>
        /// Récupère une valeur typée
        /// </summary>
        T GetValue<T>(string key);

        /// <summary>
        /// Ajoute ou met à jour une valeur
        /// </summary>
        void Add<T>(string key, T value);
    }

    /// <summary>
    /// Entrée dans l'historique de navigation
    /// </summary>
    public class NavigationHistoryEntry
    {
        public Type ViewModelType { get; set; }
        public string PageTitle { get; set; }
        public DateTime Timestamp { get; set; }
        public INavigationParameters Parameters { get; set; }
    }

    /// <summary>
    /// Options de configuration de la navigation
    /// </summary>
    public class NavigationOptions
    {
        public bool EnableDeepLinking { get; set; } = true;
        public bool ClearNavigationStackOnLogin { get; set; } = true;
        public bool SaveNavigationHistory { get; set; } = true;
        public int MaxHistoryEntries { get; set; } = 50;
        public TimeSpan NavigationTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public IDictionary<string, string> DeepLinkMappings { get; set; }
        public NavigationAnimationOptions AnimationOptions { get; set; }
    }

    /// <summary>
    /// Options d'animation pour la navigation
    /// </summary>
    public class NavigationAnimationOptions
    {
        public bool EnableAnimations { get; set; } = true;
        public int AnimationDuration { get; set; } = 250;
        public string TransitionType { get; set; } = "slide";
    }

    /// <summary>
    /// Interface pour les middleware de navigation
    /// </summary>
    public interface INavigationMiddleware
    {
        Task OnNavigatingAsync(NavigationContext context, NavigationDelegate next);
    }
}
