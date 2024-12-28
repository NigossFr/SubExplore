using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using SubExplore.Services.Navigation;

namespace SubExplore.ViewModels.Base;

public partial class ViewModelBase : ObservableObject, IDisposable
{
    protected readonly INavigationService NavigationService;
    private bool _isBusy;
    private string _title;
    private bool _isRefreshing;
    private bool _isEmpty;
    private bool _isError;
    private string _errorMessage;

    [ObservableProperty]
    private bool _isLoading;

    public ViewModelBase(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public virtual void Initialize(IDictionary<string, object> parameters) { }

    public virtual Task InitializeAsync(IDictionary<string, object> parameters)
    {
        return Task.CompletedTask;
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
                OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    public bool IsNotBusy => !IsBusy;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool IsEmpty
    {
        get => _isEmpty;
        set => SetProperty(ref _isEmpty, value);
    }

    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
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

    protected virtual async Task SafeExecuteAsync(Func<Task> action, string errorMessage = "Une erreur est survenue")
    {
        try
        {
            SetBusy(true);
            ClearError();
            await action();
        }
        catch (Exception ex)
        {
            ShowError($"{errorMessage}: {ex.Message}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Error: {ex}");
#endif
        }
        finally
        {
            SetBusy(false);
        }
    }

    protected virtual void SafeExecute(Action action, string errorMessage = "Une erreur est survenue")
    {
        try
        {
            SetBusy(true);
            ClearError();
            action();
        }
        catch (Exception ex)
        {
            ShowError($"{errorMessage}: {ex.Message}");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Error: {ex}");
#endif
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

    public virtual void Dispose()
    {
        // Cleanup code here
    }
}
