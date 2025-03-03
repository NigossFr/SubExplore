using SubExplore.ViewModels.Main;

namespace SubExplore.Views.Main;

public partial class StoryDetailsPage : ContentPage
{
    public StoryDetailsPage(StoryDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}