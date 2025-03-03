using SubExplore.ViewModels.Profile;

namespace SubExplore.Views.Profile;

public partial class ProfileEditPage : ContentPage
{
    public ProfileEditPage(ProfileEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}