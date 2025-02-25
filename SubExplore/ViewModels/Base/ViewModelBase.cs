using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using SubExplore.Services.Interfaces;

namespace SubExplore.ViewModels.Base;

public partial class ViewModelBase : ObservableObject, IDisposable, IQueryAttributable
{
    protected readonly INavigationService NavigationService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _canNavigateBack;

    public bool IsNotBusy => !IsBusy;

    public ViewModelBase(INavigationService navigationService)
    {
        NavigationService = navigationService;
        UpdateNavigationState();
    }

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Initialize(query);
    }

    public virtual void Initialize(IDictionary<string, object> parameters) { }

    public virtual Task InitializeAsync(IDictionary<string, object> parameters)
    {
        return Task.CompletedTask;
    }

    protected virtual void UpdateNavigationState()
    {
        CanNavigateBack = NavigationService.CanGoBack;
    }

    protected virtual void SetBusy(bool value)
    {
        IsBusy = value;
        IsLoading = value;
    }

    protected virtual void ShowError(string message)
    {
        IsError = true;
        ErrorMessage = message;
    }

    protected virtual void ClearError()
    {
        IsError = false;
        ErrorMessage = string.Empty;
    }

    protected virtual void HandleError(Exception ex, string customMessage = null)
    {
        var message = customMessage ?? "Une erreur est survenue";

        if (ex is AuthenticationException authEx)
        {
            message = $"Erreur d'authentification : {authEx.Message}";
        }
        else if (ex is NavigationException navEx)
        {
            message = $"Erreur de navigation : {navEx.Message}";
        }
        else if (ex is HttpRequestException httpEx)
        {
            message = "Erreur de connexion au serveur";
        }

        ShowError(message);
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Error: {ex}");
#endif
    }

    protected virtual async Task SafeExecuteAsync(Func<Task> action, string errorMessage = null)
    {
        try
        {
            SetBusy(true);
            ClearError();
            await action();
        }
        catch (Exception ex)
        {
            HandleError(ex, errorMessage);
        }
        finally
        {
            SetBusy(false);
        }
    }

    protected virtual void SafeExecute(Action action, string errorMessage = null)
    {
        try
        {
            SetBusy(true);
            ClearError();
            action();
        }
        catch (Exception ex)
        {
            HandleError(ex, errorMessage);
        }
        finally
        {
            SetBusy(false);
        }
    }

    protected virtual Task DisplayAlert(string title, string message, string button = "OK")
    {
        return Shell.Current.DisplayAlert(title, message, button);
    }

    protected virtual Task<bool> DisplayConfirmation(string title, string message, string accept = "Oui", string cancel = "Non")
    {
        return Shell.Current.DisplayAlert(title, message, accept, cancel);
    }

    protected async Task NavigateBackAsync()
    {
        if (NavigationService.CanGoBack)
            await NavigationService.GoBackAsync();
    }

    protected async Task NavigateToAsync(string route, IDictionary<string, object> parameters = null)
    {
        await NavigationService.NavigateToAsync(route, parameters);
    }

    protected async Task ShowModalAsync<TViewModel>(IDictionary<string, object> parameters = null)
        where TViewModel : ViewModelBase
    {
        await NavigationService.ShowModalAsync<TViewModel>(new NavigationParameters(parameters));
    }

    protected async Task CloseModalAsync(object result = null)
    {
        await NavigationService.CloseModalAsync(result);
    }

    public virtual void Dispose()
    {
        // Code de nettoyage ici
    }
}