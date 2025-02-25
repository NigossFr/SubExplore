using SubExplore.ViewModels.Auth;

namespace SubExplore.Views.Auth;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Initialiser le ViewModel quand la page apparaît
        await _viewModel.InitializeAsync(null);
    }
}