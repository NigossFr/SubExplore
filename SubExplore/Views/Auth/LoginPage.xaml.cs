using SubExplore.ViewModels.Auth;

namespace SubExplore.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Initialiser le ViewModel quand la page appara�t
        await _viewModel.InitializeAsync(null);

        // Vider les messages d'erreur
        _viewModel.ClearError();
    }
}