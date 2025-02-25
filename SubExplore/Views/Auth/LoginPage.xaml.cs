using SubExplore.ViewModels;

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
        await _viewModel.InitializeAsync(null);
    }

    protected override bool OnBackButtonPressed()
    {
        // Désactiver le bouton retour sur la page de login
        return true;
    }
}