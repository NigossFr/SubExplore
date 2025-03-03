using SubExplore.ViewModels.Auth;

namespace SubExplore.Views.Auth;

public partial class ResetPasswordConfirmationPage : ContentPage
{
    public ResetPasswordConfirmationPage(ResetPasswordConfirmationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}