using SubExplore.ViewModels;

namespace SubExplore.Views.Auth;

public partial class ForgotPasswordPage : ContentPage
{
    private readonly ForgotPasswordViewModel _viewModel;

    public ForgotPasswordPage(ForgotPasswordViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ErrorMessage = string.Empty;
        _viewModel.SuccessMessage = string.Empty;
    }
}